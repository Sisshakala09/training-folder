using System;

class Program
{
    static void Main()
    {
        Logger.Instance.Log("Starting Application...");

        IReportFormatter formatter = new PdfFormatter(); // or new ExcelFormatter()

        var generator = new ReportGenerator(formatter);
        var saver = new ReportSaver();
        IReportService reportService = new ReportService(generator, saver);
        reportService.GenerateAndSaveReport();

        Report monthly = new MonthlyReport();
        Report annual = new AnnualReport();
        Console.WriteLine(monthly.GetContent());
        Console.WriteLine(annual.GetContent());

        var detailed = new DetailedReport();
        detailed.Print();
        detailed.Save("detailed.txt");

        Logger.Instance.Log("Application Finished.");
    }
}
