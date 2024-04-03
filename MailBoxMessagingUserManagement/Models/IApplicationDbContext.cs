using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MailBoxMessagingUserManagement.Models
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Message> Messages { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
