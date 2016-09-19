﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Models.Content.Base;

namespace Famoser.MassPass.Business.Content.Providers.Interfaces
{
    public interface IContentModelProvider
    {
        void WriteValues(BaseContentModel target, BaseContentModel source);
        BaseContentModel Deserialize(string json);
        bool CanDeserialize(string json);
        string Serialize(BaseContentModel model);
        string GetListName();
        bool ShowAsList();
    }
}
