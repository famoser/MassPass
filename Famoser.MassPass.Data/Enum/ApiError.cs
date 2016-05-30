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

        [Description("Execution of request was denied")]
        Forbidden = 104,

        [Description("Some required properties were missing")]
        NotWellDefined = 105,

        [Description("A failure occured on the server while accessing the database")]
        DatabaseFailure = 106,
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
        #endregion

        #region AuthorisationRequest
        /// <summary>
        /// The provided AuthorisationCode is invalid
        /// </summary>
        [Description("Your athorization code is invalid")]
        AuthorizationCodeInvalid = 2000,
        #endregion

        #region UnAuthorisationRequest
        [Description("The device to unauthorize could not be found")]
        DeviceNotFound = 3000,
        #endregion

        #region AuthorizedDevicesRequst
        [Description("No authorized devices could be found")]
        NoDevicesFound = 4000,
        #endregion

        #region ReadContentEntityRequest
        [Description("No authorized devices could be found")]
        ContentNotFound = 5000
        #endregion
    }
}
