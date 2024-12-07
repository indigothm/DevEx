using GitHubAnalyzer.Models;

namespace GitHubAnalyzer.UI
{
    public class ConsoleUI
    {
        public enum AnalysisMode
        {
            SinglePeriod,
            Comparison
        }

        public record UserInput(
            AnalysisMode Mode,
            AnalysisType Type,
            string Owner,
            string Repo,
            DateTime FirstStart,
            DateTime FirstEnd,
            DateTime? SecondStart,
            DateTime? SecondEnd,
            string Token
        );

        public UserInput GetUserInput()
        {
            Console.WriteLine("GitHub Repository Analysis Tool");
            
            Console.Write("Enter GitHub repository owner (e.g., 'microsoft'): ");
            string owner = Console.ReadLine() ?? throw new InvalidOperationException("Owner cannot be null");
            
            Console.Write("Enter repository name (e.g., 'vscode'): ");
            string repo = Console.ReadLine() ?? throw new InvalidOperationException("Repository cannot be null");
            
            Console.WriteLine("\nChoose analysis mode:");
            Console.WriteLine("1. Single period analysis");
            Console.WriteLine("2. Compare two periods");
            Console.Write("Enter your choice (1 or 2): ");
            
            var mode = Console.ReadLine()?.Trim() switch
            {
                "1" => AnalysisMode.SinglePeriod,
                "2" => AnalysisMode.Comparison,
                _ => throw new InvalidOperationException("Invalid choice. Please enter 1 or 2.")
            };

            Console.WriteLine("\nChoose analysis type:");
            Console.WriteLine("1. Quantitative (commits, additions, deletions)");
            Console.WriteLine("2. Qualitative (AI-powered code quality analysis)");
            Console.WriteLine("3. Both");
            Console.Write("Enter your choice (1, 2 or 3): ");
            
            var analysisType = Console.ReadLine()?.Trim() switch
            {
                "1" => AnalysisType.Quantitative,
                "2" => AnalysisType.Qualitative,
                "3" => AnalysisType.Both,
                _ => throw new InvalidOperationException("Invalid choice. Please enter 1, 2 or 3.")
            };

            Console.WriteLine("\nFirst Time Period:");
            Console.Write("Enter start date (YYYY-MM-DD): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime firstStart))
                throw new InvalidOperationException("Invalid start date format");
                
            Console.Write("Enter end date (YYYY-MM-DD): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime firstEnd))
                throw new InvalidOperationException("Invalid end date format");

            DateTime? secondStart = null, secondEnd = null;
            if (mode == AnalysisMode.Comparison)
            {
                Console.WriteLine("\nSecond Time Period:");
                Console.Write("Enter start date (YYYY-MM-DD): ");
                if (!DateTime.TryParse(Console.ReadLine(), out var tempSecondStart))
                    throw new InvalidOperationException("Invalid start date format");
                    
                Console.Write("Enter end date (YYYY-MM-DD): ");
                if (!DateTime.TryParse(Console.ReadLine(), out var tempSecondEnd))
                    throw new InvalidOperationException("Invalid end date format");

                secondStart = tempSecondStart;
                secondEnd = tempSecondEnd;
            }

            Console.Write("\nEnter your GitHub personal access token: ");
            string token = Console.ReadLine() ?? throw new InvalidOperationException("Token cannot be null");

            return new UserInput(mode, analysisType, owner, repo, firstStart, firstEnd, secondStart, secondEnd, token);
        }

        public void DisplayResults(Dictionary<string, ContributorStats> stats)
        {
            Console.WriteLine("\nContribution Statistics:");
            Console.WriteLine("------------------------");
            
            foreach (var stat in stats.Values)
            {
                Console.WriteLine($"\nContributor: {stat.ContributorName}");
                Console.WriteLine($"Total Commits: {stat.TotalCommits}");
                Console.WriteLine($"Lines Added: {stat.TotalAdditions}");
                Console.WriteLine($"Lines Deleted: {stat.TotalDeletions}");
                
                if (stat.AverageCodeQuality > 0)
                {
                    Console.WriteLine($"Average Code Quality Score: {stat.AverageCodeQuality:F2}/5.00");
                }
            }
        }

        public void DisplayComparison(Dictionary<string, ContributorComparison> comparisons)
        {
            Console.WriteLine("\nContribution Comparisons:");
            Console.WriteLine("-------------------------");
            
            foreach (var comparison in comparisons.Values)
            {
                Console.WriteLine($"\nContributor: {comparison.ContributorName}");
                Console.WriteLine($"Commits Change: {comparison.CommitsDifference:+#;-#;0}");
                Console.WriteLine($"Lines Added Change: {comparison.AdditionsDifference:+#;-#;0}");
                Console.WriteLine($"Lines Deleted Change: {comparison.DeletionsDifference:+#;-#;0}");
                
                if (comparison.QualityScoreDifference != 0)
                {
                    Console.WriteLine($"Code Quality Score Change: {comparison.QualityScoreDifference:+0.00;-0.00;0.00}");
                }
            }
        }

        public void DisplayError(Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Details: {ex.InnerException.Message}");
            }
        }
    }
} 