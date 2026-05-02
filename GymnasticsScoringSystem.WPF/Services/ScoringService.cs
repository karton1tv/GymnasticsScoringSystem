using System;
using System.Collections.Generic;
using System.Linq;
using GymnasticsScoringSystem.WPF.Models;

namespace GymnasticsScoringSystem.WPF.Services
{
    public class ScoringService
    {
        private readonly FileRepository _repository;

        public ScoringService(FileRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            _repository = repository;
        }

        public ScoringResult CalculateAndSaveScores(int performanceId, int brigadeId,
            List<double> scores, double maxAllowedDeviation, double penalty)
        {
            if (scores == null || scores.Count == 0)
                throw new ArgumentException("Список оценок не может быть пустым", "scores");
            foreach (double score in scores)
            {
                if (score < 0.0 || score > 10.0)
                    throw new ArgumentException("Оценки должны быть в диапазоне 0.00–10.00");
            }
            if (penalty < 0.0)
                throw new ArgumentException("Штраф не может быть отрицательным", "penalty");

            double maxScore = scores.Max();
            double minScore = scores.Min();
            double deviation = Math.Round(maxScore - minScore, 2);
            double averageScore = Math.Round(scores.Average(), 2);
            bool isConsensusReached = deviation <= maxAllowedDeviation;
            double finalScore = Math.Max(0.0, Math.Round(averageScore - penalty, 2));

            ScoreRecord record = new ScoreRecord
            {
                PerformanceId = performanceId,
                BrigadeId = brigadeId,
                Scores = new List<double>(scores),
                AverageScore = averageScore,
                FinalScore = finalScore,
                IsConsensusReached = isConsensusReached,
                SavedAt = DateTime.UtcNow
            };

            List<ScoreRecord> allRecords = _repository.LoadScores();
            allRecords.Add(record);
            _repository.SaveScores(allRecords);

            return new ScoringResult
            {
                AverageScore = averageScore,
                FinalScore = finalScore,
                IsConsensusReached = isConsensusReached,
                Deviation = deviation
            };
        }

        public double GetTotalScore(int performanceId, int brigadeCount)
        {
            List<ScoreRecord> records = _repository.LoadScores()
                .Where(r => r.PerformanceId == performanceId)
                .ToList();

            if (records.Count != brigadeCount)
                throw new InvalidOperationException(string.Format("Ожидается {0} бригад, получено {1}", brigadeCount, records.Count));

            double total = records.Sum(r => r.FinalScore);
            double maxPossible = brigadeCount * 10.0;
            return Math.Min(Math.Round(total, 2), maxPossible);
        }
    }
}