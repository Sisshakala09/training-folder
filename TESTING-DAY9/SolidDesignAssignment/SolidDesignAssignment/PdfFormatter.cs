public class PdfFormatter : IReportFormatter
{
    public string Format(string content)
    {
        return $"PDF Format: {content}";
    }
}
