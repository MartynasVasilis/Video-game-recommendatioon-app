using GamePicker.Application;
using GamePicker.Domain;
using Microsoft.EntityFrameworkCore;

namespace GamePicker.Infrastructure;

public sealed class GamePickerDbContext(
    DbContextOptions<GamePickerDbContext> options,
    ICurrentUser currentUser) : DbContext(options)
{
    public DbSet<Game> Games => Set<Game>();
    public DbSet<User> Users => Set<User>();
    public DbSet<LibraryEntry> LibraryEntries => Set<LibraryEntry>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Game>().HasData(SampleData.Games);
        model.Entity<User>(user =>
        {
            user.HasQueryFilter(u => u.Id == currentUser.UserId);
            user.HasData(SampleData.Users);
        });

        model.Entity<LibraryEntry>(entry =>
        {
            entry.HasKey(e => new { e.UserId, e.GameId });
            entry.HasOne<User>().WithMany().HasForeignKey(e => e.UserId);
            entry.HasOne<Game>().WithMany().HasForeignKey(e => e.GameId);
            entry.Property(e => e.Status).HasConversion<string>();
            entry.HasQueryFilter(e => e.UserId == currentUser.UserId);
            entry.HasData(SampleData.LibraryEntries);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<LibraryEntry>().Where(e => e.State != EntityState.Unchanged))
            if (entry.Entity.UserId != currentUser.UserId)
                throw new InvalidOperationException("Cannot change the data of another client.");

        return base.SaveChangesAsync(cancellationToken);
    }
}
