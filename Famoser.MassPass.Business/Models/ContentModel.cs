﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.MassPass.Business.Models
{
    public class ContentModel : CollectionModel
    {
        public ContentModel(Guid id, Guid typeId, Guid serverRelationId, CollectionModel parent) : base(id, typeId, serverRelationId, parent)
        {

        }

        private string _contentJson;
        public string ContentJson
        {
            get { return _contentJson; }
            set { Set(ref _contentJson, value); }
        }

        private byte[] _contentFile;
        public byte[] ContentFile
        {
            get { return _contentFile; }
            set { Set(ref _contentFile, value); }
        }
    }
}
