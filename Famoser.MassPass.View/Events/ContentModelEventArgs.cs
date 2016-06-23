using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Models;

namespace Famoser.MassPass.View.Events
{
    public class ContentModelEventArgs : EventArgs
    {
        public ContentModelEventArgs(ContentModel contentModel)
        {
            ContentModel = contentModel;
        }

        public ContentModel ContentModel { get; }

    }
}
