using System;
using GitHubAnalyzer.UI;

namespace GitHubAnalyzer.Models
{
    public class AnalysisConfig
    {
        public string Owner { get; set; } = "";
        public string Repository { get; set; } = "";
        public string AnalysisMode { get; set; } = nameof(ConsoleUI.AnalysisMode.SinglePeriod);
        public string AnalysisType { get; set; } = nameof(Models.AnalysisType.Quantitative);
        public DateRange FirstPeriod { get; set; } = new();
        public DateRange? SecondPeriod { get; set; }
        public string? GitHubToken { get; set; }
    }

    public class DateRange
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
} 