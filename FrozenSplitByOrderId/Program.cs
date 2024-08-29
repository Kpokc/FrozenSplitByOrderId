using DotNetEnv;
using System.Text;
using System.IO;
using FrozenSplitByOrderId;

internal class Program : SendEmailNotification
{
    static async Task Main(string[] args)
    {

        SplitCsvFile splitCsvFile = new SplitCsvFile();

        Env.Load("./paths.env");
        string? inputFolder = Environment.GetEnvironmentVariable("INPUTPATH");
        string? sorieOutputFolder = Environment.GetEnvironmentVariable("SORIEOUTPUTPATH");
        string? sorgbOutputFolder = Environment.GetEnvironmentVariable("SORGBOUTPUTPATH");
        string? archiveFolder = Environment.GetEnvironmentVariable("ARCHIVEPATH");
        string? smtpClientAddrs = Environment.GetEnvironmentVariable("SMTPCLIENT");
        
        if (!ValidatePaths(inputFolder, sorieOutputFolder, sorgbOutputFolder))
        {
            SendEmail("Error with folder paths!","Please check if correct folder paths are set in app root folder 'paths.env' file!", smtpClientAddrs);
            return;
        }

        string[] csvFiles = Directory.GetFiles(inputFolder, "*.csv");

        if (csvFiles.Length == 0)
        {
            return;
        }

        await splitCsvFile.SplitCsvByOrderId(csvFiles, sorieOutputFolder, sorgbOutputFolder, smtpClientAddrs);

        await MoveEdiFilesToArchive(csvFiles, archiveFolder);
    }

    private static async Task MoveEdiFilesToArchive(string[] csvFiles, string? archiveFolder)
    {
        // Create the archive folder if it doesn't exist
        if (!Directory.Exists(archiveFolder))
        {
            Directory.CreateDirectory(archiveFolder);
        }

        foreach (string csvFile in csvFiles) 
        { 
            string archDestFilePath = Path.Combine(archiveFolder, Path.GetFileName(csvFile));

            await MoveEdiFilesToArchive(csvFile, archDestFilePath);
        }
    }

    private static async Task MoveEdiFilesToArchive(string csvFile, string archDestFilePath)
    {
        await Task.Run(() => 
        { 
            if (File.Exists(archDestFilePath)) 
            { 
                File.Delete(archDestFilePath);
            }
            File.Move(csvFile, archDestFilePath);
        });
    }

    private static bool ValidatePaths(string? inputFolder, string? sorieOutputFolder, string? sorgbOutputFolder)
    {
        if (!Directory.Exists(inputFolder))
            return false;

        if (!Directory.Exists(sorieOutputFolder))
            return false;

        if (!Directory.Exists(sorgbOutputFolder))
            return false;

        return true;
    }
}