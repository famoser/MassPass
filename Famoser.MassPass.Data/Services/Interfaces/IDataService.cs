﻿using System.Threading.Tasks;
using Famoser.MassPass.Data.Entities.Communications;
using Famoser.MassPass.Data.Entities.Communications.Request;
using Famoser.MassPass.Data.Entities.Communications.Request.Authorization;
using Famoser.MassPass.Data.Entities.Communications.Response;
using Famoser.MassPass.Data.Entities.Communications.Response.Authorization;

namespace Famoser.MassPass.Data.Services.Interfaces
{
    /// <summary>
    /// the service which communicates with the api
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Authorizes the device against the API. Please provide an authorisation code if user has already entries online
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<AuthorizationStatusResponse> GetAuthorizationStatusAsync(AuthorizationStatusRequest request);

        /// <summary>
        /// Authorizes the device against the API. Please provide an authorisation code if user has already entries online
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<AuthorizationResponse> AuthorizeAsync(AuthorizationRequest request);

        /// <summary>
        /// Provide a new AuthCode which is only valid for a short period of time, and can be used by another device to authorize itself
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<CreateAuthorizationResponse> CreateAuthorizationAsync(CreateAuthorizationRequest request);

        /// <summary>
        /// UnAuthorizes a device against the API
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<UnAuthorizationResponse> UnAuthorizeAsync(UnAuthorizationRequest request);

        /// <summary>
        /// UnAuthorizes a device against the API
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<AuthorizedDevicesResponse> GetAuthorizedDevicesAsync(AuthorizedDevicesRequest request);

        /// <summary>
        /// recieve a changelist with informations which local data may be outdated
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<RefreshResponse> RefreshAsync(RefreshRequest request);

        /// <summary>
        /// Download a specific ContentEntity by providing the ServerId
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ContentEntityResponse> ReadAsync(ContentEntityRequest request);

        /// <summary>
        /// Update the ContentEntity on the server
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<UpdateResponse> UpdateAsync(UpdateRequest request);

        /// <summary>
        /// get a list of all entries in a collection
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<CollectionEntriesResponse> ReadAsync(CollectionEntriesRequest request);

        /// <summary>
        /// Get history of changes of a specific ContentEntity
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ContentEntityHistoryResponse> GetHistoryAsync(ContentEntityHistoryRequest request);
    }
}
