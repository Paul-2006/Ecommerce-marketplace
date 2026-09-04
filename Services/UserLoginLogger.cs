using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Ecommerce.Services
{
    public class UserLoginLogger : IUserLoginLogger
    {
        private static readonly ConcurrentQueue<UserLoginLogItem> LogQueue = new();

        public void RecordLogin(int userId, int? customerId, string username, string email, string userRole, string accountStatus = "Active", string loginStatus = "Success")
        {
            var log = new UserLoginLogItem
            {
                UserId = userId,
                CustomerId = customerId,
                Username = string.IsNullOrWhiteSpace(username) ? "Marketplace User" : username,
                Email = email ?? "",
                UserRole = string.IsNullOrWhiteSpace(userRole) ? "Customer" : userRole,
                LoginTime = DateTime.Now,
                AccountStatus = string.IsNullOrWhiteSpace(accountStatus) ? "Active" : accountStatus,
                LoginStatus = string.IsNullOrWhiteSpace(loginStatus) ? "Success" : loginStatus,
                IsReadByAdmin = false
            };

            LogQueue.Enqueue(log);

            // Keep maximum 500 recent records in memory
            while (LogQueue.Count > 500)
            {
                LogQueue.TryDequeue(out _);
            }
        }

        public List<UserLoginLogItem> GetRecentLogins(int limit = 50)
        {
            return LogQueue
                .Reverse()
                .Take(limit)
                .ToList();
        }

        public List<UserLoginLogItem> GetUnreadCustomerLogins()
        {
            return LogQueue
                .Where(l => !l.IsReadByAdmin && l.UserRole.Equals("Customer", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(l => l.LoginTime)
                .ToList();
        }

        public void MarkAsRead(string logId)
        {
            if (string.IsNullOrWhiteSpace(logId)) return;
            var item = LogQueue.FirstOrDefault(l => l.LogId == logId);
            if (item != null)
            {
                item.IsReadByAdmin = true;
            }
        }
    }
}
