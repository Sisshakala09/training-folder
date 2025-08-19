using System;
using System.IO;

public class DetailedReport : IPrintable, ISavable
{
    public void Print()
    {
        Console.WriteLine("Printing Detailed Report...");
    }

    public void Save(string path)
    {
        File.WriteAllText(path, "Detailed Report Content");
    }
}
