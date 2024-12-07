using GitHubAnalyzer.Services;
using GitHubAnalyzer.UI;

namespace GitHubAnalyzer
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var ui = new ConsoleUI();
            
            try
            {
                var (mode, owner, repo, firstStart, firstEnd, secondStart, secondEnd, token) = ui.GetUserInput();
                
                var githubService = new GitHubService(token);

                if (mode == ConsoleUI.AnalysisMode.SinglePeriod)
                {
                    var stats = await githubService.GetContributorStats(owner, repo, firstStart, firstEnd);
                    ui.DisplayResults(stats);
                }
                else
                {
                    var comparisons = await githubService.CompareTimeRanges(
                        owner, repo,
                        firstStart, firstEnd,
                        secondStart!.Value, secondEnd!.Value);
                    
                    ui.DisplayComparison(comparisons);
                }
            }
            catch (Exception ex)
            {
                ui.DisplayError(ex);
            }
        }
    }
}
