namespace GitHubAnalyzer.Models
{
    public class ContributorComparison
    {
        // Productivity weights
        private const double COMMITS_WEIGHT = 0.30;
        private const double ADDITIONS_WEIGHT = 0.35;
        private const double DELETIONS_WEIGHT = 0.35;

        public string ContributorName { get; set; } = "";
        
        // First period stats
        public int FirstPeriodCommits { get; set; }
        public int FirstPeriodAdditions { get; set; }
        public int FirstPeriodDeletions { get; set; }
        public double FirstPeriodQualityScore { get; set; }

        // Second period stats
        public int SecondPeriodCommits { get; set; }
        public int SecondPeriodAdditions { get; set; }
        public int SecondPeriodDeletions { get; set; }
        public double SecondPeriodQualityScore { get; set; }

        // Differences
        public int CommitsDifference { get; set; }
        public int AdditionsDifference { get; set; }
        public int DeletionsDifference { get; set; }
        public double QualityScoreDifference { get; set; }

        // Percentage changes
        public double CommitsChangePercentage => FirstPeriodCommits > 0 
            ? ((double)CommitsDifference / FirstPeriodCommits) * 100 
            : 0;

        public double AdditionsChangePercentage => FirstPeriodAdditions > 0 
            ? ((double)AdditionsDifference / FirstPeriodAdditions) * 100 
            : 0;

        public double DeletionsChangePercentage => FirstPeriodDeletions > 0 
            ? ((double)DeletionsDifference / FirstPeriodDeletions) * 100 
            : 0;

        // Weighted productivity calculation
        public double WeightedProductivityChange => 
            (CommitsChangePercentage * COMMITS_WEIGHT) +
            (AdditionsChangePercentage * ADDITIONS_WEIGHT) +
            (DeletionsChangePercentage * DELETIONS_WEIGHT);

        // Detailed weighted components
        public double WeightedCommitsChange => CommitsChangePercentage * COMMITS_WEIGHT;
        public double WeightedAdditionsChange => AdditionsChangePercentage * ADDITIONS_WEIGHT;
        public double WeightedDeletionsChange => DeletionsChangePercentage * DELETIONS_WEIGHT;

        public void CalculateDifferences()
        {
            CommitsDifference = SecondPeriodCommits - FirstPeriodCommits;
            AdditionsDifference = SecondPeriodAdditions - FirstPeriodAdditions;
            DeletionsDifference = SecondPeriodDeletions - FirstPeriodDeletions;
            QualityScoreDifference = SecondPeriodQualityScore - FirstPeriodQualityScore;
        }
    }
} 