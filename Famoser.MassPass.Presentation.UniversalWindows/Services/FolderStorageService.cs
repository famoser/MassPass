using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Famoser.FrameworkEssentials.UniversalWindows.Platform;
using Famoser.MassPass.Business.Services.Interfaces;

namespace Famoser.MassPass.Presentation.UniversalWindows.Services
{
    public class FolderStorageService : StorageService, IFolderStorageService
    {
        public FolderStorageService() : base(false) { }

        private IAsyncOperation<StorageFolder> GetCacheSubFolder(string folderPath)
        {
            return ApplicationData.Current.LocalCacheFolder.CreateFolderAsync(folderPath,
                CreationCollisionOption.OpenIfExists);
        }

        public async Task<List<string>> GetFiles(string folderPath)
        {
            var folder = await GetCacheSubFolder(folderPath);
            var files = await folder.GetFilesAsync();
            return files.Select(storageFile => storageFile.Name).ToList();
        }

        public async Task<byte[]> GetFile(string folderPath, string fileName)
        {
            var folder = await GetCacheSubFolder(folderPath);
            var file = await folder.GetFileAsync(fileName);
            var str = await FileIO.ReadBufferAsync(file);
            return str.ToArray();
        }

        public async Task<bool> SaveFile(string folderPath, string fileName, byte[] content)
        {
            var folder = await GetCacheSubFolder(folderPath);
            StorageFile localFile = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            if (localFile != null)
            {
                await FileIO.WriteBytesAsync(localFile, content);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteFile(string folderPath, string fileName)
        {
            var folder = await GetCacheSubFolder(folderPath);
            StorageFile localFile = await folder.GetFileAsync(fileName);
            await localFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
            return true;
        }
    }
}
