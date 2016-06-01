﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Models;

namespace Famoser.MassPass.Business.Repositories.Interfaces
{
    public interface IContentRepository
    {
        Task<bool> FillValues(ContentModel model);
        Task<bool> FillHistory(ContentModel model);
    }
}
