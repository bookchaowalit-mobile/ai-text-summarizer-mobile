# Upgrade plan

## Current state

Score: 6/10 (was 4/10) — app now has the full MAUI layout and its shared XAML/C#
compiles (XamlC) against the core; platform heads are CI-built only, no persistence yet.

## Backlog

- P0: Confirm the new `android` CI job is green on GitHub (never run locally:
  this container has no Android SDK / MAUI workload). Fix whatever it reports.
- P1: Thai support — Thai has no sentence punctuation or word spaces; needs a dictionary word segmenter (e.g. ICU `BreakIterator`) before scoring.
- P1: Optional online abstractive mode via the Central AI Gateway, clearly labelled and opt-in (would need the INTERNET permission).
- P1: Share-sheet / clipboard input and copy-summary button.
- P2: Build `net8.0-ios` on a macOS runner.
- P2: Add a solution file or `Directory.Build.props` to share analyzer settings
  between core and app.

## Done in this pass (pass 1)

- Added `src/TextSummarizer.Core` with the app's domain logic and 8 xUnit tests.
- App project now references the core library and excludes `src/**`/`tests/**` from its globs.
- CI: removed the `|| true` steps that hid a non-compiling app; runs the core tests for real.
- README now states what is verified and what is not.

## Done in this pass (pass 2)

- Scaffolded the standard `dotnet new maui` (.NET 8) files wired to the core:
  `App.xaml`, `AppShell.xaml.cs`, `MauiProgram.cs`, `Platforms/Android`
  (`MainActivity`, `MainApplication`, manifest without INTERNET), `Platforms/iOS`
  (`AppDelegate`, `Program`, `Info.plist`), `Resources/` icon + splash SVGs.
- `RootNamespace` now matches the C# namespace; added `ApplicationId`, versions,
  `ImplicitUsings`/`Nullable`, and a unique `ApplicationIdGuid`.
- Replaced the missing `ExplorePage`/`ProfilePage` with a single Summarize page using `TextSummarizer.Core`.
- Fixed sentence splitting: closing quotes/brackets after a terminator (`"stop." Then`) no longer glue two sentences together; CJK full stops split without a following space.
- 10 more edge-case tests (quotes, CJK, empty/whitespace input, ellipsis, budgets, stop-word-only text, compression ratio): 18 total.
- `MauiSharedCheck=true` build switch: compiles the shared XAML/C# as `net8.0`
  without the workload (verified locally, Release/XamlC, 0 warnings).
- CI: new `app-shared` job (that check) and `android` job (`maui-android` workload build).

## Done in this pass (pass 3)

- Fixed word splitting for Thai/Indic text: the word regex lacked `\p{M}`, so combining vowels/tone marks split "สวัสดี" into "สว" + "สด", inflating word counts and producing junk keywords.
- Fixed curly apostrophes (U+2019, the default on iOS keyboards): "don’t" was two words ("don", "t"); it is now one word normalised to "don't".
- Fixed sentence splitting after an abbreviation that follows an opening bracket/quote: "(e.g. apples)" and "\"Dr. Who\"" were cut into separate sentences.
- 3 regression tests (21 total), verified with `dotnet test tests/TextSummarizer.Core.Tests` and `dotnet build TextSummarizer.csproj -c Release -p:MauiSharedCheck=true` (0 warnings).
