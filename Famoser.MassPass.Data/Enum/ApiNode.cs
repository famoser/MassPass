using Famoser.MassPass.Data.Attributes;

namespace Famoser.MassPass.Data.Enum
{
    public enum ApiNode
    {
        [ApiUri("")]
        Index,

        [ApiUri("authorization")]
        Authorization,

        [ApiUri("status", Authorization)]
        AuthorizationStatus,

        [ApiUri("createuser", Authorization)]
        CreateUser,

        [ApiUri("authorize", Authorization)]
        Authorize,

        [ApiUri("createauthorization", Authorization)]
        CreateAuthorization,

        [ApiUri("wipeuser", Authorization)]
        WipeUser,

        [ApiUri("unauthorize", Authorization)]
        UnAuthorize,

        [ApiUri("authorizeddevices", Authorization)]
        AuthorizedDevices,

        [ApiUri("sync")]
        Sync,

        [ApiUri("sync", Sync)]
        SyncContent,

        [ApiUri("update", Sync)]
        Update,

        [ApiUri("readcontententity", Sync)]
        ReadContentEntity,

        [ApiUri("gethistory", Sync)]
        GetHistory
    }
}
