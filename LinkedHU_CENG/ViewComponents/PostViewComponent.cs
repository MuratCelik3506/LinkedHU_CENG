using Microsoft.AspNetCore.Mvc;
using LinkedHU_CENG.Models;
using Microsoft.EntityFrameworkCore;
using LinkedHU_CENG.Models.ViewModels;
using System.Linq;

namespace LinkedHU_CENG.ViewComponents
{
    [ViewComponent(Name = "PostViewComponent")] //Solution
    public class PostViewComponent : ViewComponent
    {

        private readonly ApplicationDbContext _db;

        public PostViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {

            List<Post> posts = await _db.Posts.ToListAsync();
            List<Follow> follows = await _db.Follows.Where(f => f.FollowerId == HttpContext.Session.GetInt32("UserID")).ToListAsync();
            List<PostCommentViewModel> viewModels = new List<PostCommentViewModel>();
            List<int> followingIds = new List<int>();

            foreach (Follow follow in follows)
            {
                followingIds.Add(follow.FollowingId);
            }

            foreach (Post post in posts)
            {
                if (followingIds.Contains((int)post.UserId) || post.UserId == HttpContext.Session.GetInt32("UserID"))
                {
                    PostCommentViewModel viewModel = new PostCommentViewModel();
                    List<Comment> postComments = await _db.Comments.Where(s => s.PostId == post.PostId).ToListAsync();
                    viewModel.comments = postComments;
                    viewModel.post = post;
                    viewModel.postId = post.PostId;

                    viewModels.Add(viewModel);
                }
            }

            ViewData["SessionUserId"] = HttpContext.Session.GetInt32("UserID");
            return View(viewModels);
        }
    }
}
