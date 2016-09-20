using System;
using System.Collections.Generic;
using System.Linq;
using Famoser.MassPass.Business.Models.Base;
using Famoser.MassPass.View.Models.Interfaces;

namespace Famoser.MassPass.View.Models.Base
{
    public abstract class ViewContentModel : BaseModel, ICustomContentModel
    {
        private readonly Dictionary<string, string> _startValueDic = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _currentValueDic = new Dictionary<string, string>();
        protected void RegisterValueChange(string key, string value)
        {
            if (!_startValueDic.ContainsKey(key))
                _startValueDic[key] = value;
            _currentValueDic[key] = value;

            CanBeSavedChanged?.Invoke(this, EventArgs.Empty);
        }
        
        public bool ContentChanged()
        {
            return _startValueDic.Any(s => _currentValueDic[s.Key] != s.Value);
        }


        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (Set(ref _name, value))
                    RegisterValueChange("Name", _name);
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                if (Set(ref _description, value))
                    RegisterValueChange("Description", _description);
            }
        }

        public virtual bool CanBeSaved()
        {
            return !string.IsNullOrEmpty(Name);
        }

        public EventHandler CanBeSavedChanged { get; set; }
    }
}
