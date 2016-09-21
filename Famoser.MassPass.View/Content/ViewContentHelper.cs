using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Content.Enums;
using Famoser.MassPass.Business.Content.Helpers;
using Famoser.MassPass.Business.Content.Models.Base;
using Famoser.MassPass.Business.Content.Providers.Interfaces;
using Famoser.MassPass.View.Content.Interfaces;

namespace Famoser.MassPass.View.Content
{
    public class ViewContentHelper
    {
        public static readonly List<IViewContentModelProvider> ViewContentModelProviders = new List<IViewContentModelProvider>();

        public static void RegisterContentModelProvider(IViewContentModelProvider provider)
        {
            ViewContentModelProviders.Add(provider);
            ContentHelper.RegisterContentModelProvider(provider);
        }
        
        public static IViewContentModelProvider GetProvider(BaseContentModel model)
        {
            return GetProvider(model.ContentType);
        }

        public static IViewContentModelProvider GetProvider(ContentType type)
        {
            foreach (var contentModelProvider in ViewContentModelProviders)
            {
                if (contentModelProvider.GetContentType() == type)
                    return contentModelProvider;
            }
            return null;
        }
    }
}
