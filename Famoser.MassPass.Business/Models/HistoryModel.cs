using System;
using Famoser.MassPass.Business.Models.Base;
using Famoser.MassPass.Business.Models.Sync;

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

        private BaseSyncModel _baseSyncModel;
        public BaseSyncModel BaseSyncModel
        {
            get { return _baseSyncModel; }
            set { Set(ref _baseSyncModel, value); }
        }

        public DateTime CreationDateTime { get; set; }
        public string VersionId { get; set; }
    }
}
