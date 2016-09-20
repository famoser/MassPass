using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Content.Models.Base;
using Famoser.MassPass.View.Models;
using Famoser.MassPass.View.Models.Base;

namespace Famoser.MassPass.View.Content.Interfaces
{
    public interface IViewContentModelProvider<T1, T2>
        where T1 : BaseContentModel
        where T2 : ViewContentModel
    {
        void SaveValues(T1 target, T2 source);
        T2 GetViewContentModel(T1 model);
    }
}
