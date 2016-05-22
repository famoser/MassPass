using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Entities.Communications.Request;
using Famoser.MassPass.Data.Entities.Communications.Request.Authorization;
using Famoser.MassPass.Data.Entities.Communications.Response;
using Famoser.MassPass.Data.Entities.Communications.Response.Authorization;
using Famoser.MassPass.Data.Services.Interfaces;

namespace Famoser.MassPass.Data.Services
{
    public class DataService : IDataService
    {
        private IEncryptionService _encryptionService;

        public DataService(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }


        public Task<AuthorizationResponse> Authorize(AuthorizationRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<UnAuthorizationResponse> UnAuthorize(UnAuthorizationRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<RefreshResponse> Refresh(RefreshRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ContentEntityResponse> Read(ContentEntityRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<UpdateResponse> Update(UpdateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<CollectionEntriesResponse> Read(CollectionEntriesRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ContentEntityHistoryResponse> GetHistory(ContentEntityHistoryRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
