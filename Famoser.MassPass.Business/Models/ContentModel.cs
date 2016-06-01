using System;
using Famoser.MassPass.Data.Models;

namespace Famoser.MassPass.Business.Models
{
    public class ContentModel : CollectionModel
    {
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
