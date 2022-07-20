using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinkedHU_CENG.Models
{
    public class Application
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ApplicationId { get; set; }
        public int AdvertisementId { get; set; }
        [Display(Name = "Title")]
        public string? AdvertisementTitle { get; set; }
        [Display(Name = "Company")]
        public string? Company { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNum { get; set; }
        public string? UserLocation { get; set; }
        public string? UserAbout { get; set; }
        public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        [Display(Name = "Resume")]
        [NotMapped]
        public IFormFile? Resume { get; set; }
        public string? ResumePath { get; set; }
        public string? UserProfilePicture { get; set; }
        [Display(Name = "Certificate")]
        [NotMapped]
        public IFormFile? Certificate1 { get; set; }
        public string? Certificate1Path { get; set; }
        [Display(Name = "Certificate")]
        [NotMapped]
        public IFormFile? Certificate2 { get; set; }
        public string? Certificate2Path { get; set; }
        [Display(Name = "Certificate")]
        [NotMapped]
        public IFormFile? Certificate3 { get; set; }
        public string? Certificate3Path { get; set; }
    }
}
