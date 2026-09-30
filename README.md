# Text Summarizer — Mobile

.NET MAUI app for **Text Summarizer** — condense long text into its key sentences, on the device.

Part of [Chaowalit Greepoke](https://bookchaowalit.com)'s 101 Portfolio Projects.

## Status

- `src/TextSummarizer.Core` — plain `net8.0` class library with the app's domain
  logic, covered by xUnit tests in `tests/TextSummarizer.Core.Tests`. **This is the
  verified part of the repo** and what CI builds.
- `TextSummarizer.csproj` + XAML at the repo root — the MAUI app shell. It references
  `TextSummarizer.Core` but **does not compile yet**: `App.xaml`, `AppShell.xaml.cs`,
  `MauiProgram.cs`, the `Platforms/` folders and the `ExplorePage` /
  `ProfilePage` referenced from `AppShell.xaml` are missing. See
  [docs/UPGRADE-PLAN.md](docs/UPGRADE-PLAN.md).

## Core features (`TextSummarizer.Core`)

- Sentence splitting that does not break on common abbreviations (`Dr.`, `e.g.`) or decimals (`3.14`)
- Offline extractive summary: stop-word-filtered term frequency, length-normalised, slight lead-sentence bias; output keeps original sentence order and never invents text
- Sentence budget from a target ratio, compression ratio reporting
- Top keywords and reading-time estimate

## Tech Stack

- **Core:** C# / .NET 8 class library (nullable enabled, warnings as errors)
- **Tests:** xUnit
- **App (planned):** .NET MAUI 8 — Android, iOS

## Getting Started

```bash
dotnet test tests/TextSummarizer.Core.Tests      # build core + run unit tests
```

The MAUI app additionally needs `dotnet workload install maui` plus the
Android SDK / Xcode once the missing app files are added.

## Related

- **Frontend:** [bookchaowalit-website/text-summarizer-frontend](https://github.com/bookchaowalit-website/bookchaowalit-text-summarizer-frontend)
- **Portfolio:** [bookchaowalit.com](https://bookchaowalit.com)

## License

MIT
