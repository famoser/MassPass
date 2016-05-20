using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Enum
{
    /// <summary>
    /// response from the server; indicates if contentmodel is up to date or not
    /// </summary>
    public enum RemoteStatus
    {
        UpToDate = 1,
        Changed = 2
    }
}
