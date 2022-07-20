using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LinkedHU_CENG.Models
{
    public class Administrator
    {
        [Key]
        [Required(ErrorMessage = "Username field is required.")]
        public string UserName { get; set; }


        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
