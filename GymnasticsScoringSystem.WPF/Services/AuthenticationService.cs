using System;
using System.Collections.Generic;
using System.Linq;
using GymnasticsScoringSystem.WPF.Models;

namespace GymnasticsScoringSystem.WPF.Services
{
    public class AuthenticationService
    {
        private List<User> _usersList = new List<User>();
        private Dictionary<string, User> _usersDict = new Dictionary<string, User>();
        private UserRepository _repository;
        private int _nextUserId = 1;

        // Конструктор теперь принимает путь к файлу пользователей
        public AuthenticationService(string usersFilePath)
        {
            _repository = new UserRepository(usersFilePath);
            _usersList = _repository.LoadUsers();

            // Заполняем словарь из списка
            foreach (var u in _usersList)
            {
                _usersDict[u.Username.ToLowerInvariant()] = u;
                if (u.Id >= _nextUserId) _nextUserId = u.Id + 1;
            }
        }

        public User RegisterUser(string username, string password, User.UserRole role)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Имя пользователя не может быть пустым", "username");
            if (_usersDict.ContainsKey(username.ToLowerInvariant()))
                throw new InvalidOperationException(string.Format("Пользователь '{0}' уже существует", username));

            User user = new User(_nextUserId++, username, password, role);

            // Сохраняем в оба хранилища
            _usersList.Add(user);
            _usersDict[username.ToLowerInvariant()] = user;

            // ВАЖНО: Сохраняем на диск сразу!
            _repository.SaveUsers(_usersList);

            return user;
        }

        public User Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Имя пользователя и пароль обязательны");
            if (!_usersDict.TryGetValue(username.ToLowerInvariant(), out User user))
                throw new InvalidOperationException("Неверное имя пользователя или пароль");
            if (!user.ValidatePassword(password))
                throw new InvalidOperationException("Неверное имя пользователя или пароль");
            return user;
        }

        public bool HasAccess(User user, OperationType operation)
        {
            switch (operation)
            {
                case OperationType.ManageVoting:
                case OperationType.CreateJudge:
                    return user.Role == User.UserRole.Admin;
                case OperationType.EnterScore:
                    return user.Role == User.UserRole.Judge;
                default:
                    return false;
            }
        }

        public enum OperationType
        {
            ManageVoting,
            CreateJudge,
            EnterScore
        }

        // Для тестирования: получить всех пользователей
        public List<User> GetAllUsers()
        {
            return new List<User>(_usersDict.Values);
        }
        /// <summary>
        /// Проверяет надёжность пароля
        /// Требование: длина ≥ 8 символов И содержит хотя бы одну цифру
        /// </summary>
        public bool IsPasswordStrong(string password)
        {
            // Защита от null/empty
            if (string.IsNullOrEmpty(password))
                return false;

            // Проверка длины
            if (password.Length < 8)
                return false;

            // Проверка наличия цифры (цикл для C# 7.3)
            for (int i = 0; i < password.Length; i++)
            {
                if (char.IsDigit(password[i]))
                    return true;
            }

            return false;
        }

    }
}