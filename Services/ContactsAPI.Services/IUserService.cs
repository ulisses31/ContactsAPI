using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContactsAPI.Data;
using ContactsAPI.Data.Model;

namespace ContactsAPI.Services
{
    public interface IUserService : IBaseService<User>
    {
        public User Authenticate(string username, string password);

        public Task<bool> UsernameAlreadyExists(string userName);

        public Task<int?> GetContactId(int userId);

    }
}
