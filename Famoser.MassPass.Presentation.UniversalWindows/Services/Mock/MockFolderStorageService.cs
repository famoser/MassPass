using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Services.Interfaces;

namespace Famoser.MassPass.Presentation.UniversalWindows.Services.Mock
{
    public class MockFolderStorageService : IFolderStorageService
    {
        public Task<string> GetCachedTextFileAsync(string fileKey)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetCachedTextFileAsync(string fileKey, string content)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GetCachedFileAsync(string fileKey)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetCachedFileAsync(string fileKey, byte[] content)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserTextFileAsync(string fileKey)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetUserTextFileAsync(string fileKey, string content)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GetUserFileAsync(string fileKey)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetUserFileAsync(string fileKey, byte[] content)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetAssetTextFileAsync(string path)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GetAssetFileAsync(string path)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetFiles(string folderPath)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GetFile(string folderPath, string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveFile(string folderPath, string fileName, byte[] content)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteFile(string folderPath, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
