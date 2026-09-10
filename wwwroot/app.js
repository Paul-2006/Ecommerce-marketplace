const state = {
  token: localStorage.getItem("token") || sessionStorage.getItem("token"),
  user: JSON.parse(localStorage.getItem("user") || sessionStorage.getItem("user") || "null"),
  resetEmail: ""
};

const roleNames = {
  1: "Admin",
  2: "Seller",
  3: "Warehouse",
  4: "Delivery",
  5: "Customer"
};

const $ = (selector) => document.querySelector(selector);

function showMessage(text, isError = false) {
  const message = $("#message");
  message.textContent = text;
  message.style.background = isError ? "#fff1f0" : "#fff8e8";
  message.style.borderColor = isError ? "#efb1aa" : "#efd397";
  message.classList.remove("hidden");
}

function hideMessage() {
  $("#message").classList.add("hidden");
}

async function api(path, options = {}) {
  const headers = options.headers ? { ...options.headers } : {};

  if (state.token) {
    headers.Authorization = `Bearer ${state.token}`;
  }

  if (options.body && !(options.body instanceof FormData)) {
    headers["Content-Type"] = "application/json";
    options.body = JSON.stringify(options.body);
  }

  const response = await fetch(path, {
    ...options,
    headers
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || `Request failed with status ${response.status}`);
  }

  const text = await response.text();
  return text ? JSON.parse(text) : null;
}

function saveSession(loginData, rememberMe) {
  state.token = loginData.token;
  state.user = loginData;

  const storage = rememberMe ? localStorage : sessionStorage;
  const otherStorage = rememberMe ? sessionStorage : localStorage;

  storage.setItem("token", loginData.token);
  storage.setItem("user", JSON.stringify(loginData));
  otherStorage.removeItem("token");
  otherStorage.removeItem("user");
}

function clearSession() {
  state.token = null;
  state.user = null;
  localStorage.removeItem("token");
  localStorage.removeItem("user");
  sessionStorage.removeItem("token");
  sessionStorage.removeItem("user");
}

function renderShell() {
  const isLoggedIn = Boolean(state.token);

  $("#authPanel").classList.toggle("hidden", isLoggedIn);
  $("#dashboard").classList.toggle("hidden", !isLoggedIn);
  $("#logoutBtn").classList.toggle("hidden", !isLoggedIn);

  if (!isLoggedIn) {
    return;
  }

  const role = roleNames[state.user.roleId] || state.user.role || "User";
  $("#welcomeTitle").textContent = `${role} Dashboard`;
  $("#tokenNote").textContent = state.user.tokenExpiresAt
    ? `Session expires: ${new Date(state.user.tokenExpiresAt).toLocaleString()}`
    : "";

  $("#adminPanel").classList.toggle("hidden", state.user.roleId !== 1);
  $("#sellerPanel").classList.toggle("hidden", state.user.roleId !== 2);
  $("#warehousePanel").classList.toggle("hidden", state.user.roleId !== 3 && state.user.roleId !== 1);
  $("#deliveryPanel").classList.toggle("hidden", state.user.roleId !== 4 && state.user.roleId !== 1);

  loadProfile();
  loadProducts();

  if (state.user.roleId === 1) {
    loadAdminSummary();
  }

  if (state.user.roleId === 2 && state.user.sellerId) {
    loadSellerProducts();
  }

  if (state.user.roleId === 3 || state.user.roleId === 1) {
    loadStock();
  }
}

function renderObject(container, value) {
  if (!value) {
    container.innerHTML = "<p class=\"muted\">No data found.</p>";
    return;
  }

  container.innerHTML = Object.entries(value)
    .filter(([, item]) => item !== null && item !== undefined && typeof item !== "object")
    .map(([key, item]) => `<p><strong>${formatKey(key)}:</strong> ${escapeHtml(item)}</p>`)
    .join("");
}

function formatKey(key) {
  return key.replace(/([A-Z])/g, " $1").replace(/^./, (letter) => letter.toUpperCase());
}

function escapeHtml(value) {
  return String(value)
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll("\"", "&quot;")
    .replaceAll("'", "&#039;");
}

async function loadProfile() {
  try {
    const profile = await api("/api/Profile/Me");
    const card = $("#profileCard");
    const roleBlock = profile.customer || profile.seller || profile.deliveryPartner || profile.warehouseManager;

    card.innerHTML = `
      <p><strong>Name:</strong> ${escapeHtml(profile.username)}</p>
      <p><strong>Email:</strong> ${escapeHtml(profile.email)}</p>
      <p><strong>Phone:</strong> ${escapeHtml(profile.phoneNumber || "Not set")}</p>
      <p><strong>Role:</strong> ${escapeHtml(profile.role || roleNames[profile.roleId] || "User")}</p>
      <hr>
      ${roleBlock
        ? Object.entries(roleBlock).map(([key, value]) => `<p><strong>${formatKey(key)}:</strong> ${escapeHtml(value ?? "Not set")}</p>`).join("")
        : "<p class=\"muted\">No role profile details found.</p>"}
    `;

    $("#profileForm").username.value = profile.username || "";
    $("#profileForm").phoneNumber.value = profile.phoneNumber || "";
  } catch (error) {
    showMessage(error.message, true);
  }
}

