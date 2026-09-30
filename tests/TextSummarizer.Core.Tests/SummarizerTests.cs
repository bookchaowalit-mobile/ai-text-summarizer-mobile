using TextSummarizer.Core;
using Xunit;

namespace TextSummarizer.Core.Tests;

public class SummarizerTests
{
    private const string Article =
        "Solar power is now the cheapest source of electricity in many countries. " +
        "The weather was nice yesterday. " +
        "Falling solar panel prices and better batteries make solar power attractive for homes. " +
        "My cat likes boxes. " +
        "Governments are expanding solar power targets as electricity demand grows. " +
        "Lunch was fine.";

    [Fact]
    public void SplitsSentencesWithoutBreakingAbbreviationsOrDecimals()
    {
        var s = Summarizer.SplitSentences("Dr. Smith paid 3.14 dollars, e.g. cash.  Really?  Yes!\nDone");
        Assert.Equal(new[] { "Dr. Smith paid 3.14 dollars, e.g. cash.", "Really?", "Yes!", "Done" }, s);
        Assert.Empty(Summarizer.SplitSentences("   "));
    }

    [Fact]
    public void PicksTopicSentencesInOriginalOrder()
    {
        var summary = Summarizer.Summarize(Article, 3);
        Assert.Equal(3, summary.Sentences.Count);
        Assert.All(summary.Sentences, s => Assert.Contains("solar", s, StringComparison.OrdinalIgnoreCase));
        Assert.StartsWith("Solar power is now", summary.Text);
        Assert.EndsWith("demand grows.", summary.Text);
        Assert.InRange(summary.CompressionRatio, 0.5, 0.95);
    }

    [Fact]
    public void ShortInputIsReturnedUnchanged()
    {
        var summary = Summarizer.Summarize("One. Two.", 3);
        Assert.Equal(new[] { "One.", "Two." }, summary.Sentences);
        Assert.Equal(1.0, summary.CompressionRatio);
        Assert.Equal(0, Summarizer.Summarize("", 2).CompressionRatio);
        Assert.Throws<ArgumentOutOfRangeException>(() => Summarizer.Summarize("x", 0));
    }

    [Fact]
    public void SummaryOnlyContainsInputText()
    {
        var summary = Summarizer.Summarize(Article, 2);
        Assert.All(summary.Sentences, s => Assert.Contains(s, Article));
    }

    [Theory]
    [InlineData(10, 0.3, 3)]
    [InlineData(10, 0.25, 3)]
    [InlineData(2, 0.1, 1)]
    public void SentenceBudgetFromRatio(int count, double ratio, int expected) =>
        Assert.Equal(expected, Summarizer.SentencesForRatio(count, ratio));

    [Fact]
    public void KeywordsAndReadingTime()
    {
        Assert.Equal(new[] { "solar", "power", "electricity" }, Summarizer.Keywords(Article, 3));
        Assert.Equal(1, Summarizer.ReadingMinutes("short"));
        Assert.Equal(2, Summarizer.ReadingMinutes(string.Join(" ", Enumerable.Repeat("word", 201))));
    }
}
