using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinkedHU_CENG.Models
{
    public class Advertisement
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdvertisementId { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        [Required(ErrorMessage = "Field cannot be left blank")]
        [StringLength(300, ErrorMessage = "The value entered must be between {2} characters and {1} characters", MinimumLength = 1)]
        [Display(Name = "Title")]
        public string? Title { get; set; }

        [Required]
        [Display(Name = "Type")]
        public string? Type { get; set; }

        [Required]
        [Display(Name = "Company")]
        public string? Company { get; set; }

        [Required]
        [Display(Name = "Location")]
        public string? Location { get; set; }

        [Required(ErrorMessage = "Field cannot be left blank")]
        [StringLength(300, ErrorMessage = "The value entered must be between {2} characters and {1} characters", MinimumLength = 1)]
        [Display(Name = "Description")]
        public string? Description { get; set; }
        public bool? IsResume { get; set; }
        public bool? IsCertificates { get; set; }
        public bool IsActive { get; set; }
    }

    public enum Type
    {
        Internship,
        Job,
        Scholarship
    }
}
