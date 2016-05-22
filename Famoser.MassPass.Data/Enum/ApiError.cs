using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Enum
{
    public enum ApiError
    {
        None = 1,

        #region general errors
        /// <summary>
        /// DeviceId unknown to the API 
        /// </summary>
        NotAuthorized = 1000,

        /// <summary>
        /// DeviceId was unauthorized 
        /// </summary>
        Unauthorized = 1001,

        /// <summary>
        /// UserId unknown to the API
        /// </summary>
        UserNotFound = 1002,

        /// <summary>
        /// Download Uri invalid
        /// </summary>
        DownloadUrlInvalid = 1003,
        #endregion

        #region AuthroisationRequest
        /// <summary>
        /// The provided AuthorisationCode is invalid
        /// </summary>
        AuthorizationCodeInvalid = 2000
        #endregion
    }
}
