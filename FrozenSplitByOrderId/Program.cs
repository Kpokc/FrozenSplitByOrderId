﻿using DotNetEnv;
using System.Text;
using System.IO;

internal class Program
{
    static async Task Main(string[] args)
    {
        Env.Load("./paths.env");
        string? inputFolder = Environment.GetEnvironmentVariable("INPUTPATH");
        string? sorieOutputFolder = Environment.GetEnvironmentVariable("SORIEOUTPUTPATH");
        string? sorgbOutputFolder = Environment.GetEnvironmentVariable("SORGBOUTPUTPATH");

        if (!ValidatePaths(inputFolder, sorieOutputFolder, sorgbOutputFolder))
        {
            return;
        }

        await SplitCsvByOrderId(inputFolder, sorieOutputFolder, sorgbOutputFolder);
    }

    private static bool ValidatePaths(string inputFolder, string sorieOutputFolder, string sorgbOutputFolder)
    {
        if (!Directory.Exists(inputFolder))
            return false;

        if (!Directory.Exists(sorieOutputFolder))
            return false;

        if (!Directory.Exists(sorgbOutputFolder))
            return false;

        return true;
    }

    private static async Task SplitCsvByOrderId(string? inputFolder, string? sorieOutputFolder, string? sorgbOutputFolder)
    {
        string[] csvFiles = Directory.GetFiles(inputFolder, "*.csv");

        foreach (string csvFile in csvFiles) 
        { 
            string originalFileName = Path.GetFileNameWithoutExtension(csvFile);

            string sorieFilePath = Path.Combine(sorieOutputFolder, $"{originalFileName}_SORIE.csv");
            string sorgbFilePath = Path.Combine(sorgbOutputFolder, $"{originalFileName}_SORGB.csv");

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
                    if (columns[3].Contains("SORIE"))
                    {
                        await sorieWriter.WriteLineAsync(line);
                    }
                    else if (columns[3].Contains("SORGB"))
                    {
                        await sorgbWriter.WriteLineAsync(line);
                    }
                }
            }
        }
    }
}