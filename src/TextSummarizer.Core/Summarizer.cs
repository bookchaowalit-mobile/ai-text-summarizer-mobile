using System.Text;
using System.Text.RegularExpressions;

namespace TextSummarizer.Core;

public sealed record Summary(IReadOnlyList<string> Sentences, int OriginalWords, int SummaryWords)
{
    public string Text => string.Join(" ", Sentences);

    /// <summary>Summary length as a fraction of the original (0 for empty input).</summary>
    public double CompressionRatio => OriginalWords == 0 ? 0 : (double)SummaryWords / OriginalWords;
}

/// <summary>
/// Offline extractive summarizer (frequency-based, TextRank-free). Picks the
/// highest-scoring sentences and returns them in their original order, so the
/// summary never contains text that was not in the input.
/// </summary>
public static partial class Summarizer
{
    private static readonly HashSet<string> StopWords = new(StringComparer.Ordinal)
    {
        "a","an","and","are","as","at","be","been","but","by","for","from","has","have","he","her","his","i","in",
        "is","it","its","of","on","or","our","she","so","that","the","their","them","they","this","to","was","we",
        "were","which","will","with","you","your","not","can","also","than","then","there","these","those","into",
    };

    private static readonly string[] Abbreviations = { "e.g.", "i.e.", "mr.", "mrs.", "ms.", "dr.", "etc.", "vs.", "u.s." };

    [GeneratedRegex(@"[\p{L}\p{N}']+")]
    private static partial Regex WordRegex();

    public static IReadOnlyList<string> Words(string text) =>
        WordRegex().Matches(text).Select(m => m.Value.ToLowerInvariant().Trim('\'')).Where(w => w.Length > 0).ToList();

    /// <summary>
    /// Splits on . ! ? (and Thai/CJK full stops) followed by whitespace, without
    /// breaking after common abbreviations or decimals such as 3.14.
    /// </summary>
    public static IReadOnlyList<string> SplitSentences(string text)
    {
        var result = new List<string>();
        var current = new StringBuilder();
        var normalized = Regex.Replace(text ?? string.Empty, @"\s+", " ").Trim();
        for (var i = 0; i < normalized.Length; i++)
        {
            var ch = normalized[i];
            current.Append(ch);
            var terminal = ch is '.' or '!' or '?' or '。' or '！' or '？';
            var atBoundary = i + 1 == normalized.Length || normalized[i + 1] == ' ';
            if (!terminal || !atBoundary) continue;
            var sentence = current.ToString().Trim();
            var lastWord = sentence.Split(' ').Last().ToLowerInvariant();
            if (ch == '.' && Abbreviations.Contains(lastWord)) continue;
            result.Add(sentence);
            current.Clear();
        }
        var tail = current.ToString().Trim();
        if (tail.Length > 0) result.Add(tail);
        return result;
    }

    /// <summary>Summarizes to at most <paramref name="maxSentences"/> sentences.</summary>
    public static Summary Summarize(string text, int maxSentences = 3)
    {
        if (maxSentences < 1) throw new ArgumentOutOfRangeException(nameof(maxSentences));
        var sentences = SplitSentences(text);
        var originalWords = sentences.Sum(s => Words(s).Count);
        if (sentences.Count <= maxSentences) return new Summary(sentences, originalWords, originalWords);

        var freq = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var w in sentences.SelectMany(Words).Where(w => !StopWords.Contains(w)))
            freq[w] = freq.GetValueOrDefault(w) + 1;
        var max = freq.Count == 0 ? 1 : freq.Values.Max();

        double Score(string s, int index)
        {
            var words = Words(s).Where(w => !StopWords.Contains(w)).ToList();
            if (words.Count == 0) return 0;
            var score = words.Sum(w => (double)freq[w] / max) / Math.Sqrt(words.Count);
            if (index == 0) score *= 1.25; // lead sentence bias, common in articles
            return score;
        }

        var chosen = sentences.Select((s, i) => (s, i, score: Score(s, i)))
            .OrderByDescending(x => x.score).ThenBy(x => x.i)
            .Take(maxSentences)
            .OrderBy(x => x.i)
            .Select(x => x.s)
            .ToList();
        return new Summary(chosen, originalWords, chosen.Sum(s => Words(s).Count));
    }

    /// <summary>Picks a sentence budget from a target ratio (e.g. 0.3 = 30%), at least 1.</summary>
    public static int SentencesForRatio(int sentenceCount, double ratio)
    {
        if (ratio is <= 0 or > 1) throw new ArgumentOutOfRangeException(nameof(ratio));
        return Math.Max(1, (int)Math.Round(sentenceCount * ratio, MidpointRounding.AwayFromZero));
    }

    /// <summary>Top keywords (non-stop-words) by frequency, ties alphabetical.</summary>
    public static IReadOnlyList<string> Keywords(string text, int count = 5) =>
        Words(text).Where(w => !StopWords.Contains(w) && w.Length > 2)
            .GroupBy(w => w).OrderByDescending(g => g.Count()).ThenBy(g => g.Key, StringComparer.Ordinal)
            .Take(count).Select(g => g.Key).ToList();

    /// <summary>Reading time in whole minutes (min 1) at 200 wpm.</summary>
    public static int ReadingMinutes(string text) => Math.Max(1, (int)Math.Ceiling(Words(text).Count / 200.0));
}
