using System.Threading.Tasks;
using Famoser.MassPass.Data;
using Famoser.MassPass.Data.Models.Configuration;

namespace Famoser.MassPass.Business.Repositories.Interfaces
{
    public interface IAuthorizationRepository
    {
        Task<ApiClient> CreateUserAsync(UserConfiguration configuration);
        Task<bool> CreateAuthorizationAsync(ApiClient client, string authCode, string content);
        Task<ApiClient> AuthorizeAsync(UserConfiguration configuration, string authCode);
        Task<ApiClient> GetAuthorizedApiClientAsync();
    }
}
