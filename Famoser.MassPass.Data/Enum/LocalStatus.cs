using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Enum
{
    /// <summary>
    /// indicates if the local file has been changed since the last sync, or has conflicts with server versions
    /// </summary>
    public enum LocalStatus
    {
        UpToDate = 1,
        Changed = 2,
        Conflict = 3,
    }
}
