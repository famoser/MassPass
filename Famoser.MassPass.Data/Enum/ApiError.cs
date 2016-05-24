using Famoser.FrameworkEssentials.Attributes;

namespace Famoser.MassPass.Data.Enum
{
    public enum ApiError
    {
        [Description("No API error occured")]
        None = 0,

        #region general errors
        /// <summary>
        /// DeviceId unknown to the API 
        /// </summary>
        [Description("Your device is unknown to the API")]
        NotAuthorized = 1000,

        /// <summary>
        /// DeviceId was unauthorized 
        /// </summary>
        [Description("Your device was unauthorized")]
        Unauthorized = 1001,

        /// <summary>
        /// Download Uri invalid
        /// </summary>
        [Description("Failed downloading an entry because the download link was invalid or expired")]
        DownloadUrlInvalid = 1002,
        #endregion

        #region AuthroisationRequest
        /// <summary>
        /// The provided AuthorisationCode is invalid
        /// </summary>
        [Description("Your athorization code is invalid")]
        AuthorizationCodeInvalid = 2000
        #endregion
    }
}
