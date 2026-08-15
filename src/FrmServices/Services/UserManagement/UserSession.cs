using System;

namespace FrmServices.Services.UserManagement
{
    public static class UserSession
    {
        private static readonly object SyncRoot = new object();
        private static UserInfo _currentUser;

        public static event EventHandler CurrentUserChanged;

        public static UserInfo CurrentUser
        {
            get
            {
                lock (SyncRoot) return _currentUser;
            }
        }

        public static bool IsAdministrator
        {
            get => AccessLevel >= UserAccessLevel.Administrator;
        }

        public static bool CanConfigure =>
            AccessLevel >= UserAccessLevel.Engineer;

        public static bool CanApplyRecipe =>
            AccessLevel >= UserAccessLevel.Employee;

        public static UserAccessLevel AccessLevel
        {
            get
            {
                UserInfo user = CurrentUser;
                return user != null && user.IsEnabled
                    ? UserRoles.GetAccessLevel(user.Role)
                    : UserAccessLevel.None;
            }
        }

        public static void SignIn(UserInfo user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            lock (SyncRoot) _currentUser = user;
            CurrentUserChanged?.Invoke(null, EventArgs.Empty);
        }

        public static void Refresh(UserInfo user)
        {
            if (user == null) return;
            lock (SyncRoot)
            {
                if (_currentUser == null || _currentUser.Id != user.Id) return;
                _currentUser = user;
            }
            CurrentUserChanged?.Invoke(null, EventArgs.Empty);
        }

        public static void SignOut()
        {
            lock (SyncRoot) _currentUser = null;
            CurrentUserChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}
