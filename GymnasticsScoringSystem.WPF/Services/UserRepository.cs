using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using GymnasticsScoringSystem.WPF.Models;

namespace GymnasticsScoringSystem.WPF.Services
{
    public class UserRepository
    {
        private readonly string _filePath;

        public UserRepository(string filePath)
        {
            _filePath = filePath;
            if (!File.Exists(_filePath))
            {
                string directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllText(_filePath, "[]");
            }
        }

        public List<User> LoadUsers()
        {
            string json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
        }

        public void SaveUsers(List<User> users)
        {
            string json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}