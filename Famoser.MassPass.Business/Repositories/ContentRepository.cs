using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Repositories.Interfaces;

namespace Famoser.MassPass.Business.Repositories
{
    public class ContentRepository : IContentRepository
    {
        private IStorageService _storageService;

        public ContentRepository(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public Task<ObservableCollection<CollectionModel>> GetCollectionsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> SyncAsync()
        {
            throw new NotImplementedException();
        }
    }
}
