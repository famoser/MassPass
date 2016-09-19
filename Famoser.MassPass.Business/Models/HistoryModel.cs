using System;
using Famoser.MassPass.Business.Models.Base;

namespace Famoser.MassPass.Business.Models
{
    public class HistoryModel : BaseModel
    {
        private DeviceModel _deviceModel;
        public DeviceModel DeviceModel
        {
            get { return _deviceModel; }
            set { Set(ref _deviceModel, value); }
        }

        public DateTime CreationDateTime { get; set; }
        public string VersionId { get; set; }
    }
}
