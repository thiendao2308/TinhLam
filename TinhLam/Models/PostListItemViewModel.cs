namespace TinhLam.Models
{
    public class PostListItemViewModel
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ViewCount { get; set; }
        public string Image { get; set; }
        public string Tags { get; set; }
        public int CommentCount { get; set; }
    }
}
