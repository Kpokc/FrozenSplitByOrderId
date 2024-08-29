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

            foreach (string? csvFile in csvFiles)
            {
                string originalFileName = Path.GetFileNameWithoutExtension(csvFile);

                string sorieFilePath = Path.Combine(sorieOutputFolder, $"{originalFileName}_SORIE.csv");
                string sorgbFilePath = Path.Combine(sorgbOutputFolder, $"{originalFileName}_SORGB.csv");

                int maxRetries = 10;
                int delayBetweenRetries = 2000; // 2 seconds
                int retryCount = 0;
                bool fileAccessed = false;

                while (retryCount < maxRetries && !fileAccessed)
                {
                    try
                    {
                        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));

                        await Task.Run(async () =>
                        {
                            using (var reader = new StreamReader(csvFile))
                            using (var sorieWriter = new StreamWriter(sorieFilePath, false, Encoding.UTF8))
                            using (var sorgbWriter = new StreamWriter(sorgbFilePath, false, Encoding.UTF8))
                            {
                                string? headerLine = await reader.ReadLineAsync();

                                if (headerLine != null)
                                {
                                    await sorieWriter.WriteLineAsync(headerLine);
                                    await sorgbWriter.WriteLineAsync(headerLine);
                                }

                                string? line;
                                while ((line = await reader.ReadLineAsync()) != null)
                                {
                                    var columns = line.Split(',');

                                    // Assuming the reference column is the fourth one (index 3)
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
                        }, cts.Token);

                        fileAccessed = true; // Successfully accessed and processed the file
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine($"Operation timed out while accessing file: {csvFile}");
                        break; // Stop processing if timeout occurs
                    }
                    catch (IOException ex)
                    {
                        retryCount++;
                        Console.WriteLine($"Attempt {retryCount} - File access error for file {csvFile}: {ex.Message}");

                        if (retryCount < maxRetries)
                        {
                            await Task.Delay(delayBetweenRetries); // Wait before retrying
                        }
                        else
                        {
                            Console.WriteLine($"Failed to access file {csvFile} after {maxRetries} attempts.");
                            //SendEmail("Failed to access file!", $"Failed to access file {csvFile} after {maxRetries} attempts.", smtpClientAddrs);
                            break; // Stop retrying after max attempts
                        }
                    }
                }
            }
        }
    }
}
