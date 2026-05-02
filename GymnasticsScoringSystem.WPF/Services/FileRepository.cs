using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using GymnasticsScoringSystem.WPF.Models;
using System.Xml;

namespace GymnasticsScoringSystem.WPF.Services
{
    public class FileRepository
    {
        private readonly string _filePath;

        public FileRepository(string filePath)
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

        public void SaveScores(List<ScoreRecord> records)
        {
            string json = JsonConvert.SerializeObject(records, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public List<ScoreRecord> LoadScores()
        {
            string json = File.ReadAllText(_filePath);
            List<ScoreRecord> result = JsonConvert.DeserializeObject<List<ScoreRecord>>(json);
            return result ?? new List<ScoreRecord>();
        }

        public void Clear()
        {
            File.WriteAllText(_filePath, "[]");
        }
    }
}