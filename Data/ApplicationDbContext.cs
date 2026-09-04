using System;
using System.Collections.Generic;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }


    public virtual DbSet<Adminaction> Adminactions { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Cartitem> Cartitems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Customeraddress> Customeraddresses { get; set; }

    public virtual DbSet<Customerfeedback> Customerfeedbacks { get; set; }

    public virtual DbSet<Customerwallet> Customerwallets { get; set; }

    public virtual DbSet<Warehouseinventory> Warehouseinventories { get; set; }

    public virtual DbSet<Deliveryassignment> Deliveryassignments { get; set; }

    public virtual DbSet<Deliveryinstruction> Deliveryinstructions { get; set; }

    public virtual DbSet<Deliverylocationtracking> Deliverylocationtrackings { get; set; }

    public virtual DbSet<Deliveryotp> Deliveryotps { get; set; }

    public virtual DbSet<Deliverypartner> Deliverypartners { get; set; }

    public virtual DbSet<Deliverypartnerzone> Deliverypartnerzones { get; set; }

    public virtual DbSet<Deliveryzone> Deliveryzones { get; set; }

    public virtual DbSet<Faileddelivery> Faileddeliveries { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Orderitem> Orderitems { get; set; }

    public virtual DbSet<Orderstatushistory> Orderstatushistories { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Pricedropnotification> Pricedropnotifications { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Productapproval> Productapprovals { get; set; }

    public virtual DbSet<Productcomparison> Productcomparisons { get; set; }

    public virtual DbSet<Productimage> Productimages { get; set; }

    public virtual DbSet<Productrecommendation> Productrecommendations { get; set; }

    public virtual DbSet<Productreview> Productreviews { get; set; }

    public virtual DbSet<Productspecification> Productspecifications { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Searchhistory> Searchhistories { get; set; }

    public virtual DbSet<Seller> Sellers { get; set; }

    public virtual DbSet<Sellerorder> Sellerorders { get; set; }

    public virtual DbSet<Sellerperformance> Sellerperformances { get; set; }

    public virtual DbSet<Sellerproduct> Sellerproducts { get; set; }

    public virtual DbSet<Sellerstatushistory> Sellerstatushistories { get; set; }

    public virtual DbSet<Sellerwarning> Sellerwarnings { get; set; }

    public virtual DbSet<Spinhistory> Spinhistories { get; set; }

    public virtual DbSet<Spinreward> Spinrewards { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    public virtual DbSet<Warehousemanager> Warehousemanagers { get; set; }

    public virtual DbSet<Warehousenotification> Warehousenotifications { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    public virtual DbSet<Wishlistitem> Wishlistitems { get; set; }



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql(
                "server=localhost;port=33069;database=ecommercemarketplacedb;user=root;password=root123",
                Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.42-mysql")
            );
        }
    }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Adminaction>(e => e.HasKey(x => x.ActionId));
        modelBuilder.Entity<Cart>(e => e.HasKey(x => x.CartId));
        modelBuilder.Entity<Cartitem>(e => e.HasKey(x => x.CartItemId));
        modelBuilder.Entity<Category>(e => e.HasKey(x => x.CategoryId));
        modelBuilder.Entity<Customer>(e => e.HasKey(x => x.CustomerId));
        modelBuilder.Entity<Customeraddress>(e => e.HasKey(x => x.AddressId));
        modelBuilder.Entity<Customerfeedback>(e => e.HasKey(x => x.FeedbackId));
        modelBuilder.Entity<Customerwallet>(e => e.HasKey(x => x.WalletId));
        modelBuilder.Entity<Warehouseinventory>(e => e.HasKey(x => x.WarehouseInventoryId));
        modelBuilder.Entity<Deliveryassignment>(e => e.HasKey(x => x.DeliveryAssignmentId));
        modelBuilder.Entity<Deliveryinstruction>(e => e.HasKey(x => x.InstructionId));
        modelBuilder.Entity<Deliverylocationtracking>(e => e.HasKey(x => x.LocationId));
        modelBuilder.Entity<Deliveryotp>(e => e.HasKey(x => x.Otpid));
        modelBuilder.Entity<Deliverypartner>(e => e.HasKey(x => x.DeliveryPartnerId));
        modelBuilder.Entity<Deliverypartnerzone>(e => e.HasKey(x => x.Id));
        modelBuilder.Entity<Deliveryzone>(e => e.HasKey(x => x.ZoneId));
        modelBuilder.Entity<Faileddelivery>(e => e.HasKey(x => x.FailedDeliveryId));
        modelBuilder.Entity<Notification>(e => e.HasKey(x => x.NotificationId));
        modelBuilder.Entity<Order>(e => e.HasKey(x => x.OrderId));
        modelBuilder.Entity<Orderitem>(e => e.HasKey(x => x.OrderItemId));
        modelBuilder.Entity<Orderstatushistory>(e => e.HasKey(x => x.HistoryId));
        modelBuilder.Entity<Payment>(e => e.HasKey(x => x.PaymentId));
        modelBuilder.Entity<Pricedropnotification>(e => e.HasKey(x => x.PriceDropId));
        modelBuilder.Entity<Product>(e => e.HasKey(x => x.ProductId));
        modelBuilder.Entity<Productapproval>(e => e.HasKey(x => x.ApprovalId));
        modelBuilder.Entity<Productcomparison>(e => e.HasKey(x => x.ComparisonId));
        modelBuilder.Entity<Productimage>(e => e.HasKey(x => x.ImageId));
        modelBuilder.Entity<Productrecommendation>(e => e.HasKey(x => x.RecommendationId));
        modelBuilder.Entity<Productreview>(e => e.HasKey(x => x.ReviewId));
        modelBuilder.Entity<Productspecification>(e => e.HasKey(x => x.SpecificationId));
        modelBuilder.Entity<Role>(e => e.HasKey(x => x.RoleId));
        modelBuilder.Entity<Searchhistory>(e => e.HasKey(x => x.SearchId));
        modelBuilder.Entity<Seller>(e => e.HasKey(x => x.SellerId));
        modelBuilder.Entity<Sellerorder>(e => e.HasKey(x => x.SellerOrderId));
        modelBuilder.Entity<Sellerperformance>(e => e.HasKey(x => x.PerformanceId));
        modelBuilder.Entity<Sellerproduct>(e => e.HasKey(x => x.SellerProductId));
        modelBuilder.Entity<Sellerstatushistory>(e => e.HasKey(x => x.HistoryId));
        modelBuilder.Entity<Sellerwarning>(e => e.HasKey(x => x.WarningId));
        modelBuilder.Entity<Spinhistory>(e => e.HasKey(x => x.SpinId));
        modelBuilder.Entity<Spinreward>(e => e.HasKey(x => x.RewardId));
        modelBuilder.Entity<User>(e => e.HasKey(x => x.UserId));
        modelBuilder.Entity<Warehouse>(e => e.HasKey(x => x.WarehouseId));
        modelBuilder.Entity<Warehousemanager>(e => e.HasKey(x => x.WarehouseManagerId));
        modelBuilder.Entity<Warehousenotification>(e => e.HasKey(x => x.NotificationId));
        modelBuilder.Entity<Wishlist>(e => e.HasKey(x => x.WishlistId));
        modelBuilder.Entity<Wishlistitem>(e => e.HasKey(x => x.WishlistItemId));

        // CART ITEM

        modelBuilder.Entity<Cartitem>(entity =>
        {
            entity.HasKey(e => e.CartItemId)
                .HasName("PRIMARY");

            entity.ToTable("cartitems");


            entity.HasIndex(e => e.CartId)
                .HasDatabaseName("CartID");

            entity.HasIndex(e => e.SellerProductId)
                .HasDatabaseName("SellerProductID");

            entity.HasIndex(e => e.ProductId)
                .HasDatabaseName("ProductID");


            entity.Property(e => e.CartItemId)
                .HasColumnName("CartItemID");

            entity.Property(e => e.CartId)
                .HasColumnName("CartID");

            entity.Property(e => e.SellerProductId)
                .HasColumnName("SellerProductID");

            entity.Property(e => e.ProductId)
                .HasColumnName("ProductID");

            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("'1'");


            entity.HasOne(e => e.Cart)
                .WithMany(c => c.Cartitems)
                .HasForeignKey(e => e.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cartitems_ibfk_1");


            entity.HasOne(e => e.SellerProduct)
                .WithMany(sp => sp.Cartitems)
                .HasForeignKey(e => e.SellerProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cartitems_ibfk_sellerproduct");
        });




        // PRODUCT

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId)
                .HasName("PRIMARY");


            entity.ToTable("products");


            entity.HasIndex(e => e.CategoryId)
                .HasDatabaseName("CategoryID");


            entity.Property(e => e.ProductId)
                .HasColumnName("ProductID");


            entity.Property(e => e.CategoryId)
                .HasColumnName("CategoryID");


            entity.Property(e => e.ProductName)
                .HasMaxLength(200);


            entity.Property(e => e.Description)
                .HasColumnType("text");


            entity.Property(e => e.Brand)
                .HasMaxLength(100);


            entity.Property(e => e.Warranty)
                .HasMaxLength(100);


            entity.Property(e => e.ApprovalStatus)
                .HasDefaultValueSql("'Pending'")
                .HasColumnType(
                "enum('Pending','Approved','Rejected')");


            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");


            entity.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("products_ibfk_1");
        });




        // SELLER PRODUCT

        modelBuilder.Entity<Sellerproduct>(entity =>
        {
            entity.HasKey(e => e.SellerProductId)
                .HasName("PRIMARY");


            entity.ToTable("sellerproducts");


            entity.HasIndex(e => e.ProductId)
                .HasDatabaseName("ProductID");


            entity.HasIndex(e => e.SellerId)
                .HasDatabaseName("SellerID");



            entity.Property(e => e.SellerProductId)
                .HasColumnName("SellerProductID");


            entity.Property(e => e.SellerId)
                .HasColumnName("SellerID");


            entity.Property(e => e.ProductId)
                .HasColumnName("ProductID");


            entity.Property(e => e.Price)
                .HasPrecision(10, 2);


            entity.Property(e => e.Discount)
                .HasPrecision(10, 2);


            entity.Property(e => e.StockQuantity)
                .HasDefaultValueSql("'0'");


            entity.Property(e => e.ProductStatus)
                .HasDefaultValueSql("'Available'")
                .HasColumnType(
                "enum('Available','Out Of Stock','Blocked')");



            entity.HasOne(e => e.Product)
                .WithMany(p => p.Sellerproducts)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sellerproducts_ibfk_1");



            entity.HasOne(e => e.Seller)
                .WithMany(s => s.Sellerproducts)
                .HasForeignKey(e => e.SellerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sellerproducts_ibfk_2");
        });
        // ORDER

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId)
                .HasName("PRIMARY");

            entity.ToTable("orders");


            entity.HasIndex(e => e.CustomerId)
                .HasDatabaseName("CustomerID");

            entity.HasIndex(e => e.AddressId)
                .HasDatabaseName("AddressID");


            entity.Property(e => e.OrderId)
                .HasColumnName("OrderID");

            entity.Property(e => e.CustomerId)
                .HasColumnName("CustomerID");

            entity.Property(e => e.AddressId)
                .HasColumnName("AddressID");

            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2);

            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.Property(e => e.OrderStatus)
                .HasDefaultValueSql("'Placed'")
                .HasColumnType(
                "enum('Placed','Confirmed','Packed','Shipped','Out For Delivery','Delivered','Cancelled','Returned')");


            entity.Property(e => e.PaymentMethod)
                .HasColumnType(
                "enum('Cash On Delivery','Online Payment')");



            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_ibfk_1");



            entity.HasOne(e => e.Address)
                .WithMany(a => a.Orders)
                .HasForeignKey(e => e.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_ibfk_2");
        });




        // ORDER ITEM

        modelBuilder.Entity<Orderitem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId)
                .HasName("PRIMARY");


            entity.ToTable("orderitems");


            entity.HasIndex(e => e.OrderId)
                .HasDatabaseName("OrderID");


            entity.HasIndex(e => e.SellerProductId)
                .HasDatabaseName("SellerProductID");



            entity.Property(e => e.OrderItemId)
                .HasColumnName("OrderItemID");


            entity.Property(e => e.OrderId)
                .HasColumnName("OrderID");


            entity.Property(e => e.SellerProductId)
                .HasColumnName("SellerProductID");


            entity.Property(e => e.Price)
                .HasPrecision(10, 2);



            entity.HasOne(e => e.Order)
                .WithMany(o => o.Orderitems)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orderitems_ibfk_1");



            entity.HasOne(e => e.SellerProduct)
                .WithMany(sp => sp.Orderitems)
                .HasForeignKey(e => e.SellerProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orderitems_ibfk_2");
        });




        // PAYMENT

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId)
                .HasName("PRIMARY");


            entity.ToTable("payments");


            entity.HasIndex(e => e.OrderId)
                .HasDatabaseName("OrderID");



            entity.Property(e => e.PaymentId)
                .HasColumnName("PaymentID");


            entity.Property(e => e.OrderId)
                .HasColumnName("OrderID");


            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50);


            entity.Property(e => e.PaymentStatus)
                .HasColumnType(
                "enum('Pending','Success','Failed','Refunded')");


            entity.Property(e => e.TransactionId)
                .HasMaxLength(100)
                .HasColumnName("TransactionID");


            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");



            entity.HasOne(e => e.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payments_ibfk_1");
        });




        // WISHLIST

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.WishlistId)
                .HasName("PRIMARY");


            entity.ToTable("wishlists");


            entity.HasIndex(e => e.CustomerId)
                .HasDatabaseName("CustomerID");


            entity.Property(e => e.WishlistId)
                .HasColumnName("WishlistID");


            entity.Property(e => e.CustomerId)
                .HasColumnName("CustomerID");


            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");



            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Wishlists)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("wishlists_ibfk_1");
        });




        // WISHLIST ITEM

        modelBuilder.Entity<Wishlistitem>(entity =>
        {
            entity.HasKey(e => e.WishlistItemId)
                .HasName("PRIMARY");


            entity.ToTable("wishlistitems");


            entity.HasIndex(e => e.WishlistId)
                .HasDatabaseName("WishlistID");


            entity.HasIndex(e => e.ProductId)
                .HasDatabaseName("ProductID");



            entity.Property(e => e.WishlistItemId)
                .HasColumnName("WishlistItemID");


            entity.Property(e => e.WishlistId)
                .HasColumnName("WishlistID");


            entity.Property(e => e.ProductId)
                .HasColumnName("ProductID");



            entity.HasOne(e => e.Wishlist)
                .WithMany(w => w.Wishlistitems)
                .HasForeignKey(e => e.WishlistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("wishlistitems_ibfk_1");



            entity.HasOne(e => e.Product)
                .WithMany(p => p.Wishlistitems)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("wishlistitems_ibfk_2");
        });
        // Remaining entities


        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId)
                .HasName("PRIMARY");

            entity.ToTable("categories");

            entity.Property(e => e.CategoryId)
                .HasColumnName("CategoryID");

            entity.Property(e => e.CategoryName)
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(255);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");


            entity.HasOne(e => e.ParentCategory)
                .WithMany(c => c.InverseParentCategory)
                .HasForeignKey(e => e.ParentCategoryId)
                .HasConstraintName("categories_ibfk_1");
        });



        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId)
                .HasName("PRIMARY");

            entity.ToTable("customers");


            entity.Property(e => e.CustomerId)
                .HasColumnName("CustomerID");


            entity.Property(e => e.UserId)
                .HasColumnName("UserID");



            entity.HasOne(e => e.User)
                .WithMany(u => u.Customers)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customers_ibfk_1");
        });



        modelBuilder.Entity<Customeraddress>(entity =>
        {
            entity.HasKey(e => e.AddressId)
                .HasName("PRIMARY");


            entity.ToTable("customeraddresses");


            entity.Property(e => e.AddressId)
                .HasColumnName("AddressID");


            entity.Property(e => e.CustomerId)
                .HasColumnName("CustomerID");


            entity.Property(e => e.AddressLine)
                .HasMaxLength(255);


            entity.Property(e => e.City)
                .HasMaxLength(100);


            entity.Property(e => e.State)
                .HasMaxLength(100);



            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Customeraddresses)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customeraddresses_ibfk_1");
        });



        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId)
                .HasName("PRIMARY");


            entity.ToTable("users");


            entity.Property(e => e.UserId)
                .HasColumnName("UserID");


            entity.Property(e => e.RoleId)
                .HasColumnName("RoleID");


            entity.Property(e => e.Email)
                .HasMaxLength(150);


            entity.Property(e => e.Username)
                .HasMaxLength(100);



            entity.HasOne(e => e.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_ibfk_1");
        });



        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId)
                .HasName("PRIMARY");


            entity.ToTable("roles");


            entity.Property(e => e.RoleId)
                .HasColumnName("RoleID");


            entity.Property(e => e.RoleName)
                .HasMaxLength(50);
        });



        modelBuilder.Entity<Seller>(entity =>
        {
            entity.HasKey(e => e.SellerId)
                .HasName("PRIMARY");


            entity.ToTable("sellers");


            entity.Property(e => e.SellerId)
                .HasColumnName("SellerID");


            entity.Property(e => e.UserId)
                .HasColumnName("UserID");



            entity.HasOne(e => e.User)
                .WithMany(u => u.Sellers)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sellers_ibfk_1");
        });



        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId)
                .HasName("PRIMARY");


            entity.ToTable("notifications");


            entity.Property(e => e.NotificationId)
                .HasColumnName("NotificationID");


            entity.Property(e => e.UserId)
                .HasColumnName("UserID");



            entity.HasOne(e => e.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("notifications_ibfk_1");
        });



        modelBuilder.Entity<Productimage>(entity =>
        {
            entity.HasKey(e => e.ImageId)
                .HasName("PRIMARY");


            entity.ToTable("productimages");


            entity.Property(e => e.ImageId)
                .HasColumnName("ImageID");


            entity.Property(e => e.ProductId)
                .HasColumnName("ProductID");



            entity.HasOne(e => e.Product)
                .WithMany(p => p.Productimages)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("productimages_ibfk_1");
        });



        modelBuilder.Entity<Productreview>(entity =>
        {
            entity.HasKey(e => e.ReviewId)
                .HasName("PRIMARY");


            entity.ToTable("productreviews");


            entity.Property(e => e.ReviewId)
                .HasColumnName("ReviewID");


            entity.Property(e => e.ProductId)
                .HasColumnName("ProductID");



            entity.HasOne(e => e.Product)
                .WithMany(p => p.Productreviews)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("productreviews_ibfk_1");
        });



        modelBuilder.Entity<Productspecification>(entity =>
        {
            entity.HasKey(e => e.SpecificationId)
                .HasName("PRIMARY");


            entity.ToTable("productspecifications");


            entity.Property(e => e.SpecificationId)
                .HasColumnName("SpecificationID");


            entity.Property(e => e.ProductId)
                .HasColumnName("ProductID");



            entity.HasOne(e => e.Product)
                .WithMany(p => p.Productspecifications)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("productspecifications_ibfk_1");
        });



        OnModelCreatingPartial(modelBuilder);

    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}