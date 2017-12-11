using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.BL
{
    public class UsersManager : EntityManager<User>
    {
        private const string ADMIN_USER = "admin";
        private const string ADMIN_PASSWORD = "admin";

        private IReadOnlyList<User> _users;
        
        public UsersManager(User currentUser = null) : base(currentUser)
        {
            _users = Search();
        }

        public int LoginTryCount { get; private set; }

        public User Login(string userName, string password)
        {
            User user = default(User);

            if (_users.Count == 0)
            {
                if (userName.ToLower() == ADMIN_USER && password == ADMIN_PASSWORD)
                {
                    user = new User(0, UserType.Manager);
                    user.IsLoggedIn = true;
                }
            }
            else
            {
                user = _users.FirstOrDefault(u => 
                    u.UserName.ToLower() == userName.ToLower() && u.Password == password);
            }

            if(user == null)
                LoginTryCount++;
            else
            {
                user.IsLoggedIn = true;
                LoginTryCount = 0;
            }

            return user;
        }
    }
}
