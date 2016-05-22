using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Enum
{
    public enum RequestType
    {
        Authorize,
        UnAuthorize,
        Refresh,
        Update,
        ReadContentEntity,
        ReadCollectionEntries,
        GetHistory
    }
}
