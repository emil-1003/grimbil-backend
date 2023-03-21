namespace grimbil_backend.Models
{
    public class PostDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> Pictures { get; set; } = new List<string>();
    }
}
