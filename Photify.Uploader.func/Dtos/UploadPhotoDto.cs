using System.Collections.Generic;

namespace Photify.Uploader.Dtos
{
    public class UploadPhotoDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; } = new();
        public string Photo { get; set; }
    }
}
