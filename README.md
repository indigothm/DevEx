# GitHub Repository Analyzer

A .NET-based tool that analyzes GitHub repositories, providing both quantitative metrics and AI-powered qualitative code analysis.

## 🚀 Features

- **Quantitative Analysis**
  - Track commits, additions, and deletions
  - Compare statistics across different time periods
  - Per-contributor metrics and comparisons

- **AI-Powered Qualitative Analysis**
  - Code quality scoring (1-5) using Azure OpenAI
  - Automated code review feedback
  - Quality trends over time

- **Flexible Analysis Options**
  - Single period analysis
  - Two-period comparison
  - Combined quantitative and qualitative insights

## 📋 Prerequisites

- .NET 8.0 SDK or later
- GitHub account and personal access token
- Azure OpenAI account (for qualitative analysis)

## 🛠️ Installation

1. Clone the repository:
bash
git clone https://github.com/yourusername/github-analyzer.git
cd github-analyzer
```

2. Build the project:
```bash
dotnet build
```

3. Run the application:
```bash
dotnet run
```

## 💻 Usage

1. Choose analysis mode:
   - Single period analysis
   - Compare two periods

2. Select analysis type:
   - Quantitative (commits, additions, deletions)
   - Qualitative (AI-powered code quality analysis)
   - Both

3. Enter repository details:
   - Owner/organization name
   - Repository name
   - Time period(s)
   - GitHub token

4. For qualitative analysis, provide Azure OpenAI credentials:
   - Deployment name
   - Endpoint
   - API key
   - Model ID

## 🏗️ Project Structure

```
GitHubAnalyzer/
├── Program.cs                    # Application entry point
├── Models/
│   ├── AnalysisType.cs          # Analysis type enums
│   ├── ContributorStats.cs      # Contributor statistics model
│   └── ContributorComparison.cs # Comparison metrics model
├── Services/
│   ├── GitHubService.cs         # GitHub interaction logic
│   └── CodeQualityAnalyzer.cs   # AI-powered code analysis
├── UI/
│   └── ConsoleUI.cs             # User interface handling
└── GitHubAnalyzer.csproj        # Project configuration
```

## 🔧 Technologies Used

- .NET 8.0
- Octokit (GitHub API client)
- Microsoft Semantic Kernel
- Azure OpenAI
- C# 12

## 🤝 Contributing

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request