using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace GitHubAnalyzer.Services
{
    public class CodeQualityAnalyzer
    {
        private readonly IChatCompletionService _chatService;
        private readonly Kernel _kernel;

        public CodeQualityAnalyzer(IChatCompletionService chatService, Kernel kernel)
        {
            _chatService = chatService;
            _kernel = kernel;
        }

        public async Task<int> AnalyzeCodeQuality(string diff)
        {
            var history = new ChatHistory();
            history.AddSystemMessage(@"You are a code quality analyzer. Analyze the given code diff and rate it from 1 to 5, where:
1 = Poor quality (messy, unclear, potential bugs)
2 = Below average (needs significant improvement)
3 = Average (acceptable but could be better)
4 = Good (clean, well-structured)
5 = Excellent (exemplary code quality)
Respond only with the numeric score.");

            history.AddUserMessage($"Please analyze this code diff and provide a score:\n{diff}");

            var result = await _chatService.GetChatMessageContentAsync(
                history,
                kernel: _kernel);

            if (int.TryParse(result.Content, out int score))
            {
                return Math.Max(1, Math.Min(5, score)); // Ensure score is between 1 and 5
            }

            return 3; // Default to average if parsing fails
        }
    }
} 