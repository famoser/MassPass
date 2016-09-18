using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using Famoser.FrameworkEssentials.Models.RestService;
using Famoser.FrameworkEssentials.Services;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.MassPass.Data.Attributes;
using Famoser.MassPass.Data.Entities.Communications.Request;
using Famoser.MassPass.Data.Entities.Communications.Request.Authorization;
using Famoser.MassPass.Data.Entities.Communications.Request.Base;
using Famoser.MassPass.Data.Entities.Communications.Response;
using Famoser.MassPass.Data.Entities.Communications.Response.Authorization;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;
using Famoser.MassPass.Data.Enum;
using Newtonsoft.Json;

namespace Famoser.MassPass.Data
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

        public Guid UserId => _userId;
        public Guid DeviceId => _deviceId;
        public Uri BaseUri => _baseUri;

        private bool _isAuthorized;
        public async Task<bool> CheckIsAuthorizedAsync()
        {
            if (!_isAuthorized) //cache authentication to save requests
            {
                var res = await GetAuthorizationStatusAsync(new AuthorizationStatusRequest()
                {
                    DeviceId = _deviceId,
                    UserId = _userId
                });
                _isAuthorized = res.IsAuthorized;
            }
            return _isAuthorized;
        }

        private T AuthenticateRequest<T>(T request) where T : ApiRequest
        {
            request.DeviceId = _deviceId;
            request.UserId = _userId;
            return request;
        }

        private Uri GetUri(ApiNode rt)
        {
            var type = typeof(ApiNode);
            var attribute = type.GetRuntimeField(rt.ToString()).GetCustomAttribute<ApiUriAttribute>();
            return new Uri(_baseUri + attribute.RelativeUrl);
        }

        private async Task<T> PostJsonToApi<T>(ApiRequest request, ApiNode type) where T : ApiResponse, new()
        {
            request = AuthenticateRequest(request);
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
            return PostJsonToApi<AuthorizationStatusResponse>(request, ApiNode.AuthorizationStatus);
        }

        public Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request)
        {
            return PostJsonToApi<CreateUserResponse>(request, ApiNode.CreateUser);
        }

        public Task<AuthorizationResponse> AuthorizeAsync(AuthorizationRequest request)
        {
            return PostJsonToApi<AuthorizationResponse>(request, ApiNode.Authorize);
        }

        public Task<CreateAuthorizationResponse> CreateAuthorizationAsync(CreateAuthorizationRequest request)
        {
            return PostJsonToApi<CreateAuthorizationResponse>(request, ApiNode.CreateAuthorization);
        }

        public Task<WipeUserResponse> WipeUserAsync(WipeUserRequest request)
        {
            return PostJsonToApi<WipeUserResponse>(request, ApiNode.WipeUser);
        }

        public Task<UnAuthorizationResponse> UnAuthorizeAsync(UnAuthorizationRequest request)
        {
            return PostJsonToApi<UnAuthorizationResponse>(request, ApiNode.UnAuthorize);
        }

        public Task<AuthorizedDevicesResponse> GetAuthorizedDevicesAsync(AuthorizedDevicesRequest request)
        {
            return PostJsonToApi<AuthorizedDevicesResponse>(request, ApiNode.AuthorizedDevices);
        }

        public Task<SyncResponse> SyncAsync(SyncRequest request)
        {
            return PostJsonToApi<SyncResponse>(request, ApiNode.SyncContent);
        }

        public Task<ContentEntityHistoryResponse> GetHistoryAsync(ContentEntityHistoryRequest request)
        {
            return PostJsonToApi<ContentEntityHistoryResponse>(request, ApiNode.GetHistory);
        }

        public async Task<TransferEntityResponse> ReadAsync(ContentEntityRequest request)
        {
            request = AuthenticateRequest(request);
            var resp = await _restService.PostJsonAsync(GetUri(ApiNode.ReadContentEntity), JsonConvert.SerializeObject(request));
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
            request = AuthenticateRequest(request);
            var response = await _restService.PostJsonAsync(GetUri(ApiNode.Update), JsonConvert.SerializeObject(request),
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
            return new UpdateResponse()
            {
                RawResponse = response
            };
        }
    }
}
