using Microsoft.EntityFrameworkCore;
using ResourceApi.Models;

namespace ResourceApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Image> Images => Set<Image>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Puzzle> Puzzles => Set<Puzzle>();
    public DbSet<Pack> Packs => Set<Pack>();
    public DbSet<UserProgress> UserProgress => Set<UserProgress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Image>()
            .HasMany(i => i.Tags)
            .WithMany(t => t.Images);

        modelBuilder.Entity<Puzzle>()
            .HasMany(p => p.Images)
            .WithMany(i => i.Puzzles);

        modelBuilder.Entity<Pack>()
            .HasMany(p => p.Puzzles)
            .WithMany(p => p.Packs);
            
        modelBuilder.Entity<UserProgress>()
            .HasIndex(up => up.UserId);
    }
}
