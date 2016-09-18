using System;
using Famoser.MassPass.Data.Enum;

namespace Famoser.MassPass.Data.Models
{
    public class ContentApiInformations
    {
        /// <summary>
        /// ContentId of the element on server
        /// </summary>
        public Guid ContentId { get; set; }

        /// <summary>
        /// ContentId of the collection on server
        /// </summary>
        public Guid CollectionId { get; set; }
        
        /// <summary>
        /// version 
        /// </summary>
        public string VersionId { get; set; }

        /// <summary>
        /// status of the entity on server (new, updated, up to date)
        /// </summary>
        public ServerVersion ServerVersion { get; set; }
    }
}
