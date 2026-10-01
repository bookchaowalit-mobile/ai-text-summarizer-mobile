using TextSummarizer.Core;
using Xunit;

namespace TextSummarizer.Core.Tests;

public class EdgeCaseTests
{
    [Fact]
    public void ClosingQuotesAndBracketsStayWithTheirSentence()
    {
        var s = Summarizer.SplitSentences("He said \"stop.\" Then he left. (It rained.) She waved!");
        Assert.Equal(new[] { "He said \"stop.\"", "Then he left.", "(It rained.)", "She waved!" }, s);
    }

    [Fact]
    public void CjkFullStopsSplitWithoutSpaces()
    {
        var s = Summarizer.SplitSentences("今天下雨。我们在家！你呢？");
        Assert.Equal(new[] { "今天下雨。", "我们在家！", "你呢？" }, s);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   \n\t ")]
    public void EmptyInputGivesEmptySummary(string text)
    {
        Assert.Empty(Summarizer.SplitSentences(text));
        var summary = Summarizer.Summarize(text);
        Assert.Empty(summary.Sentences);
        Assert.Equal(0, summary.OriginalWords);
        Assert.Equal(0, summary.CompressionRatio);
        Assert.Equal("", summary.Text);
    }

    [Fact]
    public void TextWithoutTerminatorIsOneSentenceAndWhitespaceIsNormalized()
    {
        var s = Summarizer.SplitSentences("no  full\nstop here");
        Assert.Equal(new[] { "no full stop here" }, s);
    }

    [Fact]
    public void EllipsisAndMixedTerminators()
    {
        var s = Summarizer.SplitSentences("Wait... Really?! Yes.");
        Assert.Equal(new[] { "Wait...", "Really?!", "Yes." }, s);
    }

    [Fact]
    public void RejectsInvalidBudgets()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Summarizer.Summarize("a. b.", 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => Summarizer.SentencesForRatio(10, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => Summarizer.SentencesForRatio(10, 1.5));
        Assert.Equal(10, Summarizer.SentencesForRatio(10, 1));
    }

    [Fact]
    public void StopWordOnlyTextFallsBackToLeadSentences()
    {
        var summary = Summarizer.Summarize("It is. We are. They were. You can.", 2);
        Assert.Equal(new[] { "It is.", "We are." }, summary.Sentences);
    }

    [Fact]
    public void CompressionRatioReflectsWordCounts()
    {
        var text = "Solar panels convert sunlight. Solar farms need land and sunlight. Cats sleep a lot. Solar output peaks at noon.";
        var summary = Summarizer.Summarize(text, 2);
        Assert.Equal(2, summary.Sentences.Count);
        Assert.DoesNotContain("Cats sleep a lot.", summary.Sentences);
        Assert.InRange(summary.CompressionRatio, 0.01, 0.99);
        Assert.Equal(Summarizer.Words(summary.Text).Count, summary.SummaryWords);
    }

    [Fact]
    public void WordsAreCaseFoldedAndKeepApostrophes()
    {
        Assert.Equal(new[] { "don't", "stop", "3", "14", "café" }, Summarizer.Words("Don't STOP 3.14 'Café'"));
        Assert.Empty(Summarizer.Keywords(""));
        Assert.Equal(1, Summarizer.ReadingMinutes(""));
        Assert.Equal(2, Summarizer.ReadingMinutes(string.Join(' ', Enumerable.Repeat("word", 201))));
    }
}
