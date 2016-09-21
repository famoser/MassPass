using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Content.Models.Base;
using Famoser.MassPass.Business.Content.Providers.Interfaces;
using Famoser.MassPass.View.Enums;
using Famoser.MassPass.View.Models;
using Famoser.MassPass.View.Models.Base;

namespace Famoser.MassPass.View.Content.Interfaces
{
    public interface IViewContentModelProvider : IContentModelProvider
    {
        void SaveValues(BaseContentModel target, ViewContentModel source);
        ViewContentModel GetViewContentModel(BaseContentModel model);

        PageKeys GetPageKey();
    }
}
