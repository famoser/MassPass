using System;
using System.Collections.Generic;
using System.Linq;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Models.Content;
using Famoser.MassPass.Business.Models.Content.Base;
using Famoser.MassPass.Business.Models.Sync;
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
        private static readonly Dictionary<Guid, ContentTypes> Types = new Dictionary<Guid, ContentTypes>()
        {
            { Guid.Parse("2576024e-1d60-4127-bf0f-f1a83a8af93c"), ContentTypes.Collection},
            { Guid.Parse("b7e6507f-3397-4565-9768-05267f4efced"), ContentTypes.CreditCard},
            { Guid.Parse("a63b4e52-0a45-4ca5-8327-4ef800e00e57"), ContentTypes.Note},
            { Guid.Parse("20044381-b364-432e-98b8-70b38ca45282"), ContentTypes.Login}
        };
        private static ContentTypes GetType(Guid guid)
        {
            return Types.ContainsKey(guid) ? Types[guid] : ContentTypes.Unknown;
        }

        private static Guid GetTypeId(ContentTypes type)
        {
            var res = Types.Where(s => s.Value == type).ToList();
            return res.Count > 0 ? res[0].Key : Guid.Empty;
        }

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

        private ContentEntity ToContentEntity(BaseContentModel model, CollectionModel collection)
        {
            return new ContentEntity()
            {
                Name = model.Name,
                ContentJson = model.ContentJson,
                ParentId = collection.Id,
                Id = model.Id,
                TypeId = GetTypeId(model.ContentType),
                LivecycleStatus = model.LivecycleStatus,
                ContentFile = model.ContentFile
            };
        }

        public UpdateRequest UpdateRequest(Guid serverId, Guid relationId, string versionId, ContentModel  contentModel)
        {
            return FillUserConfigurationAndReturn(new UpdateRequest()
            {
                RelationId = relationId,
                ServerId = serverId,
                ContentEntity = ToContentEntity(contentModel),
                VersionId = versionId
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
