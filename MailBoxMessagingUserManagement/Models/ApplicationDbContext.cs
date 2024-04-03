using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MailBoxMessagingUserManagement.Models
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        public ApplicationDbContext() : base()
        {
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
