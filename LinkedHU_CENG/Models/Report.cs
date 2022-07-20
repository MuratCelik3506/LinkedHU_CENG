using System.ComponentModel.DataAnnotations;

namespace LinkedHU_CENG.Models
{
    public class Report
    {
        [Key]
        public int ReportId { get; set; }

        [Required(ErrorMessage = "Field cannot be left blank")]
        [StringLength(150, ErrorMessage = "The value entered must be between {2} characters and {1} characters", MinimumLength = 1)]
        public string? ReportUserName { get; set; }

        public int UserId { get; set; } = 0;

        public string? Detail { get; set; } 

        public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");



    }
}
