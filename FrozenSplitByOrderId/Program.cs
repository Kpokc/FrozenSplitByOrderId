using DotNetEnv;
using System.Text;

internal class Program
{
    static async Task Main(string[] args)
    {
        Env.Load("./paths.env");
        string? inputFolder = Environment.GetEnvironmentVariable("INPUTPATH");
        string? outputFolder = Environment.GetEnvironmentVariable("OUTPUTPATH");

        await SplitCsvByOrderId(inputFolder, outputFolder);
    }

    private static async Task SplitCsvByOrderId(string? inputFolder, string? outputFolder)
    {
        string[] csvFiles = Directory.GetFiles(inputFolder, "*.csv");

        foreach (string csvFile in csvFiles) 
        { 
            string originalFileName = Path.GetFileNameWithoutExtension(csvFile);

            string sorieFilePath = Path.Combine(outputFolder, $"{originalFileName}_SORIE.csv");
            string sorgbFilePath = Path.Combine(outputFolder, $"{originalFileName}_SORGB.csv");

            using (var reader = new StreamReader(csvFile))
            using (var sorieWriter = new StreamWriter(sorieFilePath, false, Encoding.UTF8))
            using (var sorgbWriter = new StreamWriter(sorgbFilePath, false, Encoding.UTF8))
            {
                string headerLine = await reader.ReadLineAsync();

                if (headerLine != null)
                {
                    await sorieWriter.WriteLineAsync(headerLine);
                    await sorgbWriter.WriteLineAsync(headerLine);
                }

                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var columns = line.Split(',');

                    // Assuming the reference column is the first one (index 0)
                    if (columns[0].Contains("SORIE"))
                    {
                        await sorieWriter.WriteLineAsync(line);
                    }
                    else if (columns[0].Contains("SORGB"))
                    {
                        await sorgbWriter.WriteLineAsync(line);
                    }
                }
            }
        }
    }
}