# Text Summarizer — Mobile

.NET MAUI app for **Text Summarizer** — condense long text into its key sentences, on the device.

Part of [Chaowalit Greepoke](https://bookchaowalit.com)'s 101 Portfolio Projects.

## Status

- `src/TextSummarizer.Core` — plain `net8.0` class library with the app's domain
  logic, covered by xUnit tests in `tests/TextSummarizer.Core.Tests` (18 tests).
- `TextSummarizer.csproj` + XAML at the repo root — the .NET MAUI 8 app in the standard
  `dotnet new maui` layout (`App.xaml`, `AppShell.xaml`, `MauiProgram.cs`,
  `Platforms/Android`, `Platforms/iOS`, `Resources/`). One page, **Summarize**: paste text, pick a sentence budget, get the extractive summary with word counts, compression, reading time and keywords.
  State is in memory for now (see [docs/UPGRADE-PLAN.md](docs/UPGRADE-PLAN.md)).
- Verified locally without the MAUI workload: the shared XAML/C# compiles
  against `Microsoft.Maui.Controls` 8 with XamlC (`-p:MauiSharedCheck=true`,
  which builds a plain `net8.0` target and skips `Platforms/`). The Android/iOS
  heads are built by CI (`android` job) only; they have not been built locally.

## Core features (`TextSummarizer.Core`)

- Sentence splitting that does not break on common abbreviations (`Dr.`, `e.g.`) or decimals (`3.14`)
- Offline extractive summary: stop-word-filtered term frequency, length-normalised, slight lead-sentence bias; output keeps original sentence order and never invents text
- Sentence budget from a target ratio, compression ratio reporting
- Top keywords and reading-time estimate

## Tech Stack

- **Core:** C# / .NET 8 class library (nullable enabled, warnings as errors)
- **Tests:** xUnit
- **App:** .NET MAUI 8 — Android, iOS

## Getting Started

```bash
dotnet test tests/TextSummarizer.Core.Tests                                    # core + unit tests
dotnet build TextSummarizer.csproj -c Release -p:MauiSharedCheck=true          # app XAML/C# check, no workload needed
dotnet workload install maui-android && dotnet build TextSummarizer.csproj -f net8.0-android   # Android app (needs Android SDK + JDK 17)
```

On Linux only the Android target is enabled; macOS/Windows also get `net8.0-ios`.

## Related

- **Frontend:** [bookchaowalit-website/text-summarizer-frontend](https://github.com/bookchaowalit-website/bookchaowalit-text-summarizer-frontend)
- **Portfolio:** [bookchaowalit.com](https://bookchaowalit.com)

## License

MIT
