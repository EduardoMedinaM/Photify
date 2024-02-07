using System;
using System.Collections.Generic;

namespace Photify.Uploader.Entities
{
    public class Photo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; } = new();
    }
}
