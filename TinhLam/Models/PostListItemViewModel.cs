namespace TinhLam.Models
{
    public class PostListItemViewModel
    {
        public int PostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int ViewCount { get; set; }
        public string Image { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        public int CommentCount { get; set; }
    }
}
