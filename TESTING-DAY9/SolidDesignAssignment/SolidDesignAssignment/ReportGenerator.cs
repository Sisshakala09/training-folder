public class ReportGenerator
{
    private readonly IReportFormatter _formatter;

    public ReportGenerator(IReportFormatter formatter)
    {
        _formatter = formatter;
    }

    public string GenerateReport()
    {
        string rawContent = "This is the report content.";
        return _formatter.Format(rawContent);
    }
}
