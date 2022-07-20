using Microsoft.EntityFrameworkCore;

namespace LinkedHU_CENG.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<UnregisteredUser> UnregisteredUsers { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<BannedUser> BannedUsers { get; set; }
        public DbSet<DeleteRequest> DeleteRequests { get; set; }
        public DbSet<ForgetPassword> ForgetPasswords { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<MergeEmailRequest> MergeEmailRequests { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Follow> Follows { get; set; }
    }
}
