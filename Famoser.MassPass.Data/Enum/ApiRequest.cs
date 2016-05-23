using Famoser.MassPass.Data.Attributes;

namespace Famoser.MassPass.Data.Enum
{
    public enum ApiRequest
    {
        [ApiUri("")]
        Index,

        [ApiUri("authorization")]
        Authorization,

        [ApiUri("authorize", Authorization)]
        Authorize,

        [ApiUri("unauthorize", Authorization)]
        UnAuthorize,

        [ApiUri("authorizeddevices", Authorization)]
        AuthorizedDevices,

        [ApiUri("sync")]
        Sync,

        [ApiUri("refresh", Sync)]
        Refresh,

        [ApiUri("update", Sync)]
        Update,

        [ApiUri("readcontententity", Sync)]
        ReadContentEntity,

        [ApiUri("readcollectionentries", Sync)]
        ReadCollectionEntries,

        [ApiUri("gethistory", Sync)]
        GetHistory
    }
}
