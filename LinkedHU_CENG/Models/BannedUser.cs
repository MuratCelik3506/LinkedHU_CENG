
using System.ComponentModel.DataAnnotations;

namespace LinkedHU_CENG.Models
{
    public class BannedUser 
    {
        [Key]
        public int UserId { get; set; }

        public string BannedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email is not valid")]
        public string Email { get; set; }
    }
}
