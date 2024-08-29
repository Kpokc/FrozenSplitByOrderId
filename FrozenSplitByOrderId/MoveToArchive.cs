using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSplitByOrderId
{
    internal class MoveToArchive
    {
        public async Task MoveEdiFilesToArchive(string[] csvFiles, string? archiveFolder)
        {
            // Determine the archive folder path. If no archive folder is specified, create an "Archive" folder
            // in the same directory as the first CSV file.
            archiveFolder = archiveFolder is null ? Path.Combine(Path.GetDirectoryName(csvFiles[0]), "Archive") : archiveFolder;

            // If the archive folder doesn't exist, create it
            if (!Directory.Exists(archiveFolder))
            {
                Directory.CreateDirectory(archiveFolder);
            }

            // Iterate through each CSV file and move it to the archive folder
            foreach (string csvFile in csvFiles)
            {
                // Determine the destination file path in the archive folder
                string archDestFilePath = Path.Combine(archiveFolder, Path.GetFileName(csvFile));

                // Move the CSV file to the archive folder
                await MoveEdiFilesToArchive(csvFile, archDestFilePath);
            }
        }

        private async Task MoveEdiFilesToArchive(string csvFile, string archDestFilePath)
        {
            // Perform the file move operation asynchronously
            await Task.Run(() =>
            {
                // If the destination file already exists in the archive, delete it to avoid conflicts
                if (File.Exists(archDestFilePath))
                {
                    File.Delete(archDestFilePath);
                }

                // Move the CSV file to the archive destination
                File.Move(csvFile, archDestFilePath);
            });
        }

    }
}
