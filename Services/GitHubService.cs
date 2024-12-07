using Octokit;
using GitHubAnalyzer.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace GitHubAnalyzer.Services
{
    public class GitHubService
    {
        private readonly GitHubClient _client;
        private readonly CodeQualityAnalyzer? _qualityAnalyzer;

        public GitHubService(string token, IChatCompletionService? chatService = null, Kernel? kernel = null)
        {
            _client = new GitHubClient(new ProductHeaderValue("repo-analyzer"))
            {
                Credentials = new Credentials(token)
            };
            
            if (chatService != null && kernel != null)
            {
                _qualityAnalyzer = new CodeQualityAnalyzer(chatService, kernel);
            }
        }

        public async Task<Dictionary<string, ContributorComparison>> CompareTimeRanges(
            string owner,
            string repo,
            DateTime firstPeriodStart,
            DateTime firstPeriodEnd,
            DateTime secondPeriodStart,
            DateTime secondPeriodEnd,
            AnalysisType analysisType)
        {
            var firstPeriodStats = await GetContributorStats(owner, repo, firstPeriodStart, firstPeriodEnd, analysisType);
            var secondPeriodStats = await GetContributorStats(owner, repo, secondPeriodStart, secondPeriodEnd, analysisType);

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
            DateTime endDate,
            AnalysisType analysisType)
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

                if (analysisType is AnalysisType.Qualitative or AnalysisType.Both)
                {
                    if (_qualityAnalyzer == null)
                    {
                        throw new InvalidOperationException("Quality analyzer is not initialized. Make sure to provide chat service and kernel when initializing GitHubService for qualitative analysis.");
                    }

                    var commitDiff = await GetCommitDiff(owner, repo, commit.Sha);
                    var qualityScore = await _qualityAnalyzer.AnalyzeCodeQuality(commitDiff);
                    contributorStats[authorName].AddCommitStats(
                        stats.Stats.Additions,
                        stats.Stats.Deletions,
                        qualityScore);
                }
                else
                {
                    contributorStats[authorName].AddCommitStats(
                        stats.Stats.Additions,
                        stats.Stats.Deletions,
                        0); // Default quality score for quantitative analysis
                }
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

        private async Task<string> GetCommitDiff(string owner, string repo, string sha)
        {
            var commit = await _client.Repository.Commit.Get(owner, repo, sha);
            return commit.Commit.Message + "\n" + string.Join("\n", 
                commit.Files.Select(f => $"File: {f.Filename}\n{f.Patch}"));
        }
    }
} 