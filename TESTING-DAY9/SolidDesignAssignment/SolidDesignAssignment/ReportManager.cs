public class ReportManager
{
    private readonly ReportGenerator _generator;
    private readonly ReportSaver _saver;

    public ReportManager(ReportGenerator generator, ReportSaver saver)
    {
        _generator = generator;
        _saver = saver;
    }

    public void ProcessReport()
    {
        string report = _generator.GenerateReport();
        _saver.SaveReport(report);
    }
}
