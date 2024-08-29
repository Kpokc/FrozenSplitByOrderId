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
        string? smtpClientAddrs = Environment.GetEnvironmentVariable("SMTPCLIENT");

        if (!ValidatePaths(inputFolder, sorieOutputFolder, sorgbOutputFolder))
        {
            SendEmail("Error with folder paths!","Please check if correct folder paths are set in app root folder 'paths.env' file!", smtpClientAddrs);
            return;
        }

        await splitCsvFile.SplitCsvByOrderId(inputFolder, sorieOutputFolder, sorgbOutputFolder, smtpClientAddrs);
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
}