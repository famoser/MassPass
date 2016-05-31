using System.Threading.Tasks;
using Famoser.MassPass.Data.Services.Interfaces;
using PCLCrypto;

namespace Famoser.MassPass.Data.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly IApiConfigurationService _apiConfigurationService;

        public EncryptionService(IApiConfigurationService apiConfigurationService)
        {
            _apiConfigurationService = apiConfigurationService;
        }

        public async Task<byte[]> EncryptAsync(byte[] data, byte[] key)
        {
            var config = await _apiConfigurationService.GetApiConfigurationAsync();
            var provider = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
            var symeticKey = provider.CreateSymmetricKey(key);
            return WinRTCrypto.CryptographicEngine.Encrypt(symeticKey, data, config.InitialisationVector);//add config.InitialisationVector
        }

        public async Task<byte[]> DecryptAsync(byte[] data, byte[] key)
        {
            var config = await _apiConfigurationService.GetApiConfigurationAsync();
            var provider = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
            var symeticKey = provider.CreateSymmetricKey(key);
            return WinRTCrypto.CryptographicEngine.Decrypt(symeticKey, data, config.InitialisationVector);
        }

        public async Task<byte[]> GeneratePasswortAsync(string password)
        {
            var infos = await _apiConfigurationService.GetApiConfigurationAsync();
            return await Task.Run(() => NetFxCrypto.DeriveBytes.GetBytes(password, infos.GenerationSalt, infos.GenerationKeyInterations, infos.GenerationKeyLenghtInBytes));
        }
    }
}
