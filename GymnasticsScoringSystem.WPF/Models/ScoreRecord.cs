using System;
using System.Collections.Generic;

namespace GymnasticsScoringSystem.WPF.Models
{
    public class ScoreRecord
    {
        public int PerformanceId { get; set; }
        public int BrigadeId { get; set; }
        public List<double> Scores { get; set; }
        public double AverageScore { get; set; }
        public double FinalScore { get; set; }
        public bool IsConsensusReached { get; set; }
        public double Deviation { get; set; }
        public DateTime SavedAt { get; set; }

        public ScoreRecord()
        {
            Scores = new List<double>();
        }
    }
}