async function loadProducts() {
  try {
    const products = await api("/api/Product");
    const list = $("#productsList");

    if (!products.length) {
      list.innerHTML = "<p class=\"muted\">No products found.</p>";
      return;
    }

    list.innerHTML = products.map((product) => `
      <article class="item">
        ${product.image ? `<img class="image-thumb" src="${escapeHtml(product.image)}" alt="${escapeHtml(product.productName || "Product")}">` : ""}
        <strong>${escapeHtml(product.productName || "Unnamed product")}</strong>
        <span>${escapeHtml(product.brand || "No brand")} - ${escapeHtml(product.description || "No description")}</span>
        <span>Price: ${escapeHtml(product.price ?? "N/A")} | Stock: ${escapeHtml(product.stock ?? "N/A")}</span>
        <span class="muted">Product ID: ${escapeHtml(product.productId)} | Seller Product ID: ${escapeHtml(product.sellerProductId)}</span>
      </article>
    `).join("");
  } catch (error) {
    $("#productsList").innerHTML = `<p class="muted">${escapeHtml(error.message)}</p>`;
  }
}

async function loadSellerProducts() {
  try {
    const products = await api(`/api/SellerProduct/Seller/${state.user.sellerId}`);
    $("#sellerProducts").innerHTML = products.length
      ? products.map((product) => `
        <article class="item">
          <strong>${escapeHtml(product.productName)}</strong>
          <span>Product ID: ${escapeHtml(product.productId)}</span>
          <span>Price: ${escapeHtml(product.price)} | Stock: ${escapeHtml(product.stock)}</span>
        </article>
      `).join("")
      : "<p class=\"muted\">No seller products found.</p>";
  } catch (error) {
    $("#sellerProducts").innerHTML = `<p class="muted">${escapeHtml(error.message)}</p>`;
  }
}

async function loadAdminSummary() {
  try {
    const summary = await api("/api/AdminDashboard/Summary");
    $("#adminSummary").innerHTML = Object.entries(summary)
      .map(([key, value]) => `
        <div class="metric">
          <strong>${formatKey(key)}</strong>
          <span>${escapeHtml(value)}</span>
        </div>
      `).join("");
  } catch (error) {
    $("#adminSummary").innerHTML = `<p class="muted">${escapeHtml(error.message)}</p>`;
  }
}

async function loadStock() {
  try {
    const stock = await api("/api/WarehouseInventory/Details");
    $("#stockList").innerHTML = stock.length
      ? stock.map((item) => `
        <article class="item">
          <strong>${escapeHtml(item.productName)}</strong>
          <span>${escapeHtml(item.warehouseName)} - ${escapeHtml(item.warehouseLocation || "No location")}</span>
          <span>Quantity: ${escapeHtml(item.quantity)} | ${escapeHtml(item.stockStatus)}</span>
          <span class="muted">${escapeHtml(item.brand || "No brand")} | ${escapeHtml(item.categoryName || "No category")}</span>
        </article>
      `).join("")
      : "<p class=\"muted\">No warehouse stock found.</p>";
  } catch (error) {
    $("#stockList").innerHTML = `<p class="muted">${escapeHtml(error.message)}</p>`;
  }
}

function bindTabs() {
  document.querySelectorAll(".tab").forEach((tab) => {
    tab.addEventListener("click", () => {
      document.querySelectorAll(".tab").forEach((item) => item.classList.remove("active"));
      document.querySelectorAll("[data-tab-panel]").forEach((panel) => panel.classList.add("hidden"));
      tab.classList.add("active");
      document.querySelectorAll(`[data-tab-panel="${tab.dataset.tab}"]`).forEach((panel) => panel.classList.remove("hidden"));
    });
  });
}

function formDataToObject(form) {
  return Object.fromEntries(new FormData(form).entries());
}

$("#loginForm").addEventListener("submit", async (event) => {
  event.preventDefault();
  hideMessage();

  const form = event.currentTarget;
  const data = formDataToObject(form);
  const portal = data.portal ? `/${data.portal}` : "";
  const rememberMe = form.rememberMe.checked;

  try {
    const loginData = await api(`/api/Auth/Login${portal}`, {
      method: "POST",
      body: {
        email: data.email,
        password: data.password,
        rememberMe
      }
    });

    saveSession(loginData, rememberMe);
    renderShell();
    showMessage("Login successful.");
  } catch (error) {
    showMessage(error.message, true);
  }
});

