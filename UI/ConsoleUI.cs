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

        public (AnalysisMode mode, string owner, string repo, 
                DateTime firstStart, DateTime firstEnd, 
                DateTime? secondStart, DateTime? secondEnd, 
                string token) GetUserInput()
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

            return (mode, owner, repo, firstStart, firstEnd, secondStart, secondEnd, token);
        }

        public void DisplayResults(Dictionary<string, ContributorStats> stats)
        {
            Console.WriteLine("\nContribution Statistics:");
            Console.WriteLine("------------------------");
            
            foreach (var stat in stats.Values)
            {
                Console.WriteLine($"\nContributor: {stat.Name}");
                Console.WriteLine($"Total Commits: {stat.Commits}");
                Console.WriteLine($"Lines Added: {stat.Additions}");
                Console.WriteLine($"Lines Deleted: {stat.Deletions}");
                Console.WriteLine($"Net Lines of Code: {stat.NetLinesOfCode}");
            }
        }

        public void DisplayComparison(Dictionary<string, ContributorComparison> comparisons)
        {
            Console.WriteLine("\nContribution Comparisons (% change from first to second period):");
            Console.WriteLine("--------------------------------------------------------");
            
            foreach (var comparison in comparisons.Values)
            {
                Console.WriteLine($"\nContributor: {comparison.Name}");
                Console.WriteLine($"Commits Change: {comparison.CommitsChange:F1}%");
                Console.WriteLine($"Lines Added Change: {comparison.AdditionsChange:F1}%");
                Console.WriteLine($"Lines Deleted Change: {comparison.DeletionsChange:F1}%");
                Console.WriteLine($"Net Lines of Code Change: {comparison.NetLinesChange:F1}%");
            }
        }

        public void DisplayError(Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
} 