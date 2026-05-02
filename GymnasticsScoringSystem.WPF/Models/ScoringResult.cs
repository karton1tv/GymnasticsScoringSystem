namespace GymnasticsScoringSystem.WPF.Models
{
    public class ScoringResult
    {
        public double AverageScore { get; set; }
        public double FinalScore { get; set; }
        public bool IsConsensusReached { get; set; }
        public double Deviation { get; set; }
    }
}