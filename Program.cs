using GitHubAnalyzer.Services;
using GitHubAnalyzer.UI;
using GitHubAnalyzer.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace GitHubAnalyzer
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var ui = new ConsoleUI();
            
            try
            {
                var input = ui.GetUserInput();
                
                // Setup Semantic Kernel (only if qualitative analysis is requested)
                Kernel? kernel = null;
                IChatCompletionService? chatCompletionService = null;
                GitHubService githubService;

                if (input.Type is AnalysisType.Qualitative or AnalysisType.Both)
                {
                    Console.WriteLine("\nEnter Azure OpenAI Settings:");
                    Console.Write("Deployment Name: ");
                    var deploymentName = Console.ReadLine() ?? throw new InvalidOperationException("Deployment name cannot be null");
                    
                    Console.Write("Endpoint: ");
                    var endpoint = Console.ReadLine() ?? throw new InvalidOperationException("Endpoint cannot be null");
                    
                    Console.Write("API Key: ");
                    var apiKey = Console.ReadLine() ?? throw new InvalidOperationException("API key cannot be null");
                    
                    Console.Write("Model ID (e.g., gpt-4): ");
                    var modelId = Console.ReadLine() ?? throw new InvalidOperationException("Model ID cannot be null");

                    var builder = Kernel.CreateBuilder()
                        .AddAzureOpenAIChatCompletion(
                            deploymentName: deploymentName,
                            endpoint: endpoint,
                            apiKey: apiKey,
                            modelId: modelId);

                    kernel = builder.Build();
                    chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
                    
                    // Initialize GitHub service with AI capabilities
                    githubService = new GitHubService(input.Token, chatCompletionService, kernel);
                }
                else
                {
                    // Initialize GitHub service without AI capabilities
                    githubService = new GitHubService(input.Token);
                }

                if (input.Mode == ConsoleUI.AnalysisMode.SinglePeriod)
                {
                    var stats = await githubService.GetContributorStats(
                        input.Owner, 
                        input.Repo, 
                        input.FirstStart, 
                        input.FirstEnd,
                        input.Type);
                    ui.DisplayResults(stats);
                }
                else
                {
                    var comparisons = await githubService.CompareTimeRanges(
                        input.Owner, 
                        input.Repo,
                        input.FirstStart, 
                        input.FirstEnd,
                        input.SecondStart!.Value, 
                        input.SecondEnd!.Value,
                        input.Type);
                    
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
