using DotNetEnv;
using System.Text;
using System.IO;
using FrozenSplitByOrderId;
using System.Runtime.InteropServices;

internal class Program : SendEmailNotification
{
    // Importing GetConsoleWindow function from kernel32.dll to get the handle to the console window
    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    // Importing ShowWindow function from user32.dll to show or hide the console window
    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    // Constants to specify whether to hide or show the window
    const int SW_HIDE = 0; // Hides the console window
    const int SW_SHOW = 5; // Shows the console window

    static async Task Main(string[] args)
    {
        // Get the handle to the current console window
        IntPtr consoleWindow = GetConsoleWindow();
        // Hide the console window
        ShowWindow(consoleWindow, SW_HIDE);

        // Create instances of SplitCsvFile and MoveToArchive classes to handle CSV splitting and file archiving
        SplitCsvFile splitCsvFile = new SplitCsvFile();
        MoveToArchive moveToArchive = new MoveToArchive();

        // Load environment variables from the specified .env file (paths.env)
        Env.Load("./paths.env");

        // Retrieve necessary paths and SMTP client address from environment variables
        string? inputFolder = Environment.GetEnvironmentVariable("INPUTPATH");
        string? sorieOutputFolder = Environment.GetEnvironmentVariable("SORIEOUTPUTPATH");
        string? sorgbOutputFolder = Environment.GetEnvironmentVariable("SORGBOUTPUTPATH");
        string? archiveFolder = Environment.GetEnvironmentVariable("ARCHIVEPATH");
        string? smtpClientAddrs = Environment.GetEnvironmentVariable("SMTPCLIENT");

        // Validate that all necessary paths exist
        if (!ValidatePaths(inputFolder, sorieOutputFolder, sorgbOutputFolder))
        {
            // If any of the paths are invalid, send an error email and exit the program
            SendEmail("Error with folder paths!", "Please check if correct folder paths are set in app root folder 'paths.env' file!", smtpClientAddrs);
            return;
        }

        // Get an array of all CSV files in the input folder
        string[] csvFiles = Directory.GetFiles(inputFolder, "*.csv");

        // If there are no CSV files, exit the program
        if (csvFiles.Length == 0)
        {
            return;
        }

        // Split the CSV files by order ID and output them to the specified folders
        await splitCsvFile.SplitCsvByOrderId(csvFiles, sorieOutputFolder, sorgbOutputFolder, smtpClientAddrs);

        // Move the processed CSV files to the archive folder
        await moveToArchive.MoveEdiFilesToArchive(csvFiles, archiveFolder);
    }

    // Method to validate that the input, output, and archive folders exist
    private static bool ValidatePaths(string? inputFolder, string? sorieOutputFolder, string? sorgbOutputFolder)
    {
        if (!Directory.Exists(inputFolder)) // Check if the input folder exists
            return false;

        if (!Directory.Exists(sorieOutputFolder)) // Check if the SORIE output folder exists
            return false;

        if (!Directory.Exists(sorgbOutputFolder)) // Check if the SORGB output folder exists
            return false;

        return true; // All folders exist, return true
    }
}