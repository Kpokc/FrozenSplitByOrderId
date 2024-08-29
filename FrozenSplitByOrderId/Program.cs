using DotNetEnv;

internal class Program
{
    private static void Main(string[] args)
    {
        Env.Load("./paths.env");
        string? inputFolder = Environment.GetEnvironmentVariable("INPUTPATH");
        string? outputFolder = Environment.GetEnvironmentVariable("OUTPUTPATH");

        SplitCsvByOrderId(inputFolder, outputFolder);
    }

    private static void SplitCsvByOrderId(string? inputFolder, string? outputFolder)
    {
        Console.WriteLine(inputFolder);
        Console.WriteLine(outputFolder);
        Console.ReadLine();
    }
}