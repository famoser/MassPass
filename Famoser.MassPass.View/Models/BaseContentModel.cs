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

        public abstract bool CanBeSaved();

        public EventHandler CanBeSavedChanged { get; set; }
    }
}
