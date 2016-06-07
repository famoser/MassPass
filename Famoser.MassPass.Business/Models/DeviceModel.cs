using System;
using Famoser.MassPass.Business.Models.Base;

namespace Famoser.MassPass.Business.Models
{
    public class DeviceModel : BaseModel
    {
        public Guid DeviceId { get; set; }

        private string _deviceName;
        public string DeviceName
        {
            get { return _deviceName; }
            set { Set(ref _deviceName, value); }
        }

        private DateTime _lastRequestDateTime;
        public DateTime LastRequestDateTime
        {
            get { return _lastRequestDateTime; }
            set { Set(ref _lastRequestDateTime, value); }
        }

        private DateTime _lastModificationDateTime;
        public DateTime LastModificationDateTime
        {
            get { return _lastModificationDateTime; }
            set { Set(ref _lastModificationDateTime, value); }
        }

        private DateTime _authorizationDateTime;
        public DateTime AuthorizationDateTime
        {
            get { return _authorizationDateTime; }
            set { Set(ref _authorizationDateTime, value); }
        }
    }
}
