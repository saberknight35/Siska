using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Siska.Admin.Storage.Model;

namespace Siska.Admin.Storage.Implementations
{
    public class LocalStorage : IStorage
    {
        private string targetFilePath = string.Empty;
        private ILogger<LocalStorage> logging;

        public LocalStorage(ILogger<LocalStorage> logging, IConfiguration config)
        {
            // To save physical files to a path provided by configuration:
            targetFilePath = config.GetValue<string>("LocalStorage");
            this.logging = logging;
        }

        public async Task<string> SaveFileAsync(byte[] uploadFileStream, string container, string fileName)
        {
            if (!string.IsNullOrEmpty(targetFilePath))
                container = Path.Combine(targetFilePath, container);

            fileName = Path.Combine(container, fileName);

            var folder = Path.GetDirectoryName(fileName);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            await File.WriteAllBytesAsync(fileName, uploadFileStream);

            // Return the file path or any identifier you prefer
            return fileName;

        }

        public void DeleteFile(string uri)
        {
            File.Delete(uri);
        }

        public void DeleteFile(string container, string fileName)
        {
            if (!string.IsNullOrEmpty(targetFilePath))
                container = Path.Combine(targetFilePath, container);

            var uri = Path.Combine(container, fileName);

            DeleteFile(uri);
        }

        public void DeleteFolder(string container, string folder)
        {
            if (!string.IsNullOrEmpty(targetFilePath))
                container = Path.Combine(targetFilePath, container);

            var folders = Path.Combine(container, folder);

            if (Directory.Exists(folders))
            {
                Directory.Delete(folders, recursive: true);
            }
        }

        public IList<FileProperty> ListFileOnFolder(string container, string folder, int pageSize, int pageIndex)
        {
            if (!string.IsNullOrEmpty(targetFilePath))
                container = Path.Combine(targetFilePath, container);

            var folders = Path.Combine(container, folder);

            if (!Directory.Exists(folders))
            {
                return null;
            }

            var files = Directory.GetFiles(folders);

            // Optionally, sort files (e.g., by name)
            var orderedFiles = files.OrderBy(f => f).ToArray();

            int skip = pageSize * pageIndex;

            var pagedFiles = orderedFiles.Skip(skip).Take(pageSize);

            var result = pagedFiles.Select(f =>
            {
                var fileInfo = new FileInfo(f);
                return new FileProperty
                {
                    Name = fileInfo.Name,
                    Size = fileInfo.Length,
                    ModifiedOn = fileInfo.CreationTimeUtc.ToLongDateString(),
                    Uri = fileInfo.LinkTarget,
                };
            }).ToList();

            return result;
        }

        public int CountFileOnFolder(string container, string folder)
        {
            if (!string.IsNullOrEmpty(targetFilePath))
                container = Path.Combine(targetFilePath, container);

            var folders = Path.Combine(container, folder);

            if (!Directory.Exists(folders))
            {
                throw new DirectoryNotFoundException($"The directory '{folder}' does not exist.");
            }

            return Directory.GetFiles(folders).Length;
        }

        public async Task<byte[]> GetFile(string container, string fileName)
        {
            if (!string.IsNullOrEmpty(targetFilePath))
                container = Path.Combine(targetFilePath, container);

            string filePath = Path.Combine(container, fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file '{fileName}' does not exist in folder '{container}'.");
            }

            return await File.ReadAllBytesAsync(filePath);

        }
    }
}
