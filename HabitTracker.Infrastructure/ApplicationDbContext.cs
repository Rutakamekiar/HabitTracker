using HabitTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<RecordEntity> Records { get; set; }
    
    public DbSet<TelegramUserEntity> TelegramUsers { get; set; }
    
    public DbSet<CategoryEntity> Categories { get; set; }
}