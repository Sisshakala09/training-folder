/*class StringPrograms
{
    static void Main()
    {

        string text = "CSharp Language invented in 2002";

        int length = text.Length; //15
        Console.WriteLine("The Length of a string : " + length);
        string subString = text.Substring(7, 8);
        Console.WriteLine("The text from a string : " + subString);
        Console.WriteLine(text.IndexOf("harp"));
        Console.WriteLine(text.ToUpper());
        string newString = text.Replace("CSharp", "Java");
        Console.WriteLine(newString);

        String replaced = text.Replace(" ", "");
        Console.WriteLine("Without space : " + replaced.Length);

        int pos = text.IndexOf("Language");
        string newText = text.Substring(pos, 8);
        Console.WriteLine("New Text Value " + newText.ToUpper());

        //Count all the blank space

        string data = "CSharp,Language";
        String[] lang = data.Split(',');
        foreach (string valuess in lang)
        {
            Console.WriteLine(valuess);
        }
        int spaceCount = 0;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == ' ')
            {
                spaceCount++;
            }
        }
        Console.WriteLine("Total blank spaces: " + spaceCount);

//this is for Vowels
int vowelCount = 0;
text = text.ToLower(); // Convert to lowercase for easy comparison

foreach (char c in text)
{
    if (c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u')
    {
        vowelCount++;
    }
}

Console.WriteLine("Total vowels: " + vowelCount);

    }
}*/