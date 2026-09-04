using System;
using System.Collections.Generic;

namespace Ecommerce.Services
{
    public class UserLoginLogItem
    {
        public string LogId { get; set; } = Guid.NewGuid().ToString();
        public int UserId { get; set; }
        public int? CustomerId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserRole { get; set; } = "Customer";
        public DateTime LoginTime { get; set; } = DateTime.Now;
        public string AccountStatus { get; set; } = "Active";
        public string LoginStatus { get; set; } = "Success";
        public bool IsReadByAdmin { get; set; } = false;
    }

    public interface IUserLoginLogger
    {
        void RecordLogin(int userId, int? customerId, string username, string email, string userRole, string accountStatus = "Active", string loginStatus = "Success");
        List<UserLoginLogItem> GetRecentLogins(int limit = 50);
        List<UserLoginLogItem> GetUnreadCustomerLogins();
        void MarkAsRead(string logId);
    }
}
