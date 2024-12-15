using GitHubAnalyzer.Models;
using System.Text.Json;

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

        public UserInput GetUserInput(string[]? args = null)
        {
            AnalysisConfig? config = null;
            if (args != null && args.Length > 0)
            {
                string configPath = args[0];
                if (File.Exists(configPath))
                {
                    string jsonContent = File.ReadAllText(configPath);
                    config = JsonSerializer.Deserialize<AnalysisConfig>(jsonContent);
                    if (config != null)
                    {
                        Console.WriteLine("Using configuration from file:");
                        Console.WriteLine($"Repository: {config.Owner}/{config.Repository}");
                        Console.WriteLine($"Analysis Mode: {config.AnalysisMode}");
                        Console.WriteLine($"Analysis Type: {config.AnalysisType}");
                        Console.WriteLine($"First Period: {config.FirstPeriod.Start:yyyy-MM-dd} to {config.FirstPeriod.End:yyyy-MM-dd}");
                        if (config.SecondPeriod != null)
                        {
                            Console.WriteLine($"Second Period: {config.SecondPeriod.Start:yyyy-MM-dd} to {config.SecondPeriod.End:yyyy-MM-dd}");
                        }
                        Console.WriteLine(); // Empty line for readability

                        var mode = Enum.Parse<AnalysisMode>(config.AnalysisMode);
                        var analysisType = Enum.Parse<AnalysisType>(config.AnalysisType);
                        
                        return new UserInput(
                            mode,
                            analysisType,
                            config.Owner,
                            config.Repository,
                            config.FirstPeriod.Start,
                            config.FirstPeriod.End,
                            config.SecondPeriod?.Start,
                            config.SecondPeriod?.End,
                            config.GitHubToken ?? throw new InvalidOperationException("GitHub token is required in config file")
                        );
                    }
                }
            }

            // If no config file or invalid config, fall back to interactive mode
            Console.WriteLine("GitHub Repository Analysis Tool");
            
            Console.Write("Enter GitHub repository owner (e.g., 'microsoft'): ");
            string owner = Console.ReadLine() ?? throw new InvalidOperationException("Owner cannot be null");
            
            Console.Write("Enter repository name (e.g., 'vscode'): ");
            string repo = Console.ReadLine() ?? throw new InvalidOperationException("Repository cannot be null");
            
            Console.WriteLine("\nChoose analysis mode:");
            Console.WriteLine("1. Single period analysis");
            Console.WriteLine("2. Compare two periods");
            Console.Write("Enter your choice (1 or 2): ");
            
            var selectedMode = Console.ReadLine()?.Trim() switch
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
            
            var selectedAnalysisType = Console.ReadLine()?.Trim() switch
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
            if (selectedMode == AnalysisMode.Comparison)
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

            return new UserInput(selectedMode, selectedAnalysisType, owner, repo, firstStart, firstEnd, secondStart, secondEnd, token);
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

                // Commits change
                Console.WriteLine($"Commits: {comparison.FirstPeriodCommits} → {comparison.SecondPeriodCommits} " +
                    $"({comparison.CommitsDifference:+#;-#;0}) " +
                    $"[{comparison.CommitsChangePercentage:+0.0;-0.0;0.0}%]");

                // Lines added change
                Console.WriteLine($"Lines Added: {comparison.FirstPeriodAdditions} → {comparison.SecondPeriodAdditions} " +
                    $"({comparison.AdditionsDifference:+#;-#;0}) " +
                    $"[{comparison.AdditionsChangePercentage:+0.0;-0.0;0.0}%]");

                // Lines deleted change
                Console.WriteLine($"Lines Deleted: {comparison.FirstPeriodDeletions} → {comparison.SecondPeriodDeletions} " +
                    $"({comparison.DeletionsDifference:+#;-#;0}) " +
                    $"[{comparison.DeletionsChangePercentage:+0.0;-0.0;0.0}%]");
                
                if (comparison.QualityScoreDifference != 0)
                {
                    var qualityPercentage = comparison.FirstPeriodQualityScore > 0 
                        ? ((comparison.QualityScoreDifference / comparison.FirstPeriodQualityScore) * 100)
                        : 0;
                    Console.WriteLine($"Code Quality Score: {comparison.FirstPeriodQualityScore:F2} → {comparison.SecondPeriodQualityScore:F2} " +
                        $"({comparison.QualityScoreDifference:+0.00;-0.00;0.00}) " +
                        $"[{qualityPercentage:+0.0;-0.0;0.0}%]");
                }

                // Weighted Productivity Analysis
                Console.WriteLine("\nWeighted Productivity Analysis:");
                Console.WriteLine($"Commits Impact (30%): {comparison.WeightedCommitsChange:+0.00;-0.00;0.00}%");
                Console.WriteLine($"Additions Impact (35%): {comparison.WeightedAdditionsChange:+0.00;-0.00;0.00}%");
                Console.WriteLine($"Deletions Impact (35%): {comparison.WeightedDeletionsChange:+0.00;-0.00;0.00}%");
                Console.WriteLine($"Overall Productivity Change: {comparison.WeightedProductivityChange:+0.00;-0.00;0.00}%");
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