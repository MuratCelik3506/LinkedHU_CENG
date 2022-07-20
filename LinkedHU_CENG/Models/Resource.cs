using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinkedHU_CENG.Models
{
    public class Resource
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResourceId { get; set; }

        public int? UserId { get; set; }

        [Required(ErrorMessage = "Field cannot be left blank")]
        [StringLength(50, ErrorMessage = "The value entered must be between {2} characters and {1} characters", MinimumLength = 1)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        [Required(ErrorMessage = "Field cannot be left blank")]
        [Display(Name = "File Upload (maximum size 28mb)")]
        [NotMapped]
        public IFormFile UploadedFile { get; set; }

        public string? ResourceImageName { get; set; }

        public string? ResourceVideoName { get; set; }

        public string? ResourcePdfName { get; set; }

        public string? ResourceWordName { get; set; }

        public string? ResourceExelName { get; set; }
        public string? ResourcePointName { get; set; }

    }
}
