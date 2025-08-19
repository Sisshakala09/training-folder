public class ReportService : IReportService
{
    private readonly ReportGenerator _generator;
    private readonly ReportSaver _saver;

    public ReportService(ReportGenerator generator, ReportSaver saver)
    {
        _generator = generator;
        _saver = saver;
    }

    public void GenerateAndSaveReport()
    {
        string report = _generator.GenerateReport();
        _saver.SaveReport(report);
    }
}
