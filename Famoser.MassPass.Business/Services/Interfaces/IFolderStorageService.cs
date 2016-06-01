using System.Collections.Generic;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Services.Interfaces;

namespace Famoser.MassPass.Business.Services.Interfaces
{
    public interface IFolderStorageService : IStorageService
    {
        Task<List<string>> GetFiles(string folderPath);
        Task<byte[]> GetFile(string folderPath, string fileName);
        Task<bool> SaveFile(string folderPath, string fileName, byte[] content);
        Task<bool> DeleteFile(string folderPath, string fileName);
    }
}
