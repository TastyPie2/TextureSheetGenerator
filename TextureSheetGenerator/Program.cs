using System.Drawing;
using TextureSplicer;

class Program
{
    static readonly string inputPath = Path.Combine(Environment.CurrentDirectory, "Input");
    static readonly string outputPath = Path.Combine(Environment.CurrentDirectory, "Output");
    static void Main()
    {
        if (!Directory.Exists(inputPath))
        {
            Console.WriteLine("InputFolder is missing generating...");
            Directory.CreateDirectory(inputPath);
            Thread.Sleep(1000);
            Environment.Exit(0);
        }
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        string[] files = Directory.GetFiles(inputPath);
        
        if(files.Length <= 1)
        {
            Console.WriteLine("Not enougth images.");
            Thread.Sleep(1000);
            Environment.Exit(0);
        }


        Console.WriteLine("Assebling texture sheet...");
        Bitmap bitmap = Splicer.SpliceTextures(files);
        Console.WriteLine("Done\nSaving sheet...");
        string outputLocation = outputPath + "\\" + $"{DateTime.Now.TimeOfDay.Seconds}.{DateTime.Today.Minute}.{DateTime.Now.Hour}.{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}" + ".png";
        bitmap.Save(outputLocation);
        Console.WriteLine("Saved sheet at \"{0}\"", outputLocation);
        Console.WriteLine("Done.");
        Console.WriteLine("Press enter to close the application...");
        Console.Read();
    }
}
