# Upgrade plan

## Current state

Score: 4/10 (was 1/10) — tested .NET core library and honest CI; the MAUI app
shell is still incomplete and does not compile.

## Backlog

- P0: Make the MAUI app compile: add `App.xaml`, `AppShell.xaml.cs`,
  `MauiProgram.cs` (`CreateMauiApp`), `Platforms/Android` + `Platforms/iOS`,
  and either create `ExplorePage`/`ProfilePage` or drop them from `AppShell.xaml`.
  Align `RootNamespace` (`com.bookchaowalit.*`) with the `TextSummarizer` C# namespace.
  Then add a CI job with `dotnet workload install maui-android` + `dotnet build -f net8.0-android`.
- P0: Replace the shared placeholder `ApplicationIdGuid` with a unique value per app.
- P1: Thai support — Thai has no sentence punctuation or word spaces; needs a dictionary word segmenter (e.g. ICU `BreakIterator`) before scoring.
- P1: Optional online abstractive mode via the Central AI Gateway, clearly labelled and opt-in.
- P2: Add a solution file under a subfolder or `Directory.Build.props` to share
  analyzer settings between core and app.

## Done in this pass

- Added `src/TextSummarizer.Core` with the app's domain logic and 8 xUnit tests.
- App project now references the core library and excludes `src/**`/`tests/**` from its globs.
- CI: removed the `|| true` steps that hid a non-compiling app; runs the core tests for real.
- README now states what is verified and what is not.
