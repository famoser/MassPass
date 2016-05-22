using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Entities.Communications.Request;
using Famoser.MassPass.Data.Entities.Communications.Request.Authorization;
using Famoser.MassPass.Data.Entities.Communications.Response;
using Famoser.MassPass.Data.Entities.Communications.Response.Authorization;

namespace Famoser.MassPass.Data.Services.Interfaces
{
    public interface IDataService
    {
        /// <summary>
        /// Authorizes the device against the API. Please provide an authorisation code if user has already entries online
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<AuthorizationResponse> Authorize(AuthorizationRequest request);

        /// <summary>
        /// UnAuthorizes a device against the API
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<UnAuthorizationResponse> UnAuthorize(UnAuthorizationRequest request);

        /// <summary>
        /// recieve a changelist with informations which local data may be outdated
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<RefreshResponse> Refresh(RefreshRequest request);

        /// <summary>
        /// Download a specific ContentEntity by providing the ServerId
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ContentEntityResponse> Read(ContentEntityRequest request);

        /// <summary>
        /// Update the ContentEntity on the server
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<UpdateResponse> Update(UpdateRequest request);

        /// <summary>
        /// get a list of all entries in a collection
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<CollectionEntriesResponse> Read(CollectionEntriesRequest request);

        /// <summary>
        /// Get history of changes of a specific ContentEntity
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ContentEntityHistoryResponse> GetHistory(ContentEntityHistoryRequest request);
    }
}
