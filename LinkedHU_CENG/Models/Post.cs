using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinkedHU_CENG.Models
{
    public class Post
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostId { get; set; }

        public int? UserId { get; set; }

        public string? UserName { get; set; }

        public string? UserProfilePicture { get; set; }

        public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        [Required(ErrorMessage = "Field cannot be left blank")]
        [StringLength(240, ErrorMessage = "The value entered must be between {2} characters and {1} characters", MinimumLength = 1)]
        [Display(Name = "Content")]
        public string Content { get; set; }

        [Display(Name = "Post Upload (maximum size 28mb)")]
        [NotMapped]
        public IFormFile? PostUpload { get; set; }

        public string? PostImagePath { get; set; }

        public string? PostVideoPath { get; set; }

        public string? PostPdfPath { get; set; }
    }
}
