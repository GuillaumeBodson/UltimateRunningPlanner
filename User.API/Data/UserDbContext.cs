using Microsoft.EntityFrameworkCore;
using ToolBox.EntityFramework;
using User.API.Data.Models;

namespace User.API.Data;

public class UserDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Models.User> Users { get; set; } = null!;
    public DbSet<Athlete> Athletes { get; set; } = null!;
    public DbSet<AthletePreferences> AthletePreferences { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure relationships
        modelBuilder.Entity<Models.User>()
            .HasOne(u => u.Athlete)
            .WithOne(a => a.User)
            .HasForeignKey<Athlete>(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Models.User>()
            .HasOne(u => u.AthletePreferences)
            .WithOne(ap => ap.User)
            .HasForeignKey<AthletePreferences>(ap => ap.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure HashSet<Dictionary<DayOfWeek, RunType?>> serialization
        modelBuilder.Entity<Athlete>()
            .Property(a => a.TrainingTemplates)
            .UseJsonConversion();
    }
}
