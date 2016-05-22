using System.Threading.Tasks;
using Famoser.MassPass.Data.Services.Interfaces;
using PCLCrypto;

namespace Famoser.MassPass.Data.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly IConfigurationService _configurationService;

        public EncryptionService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public async Task<byte[]> Encrypt(byte[] data, byte[] key)
        {
            var config = await _configurationService.GetApiConfiguration();
            var provider = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
            var symeticKey = provider.CreateSymmetricKey(key);
            return WinRTCrypto.CryptographicEngine.Encrypt(symeticKey, data, config.InitialisationVector);//add config.InitialisationVector
        }

        public async Task<byte[]> Decrypt(byte[] data, byte[] key)
        {
            var config = await _configurationService.GetApiConfiguration();
            var provider = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
            var symeticKey = provider.CreateSymmetricKey(key);
            return WinRTCrypto.CryptographicEngine.Decrypt(symeticKey, data, config.InitialisationVector);
        }

        public async Task<byte[]> GeneratePasswort(string password)
        {
            var infos = await _configurationService.GetApiConfiguration();
            return await Task.Run(() => NetFxCrypto.DeriveBytes.GetBytes(password, infos.GenerationSalt, infos.GenerationKeyInterations, infos.GenerationKeyLenghtInBytes));
        }
    }
}
