    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace VisionManagement.Models
    {
        public class Evaluation
        {
            [Key]
            public int EvaluationId { get; set; }

            [ForeignKey("Project")]
            public int ProjectId { get; set; }
            public Project Project { get; set; }

            [ForeignKey("User")]
            public int UserId { get; set; }
            public User User { get; set; }   // Evaluator

            // Scores
            [Range(1, 10)] public int ProblemSignificance { get; set; }
            [Range(1, 10)] public int InnovationTechnical { get; set; }
            [Range(1, 10)] public int MarketScalability { get; set; }
            [Range(1, 10)] public int TractionImpact { get; set; }
            [Range(1, 10)] public int BusinessModel { get; set; }
            [Range(1, 10)] public int TeamExecution { get; set; }
            [Range(1, 10)] public int EthicsEquity { get; set; }

            // Comments
            public string? Strengths { get; set; }
            public string? Weaknesses { get; set; }
            public string? Recommendation { get; set; }

            public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
        }
    }
