using System.Drawing;
using TextureSplicer;

class Program
{
    static readonly string inputPath = Path.Combine(Environment.CurrentDirectory, "Input");
    static readonly string outputPath = Path.Combine(Environment.CurrentDirectory, "Output");
    static void Main()
    {
        if(Environment.OSVersion.Platform != PlatformID.Win32NT)
        {
            Console.WriteLine("Platform not supported.");
            Console.WriteLine("This application is only supported on windows 32NT or later.");
            Shutdown();
        }

        if (!Directory.Exists(inputPath))
        {
            Console.WriteLine("InputFolder is missing generating...");
            Directory.CreateDirectory(inputPath);
            Shutdown();
        }
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        string[] files = Directory.GetFiles(inputPath);
        
        if(files.Length <= 1)
        {
            Console.WriteLine("Not enough images.");
            Shutdown();
        }

        Splicer splicer = new Splicer();

        Console.WriteLine("Assebling texture sheet...");
        Bitmap bitmap = splicer.SpliceTextures(files);
        Console.WriteLine("Done\nSaving sheet...");
        string outputLocation = outputPath + "\\" + $"{DateTime.Now.TimeOfDay.Seconds}.{DateTime.Today.Minute}.{DateTime.Now.Hour}.{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}" + ".png";
        bitmap.Save(outputLocation);
        Console.WriteLine("Saved sheet at \"{0}\"", outputLocation);
        Console.WriteLine("Done.");
        Shutdown();
    }

    static void Shutdown()
    {
        Console.WriteLine("Press enter to close the application...");
        Console.Read();
        Environment.Exit(0);
    }
}
