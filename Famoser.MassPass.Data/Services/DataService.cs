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
using Famoser.MassPass.Data.Entities.Communications.Response;
using Famoser.MassPass.Data.Entities.Communications.Response.Authorization;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;
using Famoser.MassPass.Data.Enum;
using Famoser.MassPass.Data.Exceptions;
using Famoser.MassPass.Data.Services.Interfaces;
using Newtonsoft.Json;

namespace Famoser.MassPass.Data.Services
{
    public class DataService : IDataService
    {
        private readonly IApiEncryptionService _apiEncryptionService;
        private readonly IConfigurationService _configurationService;
        private readonly IRestService _restService;

        public DataService(IConfigurationService configurationService, IApiEncryptionService apiEncryptionService)
        {
            _configurationService = configurationService;
            _apiEncryptionService = apiEncryptionService;
            _restService = new RestService();
        }

        private async Task<Uri> GetUri(ApiRequest rt)
        {
            var config = await _configurationService.GetApiConfiguration();
            var baseUrl = config.Uri.AbsoluteUri.TrimEnd('/');

            var type = typeof(ApiRequest);
            var attribute = type.GetRuntimeField(rt.ToString()).GetCustomAttribute<ApiUriAttribute>();
            return new Uri(baseUrl + attribute.RelativeUrl);
        }

        private async Task<HttpResponseModel> PostJsonToApiRaw(object request, ApiRequest type)
        {
            try
            {
                return await _restService.PostJsonAsync(await GetUri(type),
                    JsonConvert.SerializeObject(request));

            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
                return new HttpResponseModel(ex);
            }
        }

        private async Task<T> PostJsonToApi<T>(object request, ApiRequest type) where T : ApiResponse, new()
        {
            var rawResponse = "";
            try
            {
                var response = await _restService.PostJsonAsync(await GetUri(type),
                    JsonConvert.SerializeObject(request));
                rawResponse = await response.GetResponseAsStringAsync();
                var obj = JsonConvert.DeserializeObject<T>(rawResponse);
                if (obj != null)
                {
                    obj.RequestFailed = !response.IsRequestSuccessfull;

                    return obj;
                }
                LogHelper.Instance.LogError("response from api null", this);
                return new T()
                {
                    RequestFailed = true,
                    RawResponse = rawResponse
                };
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
                return new T()
                {
                    RequestFailed = true,
                    Exception = ex,
                    RawResponse = rawResponse
                };
            }
        }

        public Task<AuthorizationStatusResponse> AuthorizationStatus(AuthorizationStatusRequest request)
        {
            return PostJsonToApi<AuthorizationStatusResponse>(request, ApiRequest.AuthorizationStatus);
        }

        public Task<AuthorizationResponse> Authorize(AuthorizationRequest request)
        {
            return PostJsonToApi<AuthorizationResponse>(request, ApiRequest.Authorize);
        }

        public Task<CreateAuthorizationResponse> CreateAuthorization(CreateAuthorizationRequest request)
        {
            return PostJsonToApi<CreateAuthorizationResponse>(request, ApiRequest.CreateAuthorization);
        }

        public Task<UnAuthorizationResponse> UnAuthorize(UnAuthorizationRequest request)
        {
            return PostJsonToApi<UnAuthorizationResponse>(request, ApiRequest.UnAuthorize);
        }

        public Task<AuthorizedDevicesResponse> AuthorizedDevices(AuthorizedDevicesRequest request)
        {
            return PostJsonToApi<AuthorizedDevicesResponse>(request, ApiRequest.AuthorizedDevices);
        }

        public Task<RefreshResponse> Refresh(RefreshRequest request)
        {
            return PostJsonToApi<RefreshResponse>(request, ApiRequest.Refresh);
        }

        public async Task<ContentEntityResponse> Read(ContentEntityRequest request)
        {
            try
            {
                var resp = await PostJsonToApiRaw(request, ApiRequest.ReadContentEntity);
                if (resp.IsRequestSuccessfull)
                {
                    var decrypted = await _apiEncryptionService.Decrypt(await resp.GetResponseAsByteArrayAsync(),
                        request.ServerId);
                    if (decrypted != null && decrypted.Length > 0)
                    {
                        var str = Encoding.UTF8.GetString(decrypted, 0, decrypted.Length);
                        return new ContentEntityResponse()
                        {
                            ApiError = ApiError.None,
                            ContentEntity = JsonConvert.DeserializeObject<ContentEntity>(str),
                            Successfull = true
                        };
                    }
                    return new ContentEntityResponse()
                    {
                        Successfull = false,
                        Exception = new DecryptionFailedException()
                    };
                }
                try
                {
                    var jsonResponse = JsonConvert.DeserializeObject<ApiResponse>(await resp.GetResponseAsStringAsync());
                    return new ContentEntityResponse()
                    {
                        Successfull = jsonResponse.Successfull,
                        ApiError = jsonResponse.ApiError,
                        Exception = resp.Exception,
                        RequestFailed = true
                    };
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.LogException(ex, this);
                    return new ContentEntityResponse()
                    {
                        Successfull = false,
                        Exception = ex
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
                return new ContentEntityResponse()
                {
                    Successfull = false,
                    Exception = ex
                };
            }
        }

        public async Task<UpdateResponse> Update(UpdateRequest request)
        {
            try
            {
                if (request.ContentEntity != null)
                {
                    var str = JsonConvert.SerializeObject(request.ContentEntity);
                    var bytes = Encoding.UTF8.GetBytes(str);
                    var encryptedBytes = await _apiEncryptionService.Encrypt(bytes, request.ServerId);

                    var newRequest = new RawUpdateRequest()
                    {
                        DeviceId = request.DeviceId,
                        RelationId = request.RelationId,
                        ServerId = request.ServerId,
                        UserId = request.UserId
                    };

                    var response = await _restService.PostJsonAsync(await GetUri(ApiRequest.Update), JsonConvert.SerializeObject(newRequest),
                        new List<RestFile>()
                        {
                            new RestFile()
                            {
                                Content = encryptedBytes,
                                ContentName = "updateFile",
                                FileName = request.ServerId.ToString()
                            }
                        });
                    var rawResponse = await response.GetResponseAsStringAsync();
                    var obj = JsonConvert.DeserializeObject<UpdateResponse>(rawResponse);
                    if (obj != null)
                    {
                        obj.RequestFailed = !response.IsRequestSuccessfull;

                        return obj;
                    }
                    LogHelper.Instance.LogError("response from api null", this);
                    return new UpdateResponse()
                    {
                        RequestFailed = true,
                        RawResponse = rawResponse
                    };
                }
                return new UpdateResponse()
                {
                    RequestFailed = true,
                    Exception = new UploadFailedException()
                };
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
                return new UpdateResponse()
                {
                    Exception = ex,
                    Successfull = false
                };
            }
        }

        public Task<CollectionEntriesResponse> Read(CollectionEntriesRequest request)
        {
            return PostJsonToApi<CollectionEntriesResponse>(request, ApiRequest.ReadCollectionEntries);
        }

        public Task<ContentEntityHistoryResponse> GetHistory(ContentEntityHistoryRequest request)
        {
            return PostJsonToApi<ContentEntityHistoryResponse>(request, ApiRequest.GetHistory);
        }
    }
}
