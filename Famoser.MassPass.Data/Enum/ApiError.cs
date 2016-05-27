using Famoser.FrameworkEssentials.Attributes;

namespace Famoser.MassPass.Data.Enum
{
    public enum ApiError
    {
        [Description("No API error occured")]
        None = 0,

        #region request errors
        [Description("Api Version unknown")]
        ApiVersionInvalid = 100,

        [Description("Json request could not be deserialized")]
        RequestJsonFailure = 101,

        [Description("Request could not be processed by the server. This is probably a API error, nothing you can do about it :/")]
        ServerFailure = 102,

        [Description("Request could not be processed by the server. The endpoint was not found. This is probably an application error, nothing you can do about it :/")]
        RequestUriInvalid = 103,
        #endregion

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
