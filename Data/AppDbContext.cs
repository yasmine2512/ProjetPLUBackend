using Microsoft.EntityFrameworkCore;
namespace MyBlazorApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Memoire> Memoires { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Composite key for Favorites
        modelBuilder.Entity<Favorite>()
            .HasKey(f => new { f.UserID, f.MemoireID });

        // User → Memoires (as Professor)
        modelBuilder.Entity<Memoire>()
            .HasOne(m => m.Professor)
            .WithMany(u => u.PostedMemoires)
            .HasForeignKey(m => m.ProfessorID)
            .OnDelete(DeleteBehavior.Cascade);

        // Favorites
        modelBuilder.Entity<Favorite>()
            .HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Favorite>()
            .HasOne(f => f.Memoire)
            .WithMany(m => m.Favorites)
            .HasForeignKey(f => f.MemoireID)
            .OnDelete(DeleteBehavior.Cascade);

        // Comments
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Memoire)
            .WithMany(m => m.Comments)
            .HasForeignKey(c => c.MemoireID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
