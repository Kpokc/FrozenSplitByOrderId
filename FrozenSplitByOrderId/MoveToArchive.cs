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

        private async Task MoveEdiFilesToArchive(string csvFile, string archDestFilePath)
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
    }
}
