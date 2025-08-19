using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace UserManagementSystem.Services
{
    public class User
    {
        public string Username { get; set; }
        public string HashedPassword { get; set; }
    }

    public class UserService
    {
        private List<User> users = new List<User>();

        public bool Register(string username, string password)
        {
            if (users.Any(u => u.Username == username)) return false;

            string hashedPassword = HashPassword(password);
            users.Add(new User { Username = username, HashedPassword = hashedPassword });
            return true;
        }

        public bool Login(string username, string password)
        {
            string hashedPassword = HashPassword(password);
            return users.Any(u => u.Username == username && u.HashedPassword == hashedPassword);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
