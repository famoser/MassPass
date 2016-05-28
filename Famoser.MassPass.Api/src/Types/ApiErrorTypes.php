<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 27.05.2016
 * Time: 13:23
 */

namespace Famoser\MassPass\Types;


class ApiErrorTypes
{
//[Description("No API error occured")]
    const None = 0;

    #region request errors
//[Description("Api Version unknown")]
    const ApiVersionInvalid = 100;

//[Description("Json request could not be deserialized")]
    const RequestJsonFailure = 101;

//[Description("Request could not be processed by the server. This is probably a API error, nothing you can do about it :/")]
    const ServerFailure = 102;

//[Description("Json request could not be deserialized")]
    const RequestUriInvalid = 103;

//[Description("Execution of request was denied")]
    const Forbidden = 104;
    #endregion

    #region general errors
    /// <summary>
    /// DeviceId unknown to the API
    /// </summary>
//[Description("Your device is unknown to the API")]
    const NotAuthorized = 1000;

    /// <summary>
    /// DeviceId was unauthorized
    /// </summary>
//[Description("Your device was unauthorized")]
    const Unauthorized = 1001;

    /// <summary>
    /// Download Uri invalid
    /// </summary>
//[Description("Failed downloading an entry because the download link was invalid or expired")]
    const DownloadUrlInvalid = 1002;
    #endregion

    #region AuthroisationRequest
    /// <summary>
    /// The provided AuthorisationCode is invalid
    /// </summary>
//[Description("Your athorization code is invalid")]
    const AuthorizationCodeInvalid = 2000;
    #endregion

}