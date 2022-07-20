using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinkedHU_CENG.Models
{
    public class UnregisteredUser
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email is not valid")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "PhoneNum")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Phone number is not valid")]
        public string PhoneNum { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

        [Required]
        [Display(Name = "BirthDate")]
        [DataType(DataType.Date)]
        public string BirthDate { get; set; }

        [Display(Name = "About Me")]
        public string? About { get; set; }

        [Display(Name = "Location")]
        public string? Location { get; set; }

        [Display(Name = "Second Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email is not valid")]
        public string? SecondEmail { get; set; }

        [Display(Name = "Profile Picture")]
        [NotMapped]
        public IFormFile? ProfilePicture { get; set; }

        public string? ProfilePicturePath { get; set; }

        public string? Title { get; set; }
    }

}
