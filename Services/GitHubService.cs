using Octokit;
using GitHubAnalyzer.Models;

namespace GitHubAnalyzer.Services
{
    public class GitHubService
    {
        private readonly GitHubClient _client;

        public GitHubService(string token)
        {
            _client = new GitHubClient(new ProductHeaderValue("repo-analyzer"))
            {
                Credentials = new Credentials(token)
            };
        }

        public async Task<Dictionary<string, ContributorComparison>> CompareTimeRanges(
            string owner,
            string repo,
            DateTime firstPeriodStart,
            DateTime firstPeriodEnd,
            DateTime secondPeriodStart,
            DateTime secondPeriodEnd)
        {
            var firstPeriodStats = await GetContributorStats(owner, repo, firstPeriodStart, firstPeriodEnd);
            var secondPeriodStats = await GetContributorStats(owner, repo, secondPeriodStart, secondPeriodEnd);

            var comparisons = new Dictionary<string, ContributorComparison>();
            var allContributors = firstPeriodStats.Keys.Union(secondPeriodStats.Keys);

            foreach (var contributor in allContributors)
            {
                var firstPeriod = firstPeriodStats.GetValueOrDefault(contributor) ?? new ContributorStats(contributor);
                var secondPeriod = secondPeriodStats.GetValueOrDefault(contributor) ?? new ContributorStats(contributor);

                comparisons[contributor] = secondPeriod.CompareWith(firstPeriod);
            }

            return comparisons;
        }

        public async Task<Dictionary<string, ContributorStats>> GetContributorStats(
            string owner,
            string repo,
            DateTime startDate,
            DateTime endDate)
        {
            var repository = await _client.Repository.Get(owner, repo);
            var commits = await GetCommitsInDateRange(owner, repo, startDate, endDate);
            var contributorStats = new Dictionary<string, ContributorStats>();

            foreach (var commit in commits)
            {
                var stats = await _client.Repository.Commit.Get(owner, repo, commit.Sha);
                var authorName = stats.Commit.Author.Name;

                if (!contributorStats.ContainsKey(authorName))
                {
                    contributorStats[authorName] = new ContributorStats(authorName);
                }

                contributorStats[authorName].AddCommitStats(
                    stats.Stats.Additions,
                    stats.Stats.Deletions
                );
            }

            return contributorStats;
        }

        private async Task<IEnumerable<GitHubCommit>> GetCommitsInDateRange(
            string owner,
            string repo,
            DateTime startDate,
            DateTime endDate)
        {
            var commits = new List<GitHubCommit>();
            var commitRequest = new CommitRequest
            {
                Since = startDate,
                Until = endDate
            };

            commits.AddRange(await _client.Repository.Commit.GetAll(owner, repo, commitRequest));
            return commits;
        }
    }
} 