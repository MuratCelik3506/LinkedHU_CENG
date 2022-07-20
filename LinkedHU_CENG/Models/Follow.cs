using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinkedHU_CENG.Models
{
    public class Follow
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FollowId { get; set; }

        [Required]
        [Display(Name = "Follower")]
        public int FollowerId { get; set; }

        [Required]
        [Display(Name = "Following")]
        public int FollowingId { get; set; }
    }
}
