using Octokit;
using System.Text.RegularExpressions;

namespace GitHubAnalyzer
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("GitHub Repository Analysis Tool");
            
            // Get user inputs
            Console.Write("Enter GitHub repository owner (e.g., 'microsoft'): ");
            string owner = Console.ReadLine() ?? throw new InvalidOperationException("Owner cannot be null");
            
            Console.Write("Enter repository name (e.g., 'vscode'): ");
            string repo = Console.ReadLine() ?? throw new InvalidOperationException("Repository cannot be null");
            
            Console.Write("Enter start date (YYYY-MM-DD): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
                throw new InvalidOperationException("Invalid start date format");
                
            Console.Write("Enter end date (YYYY-MM-DD): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
                throw new InvalidOperationException("Invalid end date format");

            Console.Write("Enter your GitHub personal access token: ");
            string token = Console.ReadLine() ?? throw new InvalidOperationException("Token cannot be null");

            // Initialize GitHub client
            var github = new GitHubClient(new ProductHeaderValue("repo-analyzer"))
            {
                Credentials = new Credentials(token)
            };

            try
            {
                // Get repository
                var repository = await github.Repository.Get(owner, repo);
                
                // Get all commits in the date range
                var commits = await GetCommitsInDateRange(github, owner, repo, startDate, endDate);
                
                // Group commits by author and calculate statistics
                var contributorStats = new Dictionary<string, (int commits, int additions, int deletions)>();
                
                foreach (var commit in commits)
                {
                    var stats = await github.Repository.Commit.Get(owner, repo, commit.Sha);
                    var authorName = stats.Commit.Author.Name;
                    
                    if (!contributorStats.ContainsKey(authorName))
                    {
                        contributorStats[authorName] = (0, 0, 0);
                    }
                    
                    var current = contributorStats[authorName];
                    contributorStats[authorName] = (
                        current.commits + 1,
                        current.additions + stats.Stats.Additions,
                        current.deletions + stats.Stats.Deletions
                    );
                }

                // Display results
                Console.WriteLine("\nContribution Statistics:");
                Console.WriteLine("------------------------");
                foreach (var stat in contributorStats)
                {
                    Console.WriteLine($"\nContributor: {stat.Key}");
                    Console.WriteLine($"Total Commits: {stat.Value.commits}");
                    Console.WriteLine($"Lines Added: {stat.Value.additions}");
                    Console.WriteLine($"Lines Deleted: {stat.Value.deletions}");
                    Console.WriteLine($"Net Lines of Code: {stat.Value.additions - stat.Value.deletions}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static async Task<IEnumerable<GitHubCommit>> GetCommitsInDateRange(
            GitHubClient github,
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

            commits.AddRange(await github.Repository.Commit.GetAll(owner, repo, commitRequest));
            return commits;
        }
    }
}
