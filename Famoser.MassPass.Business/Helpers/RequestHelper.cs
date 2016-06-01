using System;
using System.Collections.Generic;
using Famoser.MassPass.Data.Entities;
using Famoser.MassPass.Data.Entities.Communications.Request;
using Famoser.MassPass.Data.Entities.Communications.Request.Authorization;
using Famoser.MassPass.Data.Entities.Communications.Request.Base;
using Famoser.MassPass.Data.Entities.Communications.Request.Entities;
using Famoser.MassPass.Data.Models.Storage;

namespace Famoser.MassPass.Business.Helpers
{
    public class RequestHelper
    {
        private readonly UserConfiguration _config;
        public RequestHelper(UserConfiguration config)
        {
            _config = config;
        }
        private T FillUserConfigurationAndReturn<T>(T request) where T : ApiRequest
        {
            request.UserId = _config.UserId;
            request.DeviceId = _config.DeviceId;
            return request;
        }

        public AuthorizationStatusRequest AuthorizationStatusRequest()
        {
            var request = new AuthorizationStatusRequest();
            return FillUserConfigurationAndReturn(request);
        }

        public AuthorizationRequest AuthorizationRequest(string userName, string deviceName, string authorizationCode)
        {
            var request = new AuthorizationRequest()
            {
                UserName = userName,
                DeviceName = deviceName,
                AuthorisationCode = authorizationCode
            };
            return FillUserConfigurationAndReturn(request);
        }

        public AuthorizedDevicesRequest AuthorizedDevicesRequest()
        {
            return FillUserConfigurationAndReturn(new AuthorizedDevicesRequest());
        }

        public CreateAuthorizationRequest CreateAuthorizationRequest(string authorizationCode)
        {
            return FillUserConfigurationAndReturn(new CreateAuthorizationRequest()
            {
                AuthorisationCode = authorizationCode
            });
        }

        public UnAuthorizationRequest UnAuthorizationRequest(Guid deviceToBlockGuid, string reason)
        {
            return FillUserConfigurationAndReturn(new UnAuthorizationRequest()
            {
                DeviceToBlockId = deviceToBlockGuid,
                Reason = reason
            });
        }

        public CollectionEntriesRequest CollectionEntriesRequest(List<Guid> knownServerGuids, Guid relationId)
        {
            return FillUserConfigurationAndReturn(new CollectionEntriesRequest()
            {
                KnownServerIds = knownServerGuids,
                RelationId = relationId
            });
        }

        public ContentEntityRequest ContentEntityRequest(Guid serverId, Guid relationId, string versionId)
        {
            return FillUserConfigurationAndReturn(new ContentEntityRequest()
            {
                ServerId = serverId,
                VersionId = versionId,
                RelationId = relationId
            });
        }

        public ContentEntityHistoryRequest ContentEntityHistoryRequest(Guid serverId)
        {
            return FillUserConfigurationAndReturn(new ContentEntityHistoryRequest()
            {
                ServerId = serverId
            });
        }

        public UpdateRequest UpdateRequest(Guid serverId, Guid relationId, ContentEntity contentEntity)
        {
            return FillUserConfigurationAndReturn(new UpdateRequest()
            {
                RelationId = relationId,
                ServerId = serverId,
                ContentEntity = contentEntity
            });
        }

        public RefreshRequest RefreshRequest(List<RefreshEntity> entities)
        {
            return FillUserConfigurationAndReturn(new RefreshRequest()
            {
                RefreshEntities = entities
            });
        }
    }
}
