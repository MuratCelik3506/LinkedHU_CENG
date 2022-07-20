using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinkedHU_CENG.Models
{
    public class Announcement
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnnouncementId { get; set; }

        public int? UserId { get; set; }

        public string? UserName { get; set; }

        public string? UserProfilePicture { get; set; }

        public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        [Required(ErrorMessage = "Field cannot be left blank")]
        [StringLength(500, ErrorMessage = "The value entered must be between {2} characters and {1} characters", MinimumLength = 1)]
        [Display(Name = "Content")]
        public string Content { get; set; }
    }
}
