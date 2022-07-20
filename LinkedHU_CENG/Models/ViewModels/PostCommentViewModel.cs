
namespace LinkedHU_CENG.Models.ViewModels
{
    public class PostCommentViewModel
    {
        public Post post { get; set; }

        public int postId { get; set; }

        public List<Comment> comments { get; set; }

        public Comment comment { get; set; }
    }
}
