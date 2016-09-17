using System;
using Famoser.MassPass.Data.Enum;

namespace Famoser.MassPass.Data.Models
{
    public class ContentApiInformations
    {
        /// <summary>
        /// Id of the element on server
        /// </summary>
        public Guid ServerId { get; set; }

        /// <summary>
        /// Id of the collection on server
        /// </summary>
        public Guid ServerCollectionId { get; set; }
        
        /// <summary>
        /// version 
        /// </summary>
        public string VersionId { get; set; }

        /// <summary>
        /// status of the entity on server (new, updated, up to date)
        /// </summary>
        public ServerStatus ServerStatus { get; set; }
    }
}
