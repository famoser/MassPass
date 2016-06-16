using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.MassPass.Business.Enums
{
    public enum ContentTypes
    {
        /// <summary>
        /// A folder which has no precessor
        /// </summary>
        Root,

        /// <summary>
        /// A folder (only collections are important)
        /// </summary>
        Folder,

        /// <summary>
        /// A Note
        /// </summary>
        Note,

        /// <summary>
        /// An unknown Content Type
        /// </summary>
        Unknown
    }
}
