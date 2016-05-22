using Famoser.MassPass.Data.Attributes;

namespace Famoser.MassPass.Data.Enum
{
    public enum RequestType
    {
        [ApiUri("authorize")]
        Authorize,
        [ApiUri("unauthorize")]
        UnAuthorize,
        [ApiUri("authorizeddevices")]
        AuthorizedDevices,
        [ApiUri("refresh")]
        Refresh,
        [ApiUri("update")]
        Update,
        [ApiUri("readcontententity")]
        ReadContentEntity,
        [ApiUri("readcollectionentries")]
        ReadCollectionEntries,
        [ApiUri("gethistory")]
        GetHistory
    }
}
