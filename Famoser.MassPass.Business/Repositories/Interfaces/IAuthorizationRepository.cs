using System;
using System.Threading.Tasks;

namespace Famoser.MassPass.Business.Repositories.Interfaces
{
    public interface IAuthorizationRepository
    {
        Task<bool> AuthorizeNew(Guid userId, Guid deviceId, string userName, string deviceName);
        Task<bool> AuthorizeAdditional(Guid userId, Guid deviceId, string authCode, string userName, string deviceName);
    }
}
