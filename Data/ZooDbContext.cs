using DierentuinOpdracht.Models;
using Microsoft.EntityFrameworkCore;

namespace DierentuinOpdracht.Data
{
    public class ZooDbContext : DbContext
    {
        public ZooDbContext(DbContextOptions<ZooDbContext> options)
            : base(options)
        {
        }

        public DbSet<Animal> Animals { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Enclosure> Enclosures { get; set; }
        public DbSet<Zoo> Zoos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Animal>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Animals)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Animal>()
                .HasOne(a => a.Enclosure)
                .WithMany(e => e.Animals)
                .HasForeignKey(a => a.EnclosureId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Enclosure>()
                .HasOne(e => e.Zoo)
                .WithMany(z => z.Enclosures)
                .HasForeignKey(e => e.ZooId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
