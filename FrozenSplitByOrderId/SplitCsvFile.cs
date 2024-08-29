using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSplitByOrderId
{
    internal class SplitCsvFile: SendEmailNotification
    {
        public async Task SplitCsvByOrderId(string[]? csvFiles, string? sorieOutputFolder, string? sorgbOutputFolder, string? smtpClientAddrs)
        {
            // Iterate through each CSV file provided in the array
            foreach (string? csvFile in csvFiles)
            {
                // Get the original file name without the extension to use it in output file names
                string originalFileName = Path.GetFileNameWithoutExtension(csvFile);

                // Define the paths for the output files, appending "_SORIE" and "_SORGB" to the original file name
                string sorieFilePath = Path.Combine(sorieOutputFolder, $"{originalFileName}_SORIE.csv");
                string sorgbFilePath = Path.Combine(sorgbOutputFolder, $"{originalFileName}_SORGB.csv");

                // Set up retry logic in case of file access issues
                int maxRetries = 10; // Maximum number of retry attempts
                int delayBetweenRetries = 2000; // Delay between retries in milliseconds (2 seconds)
                int retryCount = 0; // Counter to track the number of retries
                bool fileAccessed = false; // Flag to indicate if the file was successfully accessed

                // Retry loop to handle potential IO exceptions
                while (retryCount < maxRetries && !fileAccessed)
                {
                    try
                    {
                        // Set up a cancellation token to enforce a 15-second timeout for the file processing operation
                        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));

                        // Start a task to process the CSV file
                        await Task.Run(async () =>
                        {
                            // Open the CSV file for reading and the output files for writing
                            using (var reader = new StreamReader(csvFile))
                            using (var sorieWriter = new StreamWriter(sorieFilePath, false, Encoding.UTF8))
                            using (var sorgbWriter = new StreamWriter(sorgbFilePath, false, Encoding.UTF8))
                            {
                                // Read and write the header line to both output files
                                string? headerLine = await reader.ReadLineAsync();

                                if (headerLine != null)
                                {
                                    await sorieWriter.WriteLineAsync(headerLine);
                                    await sorgbWriter.WriteLineAsync(headerLine);
                                }

                                // Process each line of the CSV file
                                string? line;
                                while ((line = await reader.ReadLineAsync()) != null)
                                {
                                    var columns = line.Split(',');

                                    // Check the fourth column (index 3) to determine which file the line should be written to
                                    if (columns[3].Contains("SORIE"))
                                    {
                                        await sorieWriter.WriteLineAsync(line); // Write to the SORIE file
                                    }
                                    else if (columns[3].Contains("SORGB"))
                                    {
                                        await sorgbWriter.WriteLineAsync(line); // Write to the SORGB file
                                    }
                                }
                            }
                        }, cts.Token); // Pass the cancellation token to the task

                        fileAccessed = true; // If the task completes successfully, mark the file as accessed
                    }
                    catch (OperationCanceledException)
                    {
                        // Handle the case where the operation times out
                        Console.WriteLine($"Operation timed out while accessing file: {csvFile}");
                        break; // Exit the retry loop if a timeout occurs
                    }
                    catch (IOException ex)
                    {
                        // Handle IO exceptions, such as file access issues
                        retryCount++;
                        Console.WriteLine($"Attempt {retryCount} - File access error for file {csvFile}: {ex.Message}");

                        if (retryCount < maxRetries)
                        {
                            // Wait for a specified delay before retrying
                            await Task.Delay(delayBetweenRetries);
                        }
                        else
                        {
                            // If the maximum number of retries is reached, log the failure and send an error email
                            Console.WriteLine($"Failed to access file {csvFile} after {maxRetries} attempts.");
                            SendEmail("Failed to access file!", $"Failed to access file {csvFile} after {maxRetries} attempts.", smtpClientAddrs);
                            break; // Exit the retry loop after max attempts
                        }
                    }
                }
            }
        }

    }
}
