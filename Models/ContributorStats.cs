namespace GitHubAnalyzer.Models
{
    public class ContributorStats
    {
        public string Name { get; set; }
        public int Commits { get; set; }
        public int Additions { get; set; }
        public int Deletions { get; set; }
        public int NetLinesOfCode => Additions - Deletions;

        public ContributorStats(string name)
        {
            Name = name;
            Commits = 0;
            Additions = 0;
            Deletions = 0;
        }

        public void AddCommitStats(int additions, int deletions)
        {
            Commits++;
            Additions += additions;
            Deletions += deletions;
        }

        public ContributorComparison CompareWith(ContributorStats other)
        {
            return new ContributorComparison
            {
                Name = Name,
                CommitsChange = CalculatePercentageChange(Commits, other.Commits),
                AdditionsChange = CalculatePercentageChange(Additions, other.Additions),
                DeletionsChange = CalculatePercentageChange(Deletions, other.Deletions),
                NetLinesChange = CalculatePercentageChange(NetLinesOfCode, other.NetLinesOfCode)
            };
        }

        private static double CalculatePercentageChange(int current, int previous)
        {
            if (previous == 0)
                return current == 0 ? 0 : 100;

            return ((double)(current - previous) / previous) * 100;
        }
    }

    public class ContributorComparison
    {
        public string Name { get; set; }
        public double CommitsChange { get; set; }
        public double AdditionsChange { get; set; }
        public double DeletionsChange { get; set; }
        public double NetLinesChange { get; set; }
    }
} 