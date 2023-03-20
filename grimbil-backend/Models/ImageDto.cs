using System.Reflection.Metadata;

namespace grimbil_backend.Models
{
    public class ImageDto
    {
        public int PostId { get; set; }
        public List<string> Base64Images { get; set; }
    }
}
