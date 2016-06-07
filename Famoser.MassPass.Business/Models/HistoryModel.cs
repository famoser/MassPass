using System;
using Famoser.MassPass.Business.Models.Base;

namespace Famoser.MassPass.Business.Models
{
    public class HistoryModel : BaseModel
    {
        public DeviceModel DeviceModel { get; set; }
        public DateTime CreationDateTime { get; set; }
        public string VersionId { get; set; }
        public ContentModel ContentModel { get; set; }
        public ContentModel Parent { get; set; }
    }
}
