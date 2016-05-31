using System;

namespace Famoser.MassPass.Data.Models
{
    public class ApiConfiguration
    {
        public Uri Uri { get; set; }
        public int Version { get; set; }
        public byte[] GenerationSalt { get; set; }
        public int GenerationKeyLenghtInBytes { get; set; }
        public int GenerationKeyInterations { get; set; }
        public byte[] InitialisationVector { get; set; }
    }
}
