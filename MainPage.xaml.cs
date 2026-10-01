using TextSummarizer.Core;

namespace TextSummarizer;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private int MaxSentences => (int)CountStepper.Value;

    private void OnCountChanged(object? sender, ValueChangedEventArgs e) => CountLabel.Text = MaxSentences.ToString();

    private void OnSummarizeClicked(object? sender, EventArgs e)
    {
        var text = InputEditor.Text ?? string.Empty;
        if (string.IsNullOrWhiteSpace(text))
        {
            StatsLabel.Text = "Paste some text first.";
            SummaryLabel.Text = KeywordsLabel.Text = string.Empty;
            return;
        }

        var summary = Summarizer.Summarize(text, MaxSentences);
        StatsLabel.Text = $"{summary.OriginalWords} → {summary.SummaryWords} words ({summary.CompressionRatio:P0}) · " +
                          $"{Summarizer.ReadingMinutes(text)} min read";
        SummaryLabel.Text = summary.Text;
        KeywordsLabel.Text = "Keywords: " + string.Join(", ", Summarizer.Keywords(text));
    }
}
