using System;
using System.Collections.Generic;
using System.Linq;

namespace GitHubAnalyzer.Models
{
    public class ContributorStats
    {
        public string ContributorName { get; }
        public int TotalCommits { get; private set; }
        public int TotalAdditions { get; private set; }
        public int TotalDeletions { get; private set; }
        public double AverageCodeQuality { get; private set; }
        private List<int> CodeQualityScores { get; } = new List<int>();

        public ContributorStats(string name)
        {
            ContributorName = name;
        }

        public void AddCommitStats(int additions, int deletions, int qualityScore)
        {
            TotalCommits++;
            TotalAdditions += additions;
            TotalDeletions += deletions;
            CodeQualityScores.Add(qualityScore);
            AverageCodeQuality = CodeQualityScores.Average();
        }

        public ContributorComparison CompareWith(ContributorStats other)
        {
            return new ContributorComparison
            {
                ContributorName = ContributorName,
                CommitsDifference = TotalCommits - other.TotalCommits,
                AdditionsDifference = TotalAdditions - other.TotalAdditions,
                DeletionsDifference = TotalDeletions - other.TotalDeletions,
                QualityScoreDifference = AverageCodeQuality - other.AverageCodeQuality
            };
        }
    }
}