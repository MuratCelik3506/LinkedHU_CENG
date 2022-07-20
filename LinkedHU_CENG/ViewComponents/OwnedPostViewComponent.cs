using Microsoft.AspNetCore.Mvc;
using LinkedHU_CENG.Models;
using Microsoft.EntityFrameworkCore;
using LinkedHU_CENG.Models.ViewModels;

namespace LinkedHU_CENG.ViewComponents
{
    [ViewComponent(Name = "OwnedPostViewComponent")] //Solution
    public class OwnedPostViewComponent : ViewComponent
    {

        private readonly ApplicationDbContext _db;

        public OwnedPostViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {

            IEnumerable<Post> mc = await _db.Posts.ToListAsync();
            List<PostCommentViewModel> viewModels = new List<PostCommentViewModel>();

            foreach (Post post in mc)
            {
                PostCommentViewModel viewModel = new PostCommentViewModel();
                List<Comment> postComments = await _db.Comments.Where(s => s.PostId == post.PostId).ToListAsync();
                viewModel.comments = postComments;
                viewModel.post = post;
                viewModel.postId = post.PostId;

                viewModels.Add(viewModel);
            }

            ViewData["SessionUserId"] = HttpContext.Session.GetInt32("UserID");
            return View(viewModels);
        }
    }
}
