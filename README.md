# DevEx Tool - GitHub Repository Analysis

A tool to analyze GitHub repository contributions and productivity metrics across different time periods.

## Features

- Single period or comparative analysis
- Quantitative metrics (commits, additions, deletions)
- Qualitative analysis with AI-powered code quality scoring
- Weighted productivity analysis
- Configuration file support for automated analysis

## Running the Tool

You can run the tool in two ways:

### 1. Interactive Mode

```bash
dotnet run --project DevEx.csproj
```

Follow the prompts to enter:
- Repository owner and name
- Analysis mode (single period or comparison)
- Analysis type (quantitative, qualitative, or both)
- Time period(s)
- GitHub token

### 2. Configuration File Mode

```bash
dotnet run --project DevEx.csproj -- config.json
```

Create a `config.json` file with the following structure:

```json
{
    "Owner": "repository-owner",
    "Repository": "repository-name",
    "AnalysisMode": "Comparison",
    "AnalysisType": "Quantitative",
    "FirstPeriod": {
        "Start": "2024-12-02",
        "End": "2024-12-08"
    },
    "SecondPeriod": {
        "Start": "2024-12-09",
        "End": "2024-12-15"
    },
    "GitHubToken": "your-github-token"
}
```

## Productivity Analysis

The tool now includes a sophisticated weighted productivity analysis that combines multiple metrics:

### Weighting Strategy

- **Commits**: 30% weight
  - Measures frequency and consistency of work
- **Lines Added**: 35% weight
  - Indicates new feature development and code expansion
- **Lines Deleted**: 35% weight
  - Reflects code refinement, cleanup, and optimization

### Productivity Score Calculation

The tool calculates a weighted productivity score using:
1. Percentage change in each metric
2. Applied weights to each metric
3. Combined weighted changes into a single productivity score

Example output:
```
Weighted Productivity Analysis:
Commits Impact (30%): -30.00%
Additions Impact (35%): +211.65%
Deletions Impact (35%): +79.75%
Overall Productivity Change: +261.40%
```

### Interpreting Results

- **Positive Overall Change**: Indicates improved productivity
- **Balanced Metrics**: Look for positive trends across all three metrics
- **High Deletion Impact**: May indicate significant code refactoring
- **High Addition Impact**: Suggests new feature development
- **Commit Impact**: Reflects development workflow changes

## Requirements

- .NET 9.0
- GitHub Personal Access Token with repo scope
- Internet connection for GitHub API access