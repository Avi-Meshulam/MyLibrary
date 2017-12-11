using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.BL
{
    public static class Permissions
    {
        public static OperationType GetUserPermissions<T>(User user) where T : Entity<T>
        {
            if(user == null || !user.IsLoggedIn)
                return OperationType.None;

            if (typeof(T) == typeof(LibraryItem))
                switch (user.UserType)
                {
                    case UserType.Worker:
                        return OperationType.View;
                    case UserType.Supervisor:
                    case UserType.Manager:
                        return
                            OperationType.View | 
                            OperationType.Add | 
                            OperationType.Edit | 
                            OperationType.Delete;
                    default:
                        return OperationType.None;
                }

            if (typeof(T) == typeof(Employee))
                switch (user.UserType)
                {
                    case UserType.Worker:
                    case UserType.Supervisor:
                        return OperationType.View;
                    case UserType.Manager:
                        return
                            OperationType.View |
                            OperationType.Add |
                            OperationType.Edit |
                            OperationType.Delete;
                    default:
                        return OperationType.None;
                }

            if (typeof(T) == typeof(User))
                switch (user.UserType)
                {
                    case UserType.Worker:
                    case UserType.Supervisor:
                        return OperationType.View;
                    case UserType.Manager:
                        return
                            OperationType.View |
                            OperationType.Add |
                            OperationType.Edit |
                            OperationType.Delete;
                    default:
                        return OperationType.None;
                }

            if (typeof(T) == typeof(Publisher))
                switch (user.UserType)
                {
                    case UserType.Worker:
                        return OperationType.View;
                    case UserType.Supervisor:
                    case UserType.Manager:
                        return
                            OperationType.View |
                            OperationType.Add |
                            OperationType.Edit |
                            OperationType.Delete;
                    default:
                        return OperationType.None;
                }

            return OperationType.None;
        }
    }
}
