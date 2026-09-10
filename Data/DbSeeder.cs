using System;
using System.Collections.Generic;
using System.Linq;
using Ecommerce.Models;
using Ecommerce.Services;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Data
{
    public static class DbSeeder
    {
        public static void SeedAll(ApplicationDbContext context)
        {
            try
            {
                // 1. Seed Roles
                SeedRoles(context);

                // 2. Seed Default Portal Users (Admin, Seller, Warehouse, Delivery, Customer)
                SeedDefaultUsers(context);

                // 3. Seed Product Catalog & Categories
                ProductSeeder.SeedProducts(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB SEED ERROR] {ex.Message}");
            }
        }

        private static void SeedRoles(ApplicationDbContext context)
        {
            if (!context.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new Role { RoleId = 1, RoleName = "Admin" },
                    new Role { RoleId = 2, RoleName = "Seller" },
                    new Role { RoleId = 3, RoleName = "Warehouse" },
                    new Role { RoleId = 4, RoleName = "Delivery" },
                    new Role { RoleId = 5, RoleName = "Customer" }
                };

                context.Roles.AddRange(roles);
                context.SaveChanges();
            }
        }

        private static void SeedDefaultUsers(ApplicationDbContext context)
        {
            // Seed Admin User
            if (!context.Users.Any(u => u.Email == "admin@nexstore.com" || u.RoleId == 1))
            {
                var adminUser = new User
                {
                    Username = "Alex Vance (Super Admin)",
                    Email = "admin@nexstore.com",
                    PasswordHash = PasswordHasher.Hash("Admin@123!"),
                    PhoneNumber = "9900000001",
                    RoleId = 1,
                    AccountStatus = "Active",
                    CreatedDate = DateTime.Now
                };
                context.Users.Add(adminUser);
                context.SaveChanges();
            }

            // Seed Seller User
            if (!context.Users.Any(u => u.Email == "seller@webkadai.com" || u.RoleId == 2))
            {
                var sellerUser = new User
                {
                    Username = "Zenith Retail Merchant",
                    Email = "seller@webkadai.com",
                    PasswordHash = PasswordHasher.Hash("Seller@123!"),
                    PhoneNumber = "9900000002",
                    RoleId = 2,
                    AccountStatus = "Active",
                    CreatedDate = DateTime.Now
                };
                context.Users.Add(sellerUser);
                context.SaveChanges();

                if (!context.Sellers.Any(s => s.UserId == sellerUser.UserId))
                {
                    var seller = new Seller
                    {
                        UserId = sellerUser.UserId,
                        BusinessName = "Zenith Retail Merchant Pvt Ltd",
                        Gstnumber = "29AAAAA0000A1Z5",
                        ApprovalStatus = "Approved",
                        Status = "Active",
                        ComplaintCount = 0,
                        CreatedDate = DateTime.Now
                    };
                    context.Sellers.Add(seller);
                    context.SaveChanges();
                }
            }

            // Seed Warehouse User
            if (!context.Users.Any(u => u.Email == "warehouse@webkadai.com" || u.RoleId == 3))
            {
                var warehouseUser = new User
                {
                    Username = "Kiran Kumar (Hub #01)",
                    Email = "warehouse@webkadai.com",
                    PasswordHash = PasswordHasher.Hash("Warehouse@123!"),
                    PhoneNumber = "9900000003",
                    RoleId = 3,
                    AccountStatus = "Active",
                    CreatedDate = DateTime.Now
                };
                context.Users.Add(warehouseUser);
                context.SaveChanges();

                if (!context.Warehousemanagers.Any(w => w.UserId == warehouseUser.UserId))
                {
                    context.Warehousemanagers.Add(new Warehousemanager
                    {
                        UserId = warehouseUser.UserId,
                        WarehouseId = 1
                    });
                    context.SaveChanges();
                }
            }

            // Seed Delivery User
            if (!context.Users.Any(u => u.Email == "delivery@webkadai.com" || u.RoleId == 4))
            {
                var deliveryUser = new User
                {
                    Username = "Vikram Rathore (Rider)",
                    Email = "delivery@webkadai.com",
                    PasswordHash = PasswordHasher.Hash("Delivery@123!"),
                    PhoneNumber = "9900000004",
                    RoleId = 4,
                    AccountStatus = "Active",
                    CreatedDate = DateTime.Now
                };
                context.Users.Add(deliveryUser);
                context.SaveChanges();

                if (!context.Deliverypartners.Any(d => d.UserId == deliveryUser.UserId))
                {
                    context.Deliverypartners.Add(new Deliverypartner
                    {
                        UserId = deliveryUser.UserId,
                        VehicleNumber = "KA-05-MB-4421",
                        AvailabilityStatus = "Online"
                    });
                    context.SaveChanges();
                }
            }

            // Seed Customer User
            if (!context.Users.Any(u => u.Email == "customer@webkadai.com" || u.RoleId == 5))
            {
                var customerUser = new User
                {
                    Username = "Rahul Sharma",
                    Email = "customer@webkadai.com",
                    PasswordHash = PasswordHasher.Hash("Customer@123!"),
                    PhoneNumber = "9900000005",
                    RoleId = 5,
                    AccountStatus = "Active",
                    CreatedDate = DateTime.Now
                };
                context.Users.Add(customerUser);
                context.SaveChanges();

                if (!context.Customers.Any(c => c.UserId == customerUser.UserId))
                {
                    var customer = new Customer
                    {
                        UserId = customerUser.UserId,
                        FirstName = "Rahul",
                        LastName = "Sharma"
                    };
                    context.Customers.Add(customer);
                    context.SaveChanges();

                    context.Carts.Add(new Cart
                    {
                        CustomerId = customer.CustomerId,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}