$("#registerForm").addEventListener("submit", async (event) => {
  event.preventDefault();
  hideMessage();

  const data = formDataToObject(event.currentTarget);

  try {
    await api("/api/Auth/Register", {
      method: "POST",
      body: {
        username: data.username,
        email: data.email,
        password: data.password,
        phoneNumber: data.phoneNumber,
        roleId: Number(data.roleId)
      }
    });
    showMessage("Account created. You can login now.");
  } catch (error) {
    showMessage(error.message, true);
  }
});

$("#forgotForm").addEventListener("submit", async (event) => {
  event.preventDefault();
  hideMessage();

  const data = formDataToObject(event.currentTarget);
  state.resetEmail = data.email;

  try {
    const result = await api("/api/Auth/ForgotPassword", {
      method: "POST",
      body: { email: data.email }
    });
    showMessage(`OTP generated: ${result.otp}`);
  } catch (error) {
    showMessage(error.message, true);
  }
});

$("#resetForm").addEventListener("submit", async (event) => {
  event.preventDefault();
  hideMessage();

  const data = formDataToObject(event.currentTarget);

  try {
    await api("/api/Auth/ResetPassword", {
      method: "POST",
      body: {
        email: state.resetEmail,
        otp: data.otp,
        newPassword: data.newPassword
      }
    });
    showMessage("Password reset successful. Login with your new password.");
  } catch (error) {
    showMessage(error.message, true);
  }
});

$("#profileForm").addEventListener("submit", async (event) => {
  event.preventDefault();
  hideMessage();

  const data = formDataToObject(event.currentTarget);
  const body = Object.fromEntries(Object.entries(data).filter(([, value]) => value));

  try {
    await api("/api/Profile/Me", {
      method: "PUT",
      body
    });
    showMessage("Profile saved.");
    loadProfile();
  } catch (error) {
    showMessage(error.message, true);
  }
});

$("#imageForm").addEventListener("submit", async (event) => {
  event.preventDefault();
  hideMessage();

  const form = event.currentTarget;
  const productId = form.productId.value;
  const file = form.image.files[0];
  const isPrimary = form.isPrimary.checked;
  const body = new FormData();
  body.append("image", file);

  try {
    const result = await api(`/api/ProductImage/Upload?productId=${encodeURIComponent(productId)}&isPrimary=${isPrimary}`, {
      method: "POST",
      body
    });
    showMessage(`Image uploaded: ${result.imageUrl}`);
    form.reset();
    loadProducts();
  } catch (error) {
    showMessage(error.message, true);
  }
});

$("#gpsForm").addEventListener("submit", async (event) => {
  event.preventDefault();
  hideMessage();

  const orderId = event.currentTarget.orderId.value;

  try {
    const data = await api(`/api/DeliveryLocation/Live/${orderId}`);
    const card = $("#gpsCard");
    card.innerHTML = `
      <p><strong>Order:</strong> ${escapeHtml(data.orderId)}</p>
      <p><strong>Status:</strong> ${escapeHtml(data.deliveryStatus || data.status || data.orderStatus || "Not set")}</p>
      <p><strong>Partner:</strong> ${escapeHtml(data.deliveryPartner?.partnerName || "Not set")}</p>
      <p><strong>Vehicle:</strong> ${escapeHtml(data.deliveryPartner?.vehicleNumber || "Not set")}</p>
      ${data.gps
        ? `<p><strong>Latitude:</strong> ${escapeHtml(data.gps.latitude)}</p>
           <p><strong>Longitude:</strong> ${escapeHtml(data.gps.longitude)}</p>
           <p><strong>Updated:</strong> ${escapeHtml(data.gps.trackingTime)}</p>
           <p><a class="ghost-link" target="_blank" rel="noreferrer" href="https://www.google.com/maps?q=${encodeURIComponent(data.gps.latitude)},${encodeURIComponent(data.gps.longitude)}">Open map</a></p>`
        : "<p class=\"muted\">GPS is not available yet.</p>"}
    `;
  } catch (error) {
    $("#gpsCard").innerHTML = `<p class="muted">${escapeHtml(error.message)}</p>`;
  }
});

$("#logoutBtn").addEventListener("click", () => {
  clearSession();
  renderShell();
});

$("#refreshProfileBtn").addEventListener("click", loadProfile);
$("#loadProductsBtn").addEventListener("click", loadProducts);
$("#loadAdminBtn").addEventListener("click", loadAdminSummary);
$("#loadStockBtn").addEventListener("click", loadStock);

bindTabs();
renderShell();
