using TextSummarizer.Core;
using Xunit;

namespace TextSummarizer.Core.Tests;

public class Pass3Tests
{
    [Fact]
    public void ThaiWordsKeepTheirCombiningMarks()
    {
        // ั and ี are combining marks (\p{M}); they used to split "สวัสดี" into "สว" + "สด".
        Assert.Equal(new[] { "สวัสดี", "ครับ" }, Summarizer.Words("สวัสดี ครับ"));
        Assert.Equal(new[] { "नमस्ते" }, Summarizer.Words("नमस्ते"));
    }

    [Fact]
    public void CurlyApostrophesDoNotSplitWords()
    {
        // iOS/macOS keyboards type ’ (U+2019) by default; it is normalised to an ASCII apostrophe.
        Assert.Equal(new[] { "don't", "stop" }, Summarizer.Words("Don’t stop"));
        Assert.Equal(new[] { "it's" }, Summarizer.Words("‘it's’"));
        Assert.Equal(1, Summarizer.ReadingMinutes("don’t"));
    }

    [Fact]
    public void AbbreviationAfterOpeningBracketOrQuoteDoesNotEndTheSentence()
    {
        var s = Summarizer.SplitSentences("Use fruit (e.g. apples) daily. Also \"Dr. Who\" is fine.");
        Assert.Equal(new[] { "Use fruit (e.g. apples) daily.", "Also \"Dr. Who\" is fine." }, s);
    }
}
