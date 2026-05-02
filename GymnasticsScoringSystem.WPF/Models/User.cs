using System;
using System.Security.Cryptography;
using System.Text;

namespace GymnasticsScoringSystem.WPF.Models
{
    public class User
    {
        // Важно: set должен быть public для JSON сериализации
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }

        public enum UserRole
        {
            Admin,
            Judge
        }

        // Пустой конструктор нужен для JSON
        public User() { }

        public User(int id, string username, string password, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Имя пользователя не может быть пустым", "username");
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Пароль не может быть пустым", "password");

            Id = id;
            Username = username;
            PasswordHash = HashPassword(password);
            Role = role;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public bool ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;
            return PasswordHash == HashPassword(password);
        }

        public void ChangePassword(string oldPassword, string newPassword)
        {
            if (!ValidatePassword(oldPassword))
                throw new InvalidOperationException("Неверный текущий пароль");
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                throw new ArgumentException("Пароль должен содержать не менее 6 символов", "newPassword");
            PasswordHash = HashPassword(newPassword);
        }

        public void ChangeRole(UserRole newRole)
        {
            Role = newRole;
        }
    }
}