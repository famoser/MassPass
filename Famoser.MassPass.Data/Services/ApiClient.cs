using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using Famoser.FrameworkEssentials.Models.RestService;
using Famoser.FrameworkEssentials.Services;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.MassPass.Data.Attributes;
using Famoser.MassPass.Data.Entities;
using Famoser.MassPass.Data.Entities.Communications.Request;
using Famoser.MassPass.Data.Entities.Communications.Request.Authorization;
using Famoser.MassPass.Data.Entities.Communications.Request.Raw;
using Famoser.MassPass.Data.Entities.Communications.Response;
using Famoser.MassPass.Data.Entities.Communications.Response.Authorization;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;
using Famoser.MassPass.Data.Enum;
using Famoser.MassPass.Data.Exceptions;
using Famoser.MassPass.Data.Models;
using Famoser.MassPass.Data.Services.Interfaces;
using Newtonsoft.Json;

namespace Famoser.MassPass.Data.Services
{
    public class ApiClient
    {
        private readonly IRestService _restService = new RestService();
        private readonly Uri _baseUri;
        private readonly Guid _userId;
        private readonly Guid _deviceId;

        public ApiClient(Uri baseUri, Guid userId, Guid deviceId)
        {
            _baseUri = baseUri;
            _userId = userId;
            _deviceId = deviceId;
        }

        public async Task<bool> CheckIsAuthorizedAsync()
        {
            var res = await GetAuthorizationStatusAsync(new AuthorizationStatusRequest()
            {
                DeviceId = _deviceId,
                UserId = _userId
            });
            return res.IsAuthorized;
        }

        private Uri GetUri(ApiRequest rt)
        {
            var type = typeof(ApiRequest);
            var attribute = type.GetRuntimeField(rt.ToString()).GetCustomAttribute<ApiUriAttribute>();
            return new Uri(_baseUri + attribute.RelativeUrl);
        }

        private async Task<HttpResponseModel> PostJsonToApiRaw(object request, ApiRequest type)
        {
            try
            {
                return await _restService.PostJsonAsync(GetUri(type), JsonConvert.SerializeObject(request));
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
                return new HttpResponseModel(ex);
            }
        }

        private async Task<T> PostJsonToApi<T>(object request, ApiRequest type) where T : ApiResponse, new()
        {
            var response = await _restService.PostJsonAsync(GetUri(type), JsonConvert.SerializeObject(request));
            var rawResponse = await response.GetResponseAsStringAsync();
            var obj = JsonConvert.DeserializeObject<T>(rawResponse);
            if (obj != null)
            {
                obj.RawResponse = response;
                return obj;
            }
            LogHelper.Instance.LogError("response from api null", this);
            return new T()
            {
                RawResponse = response
            };
        }

        public Task<AuthorizationStatusResponse> GetAuthorizationStatusAsync(AuthorizationStatusRequest request)
        {
            return PostJsonToApi<AuthorizationStatusResponse>(request, ApiRequest.AuthorizationStatus);
        }

        public Task<AuthorizationResponse> AuthorizeAsync(AuthorizationRequest request)
        {
            return PostJsonToApi<AuthorizationResponse>(request, ApiRequest.Authorize);
        }

        public Task<CreateAuthorizationResponse> CreateAuthorizationAsync(CreateAuthorizationRequest request)
        {
            return PostJsonToApi<CreateAuthorizationResponse>(request, ApiRequest.CreateAuthorization);
        }

        public Task<UnAuthorizationResponse> UnAuthorizeAsync(UnAuthorizationRequest request)
        {
            return PostJsonToApi<UnAuthorizationResponse>(request, ApiRequest.UnAuthorize);
        }

        public Task<AuthorizedDevicesResponse> GetAuthorizedDevicesAsync(AuthorizedDevicesRequest request)
        {
            return PostJsonToApi<AuthorizedDevicesResponse>(request, ApiRequest.AuthorizedDevices);
        }

        public Task<RefreshResponse> RefreshAsync(RefreshRequest request)
        {
            return PostJsonToApi<RefreshResponse>(request, ApiRequest.Refresh);
        }

        public async Task<TransferEntityResponse> ReadAsync(ContentEntityRequest request)
        {
            var req = new RawTransferEntityRequest()
            {
                UserId = request.UserId,
                DeviceId = request.DeviceId,
                ContentId = request.ContentId,
                VersionId = request.VersionId
            };
            var resp = await PostJsonToApiRaw(req, ApiRequest.ReadContentEntity);
            if (resp.IsRequestSuccessfull)
            {
                return new TransferEntityResponse()
                {
                    ApiError = ApiError.None,
                    EncryptedBytes = await resp.GetResponseAsByteArrayAsync()
                };
            }
            return new TransferEntityResponse()
            {
                RawResponse = resp
            };
        }

        public async Task<UpdateResponse> UpdateAsync(UpdateRequest request, byte[] content)
        {
            var newRequest = new RawUpdateRequest()
            {
                DeviceId = request.DeviceId,
                ContentId = request.ContentId,
                UserId = request.UserId
            };

            var response = await _restService.PostJsonAsync(GetUri(ApiRequest.Update), JsonConvert.SerializeObject(newRequest),
                new List<RestFile>()
                {
                    new RestFile()
                    {
                        Content = content,
                        ContentName = "updateFile",
                        FileName = request.ContentId.ToString()
                    }
                });
            var rawResponse = await response.GetResponseAsStringAsync();
            var obj = JsonConvert.DeserializeObject<UpdateResponse>(rawResponse);
            if (obj != null)
            {
                obj.RawResponse = response;
                return obj;
            }
            LogHelper.Instance.LogError("response from api null", this);
            return new UpdateResponse()
            {
                RawResponse = response
            };
        }

        public Task<CollectionEntriesResponse> ReadAsync(CollectionEntriesRequest request)
        {
            return PostJsonToApi<CollectionEntriesResponse>(request, ApiRequest.ReadCollectionEntries);
        }

        public Task<ContentEntityHistoryResponse> GetHistoryAsync(ContentEntityHistoryRequest request)
        {
            return PostJsonToApi<ContentEntityHistoryResponse>(request, ApiRequest.GetHistory);
        }
    }
}
