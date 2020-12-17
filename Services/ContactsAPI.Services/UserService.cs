using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContactsAPI.Data;
using ContactsAPI.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace ContactsAPI.Services
{
    public class UserService : BaseService<User>, IUserService
    {
        public UserService(ContactsApiDbContext contactsApiDbContext) : base(contactsApiDbContext) { }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var userQuery = _contactsApiDbContext.Set<User>().AsQueryable();
            var user = userQuery.Where(x => x.Username == username).FirstOrDefault();
            if (user == null)
            {
                Debug.WriteLine(String.Format("User with username {0} not found ", username));
                return null;
            }

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }


        // private helper methods

        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }


        public async Task<bool> UsernameAlreadyExists(string userName)
        {
            var userQuery = _contactsApiDbContext.Set<User>().AsQueryable();
            var user = await userQuery.FirstOrDefaultAsync(x => x.Username == userName);
            return (user != null);
        }

        public async Task<int?> GetContactId(int userId)
        {
            var query = _contactsApiDbContext.Set<User>().AsQueryable();
            var user = await query.FirstOrDefaultAsync(x => x.Id == userId);
            if (user != null)
                return user.ContactId;
            else
                return null;
        }
    }
}
