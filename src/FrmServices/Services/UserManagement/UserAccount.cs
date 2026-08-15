using System;
using SQLite;

namespace FrmServices.Services.UserManagement
{
    public enum UserAccessLevel
    {
        None = 0,
        Employee = 1,
        Engineer = 2,
        Administrator = 3
    }

    public static class UserRoles
    {
        public const string Administrator = "管理员";
        public const string Engineer = "工程师";
        public const string Employee = "员工";
        public const string LegacyOperator = "操作员";

        public static readonly string[] All =
        {
            Administrator,
            Engineer,
            Employee
        };

        public static string Normalize(string role)
        {
            return string.Equals(role, LegacyOperator, StringComparison.Ordinal)
                ? Employee
                : role;
        }

        public static UserAccessLevel GetAccessLevel(string role)
        {
            string normalizedRole = Normalize(role);
            if (string.Equals(normalizedRole, Administrator,
                    StringComparison.Ordinal))
                return UserAccessLevel.Administrator;
            if (string.Equals(normalizedRole, Engineer,
                    StringComparison.Ordinal))
                return UserAccessLevel.Engineer;
            if (string.Equals(normalizedRole, Employee,
                    StringComparison.Ordinal))
                return UserAccessLevel.Employee;
            return UserAccessLevel.None;
        }
    }

    [Table("Users")]
    public sealed class UserAccount
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed(Name = "IX_Users_UserName", Unique = true)]
        [MaxLength(32)]
        public string UserName { get; set; }

        [MaxLength(50)]
        public string DisplayName { get; set; }

        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }

        [MaxLength(20)]
        public string Role { get; set; }

        public bool IsEnabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    public sealed class UserInfo
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Role { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    public sealed class UserOperationException : Exception
    {
        public UserOperationException(string message) : base(message)
        {
        }
    }
}
