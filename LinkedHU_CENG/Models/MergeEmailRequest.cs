using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LinkedHU_CENG.Models
{
    public class MergeEmailRequest
    {
        [Key]
        public int UserId { get; set; }

        public string RequestAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

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

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email is not valid")]
        public string SecondEmail { get; set; }
    }
}
