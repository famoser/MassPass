using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.MassPass.View.Models.Interfaces
{
    public interface ICustomContentModel
    {
        bool CanBeSaved();

        EventHandler CanBeSavedChanged { get; set; }
    }
}
