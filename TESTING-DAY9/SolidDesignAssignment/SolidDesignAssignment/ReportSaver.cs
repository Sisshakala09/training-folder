using System.IO;

public class ReportSaver
{
    public void SaveReport(string content)
    {
        File.WriteAllText("report.txt", content);
    }
}
