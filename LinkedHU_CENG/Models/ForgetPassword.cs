using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinkedHU_CENG.Models
{
    public class ForgetPassword
    {


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required, Key]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email is not valid")]
        public string Email { get; set; }


        [Display(Name = "Name")]
        public string Name { get; set; } = "John";

        [Display(Name = "Surname")]
        public string Surname { get; set; } = "Doe";

        public string RequestTime { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");


    }
}
