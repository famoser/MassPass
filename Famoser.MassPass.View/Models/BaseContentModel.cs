using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Models.Base;
using Famoser.MassPass.View.Models.Interfaces;

namespace Famoser.MassPass.View.Models
{
    public abstract class BaseContentModel : BaseModel, ICustomContentModel
    {
        protected void RaiseCanBeSaved()
        {
            CanBeSavedChanged?.Invoke(this, EventArgs.Empty);
        }

        private readonly Dictionary<string, string> _startValueDic = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _currentValueDic = new Dictionary<string, string>();
        protected void RegisterValueChange(string key, string value)
        {
            if (!_startValueDic.ContainsKey(key))
                _startValueDic[key] = value;
            _currentValueDic[key] = value;
            
            RaiseCanBeSaved();
        }

        public abstract bool CanBeSaved();
        public bool ContentChanged()
        {
            return _startValueDic.Any(s => _currentValueDic[s.Key] != s.Value);
        }

        public EventHandler CanBeSavedChanged { get; set; }
    }
}
