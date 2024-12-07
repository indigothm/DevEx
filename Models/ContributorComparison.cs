namespace GitHubAnalyzer.Models
{
    public class ContributorComparison
    {
        required public string ContributorName { get; set; }
        public int CommitsDifference { get; set; }
        public int AdditionsDifference { get; set; }
        public int DeletionsDifference { get; set; }
        public double QualityScoreDifference { get; set; }
    }
} 