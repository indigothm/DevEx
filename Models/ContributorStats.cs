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
            var comparison = new ContributorComparison
            {
                ContributorName = ContributorName,
                FirstPeriodCommits = other.TotalCommits,
                FirstPeriodAdditions = other.TotalAdditions,
                FirstPeriodDeletions = other.TotalDeletions,
                FirstPeriodQualityScore = other.AverageCodeQuality,
                SecondPeriodCommits = TotalCommits,
                SecondPeriodAdditions = TotalAdditions,
                SecondPeriodDeletions = TotalDeletions,
                SecondPeriodQualityScore = AverageCodeQuality
            };
            
            comparison.CalculateDifferences();
            return comparison;
        }
    }
}