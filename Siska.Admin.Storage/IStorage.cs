using Siska.Admin.Storage.Model;

namespace Siska.Admin.Storage
{
    public interface IStorage
    {
        Task<string> SaveFileAsync(byte[] uploadFileStream, string container, string fileName);
        Task<byte[]> GetFile(string container, string fileName);
        void DeleteFile(string uri);
        void DeleteFile(string container, string fileName);
        void DeleteFolder(string container, string folder);
        IList<FileProperty> ListFileOnFolder(string container, string folder, int pageSize, int pageIndex);
        int CountFileOnFolder(string container, string folder);
    }
}