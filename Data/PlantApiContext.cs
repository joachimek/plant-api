using Microsoft.EntityFrameworkCore;
using plant_api.Models;

namespace plant_api.Data
{
    public class PlantApiContext : DbContext
    {
        public PlantApiContext (DbContextOptions<PlantApiContext> options) : base(options) { }

        public DbSet<PlantsHist>? ApiActions { get; set; }
        public DbSet<Devices>? Devices { get; set; }
        public DbSet<Guides>? Guides { get; set; }
        public DbSet<Plants>? Plants { get; set; }
        public DbSet<Reviews>? Reviews { get; set; }
        public DbSet<SpeciesDto>? Species { get; set; }
        public DbSet<Users>? Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlantsHist>().ToTable("PlantsHist");
            modelBuilder.Entity<Devices>().ToTable("Devices");
            modelBuilder.Entity<Guides>().ToTable("Guides");
            modelBuilder.Entity<Plants>().ToTable("Plants");
            modelBuilder.Entity<Reviews>().ToTable("Reviews");
            modelBuilder.Entity<SpeciesDto>().ToTable("Species");
            modelBuilder.Entity<Users>().ToTable("Users");

            modelBuilder.Entity<Devices>()
                .HasOne(x => x.Plant)
                .WithOne(x => x.Device)
                .HasForeignKey<Plants>(x => x.DeviceID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Guides>()
                .HasMany(x => x.Reviews)
                .WithOne(x => x.Guide)
                .HasForeignKey(x => x.GuideID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Guides>()
                .HasMany(x => x.Plants)
                .WithOne(x => x.Guide)
                .HasForeignKey(x => x.GuideID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Plants>()
                .HasMany(x => x.PlantsHists)
                .WithOne(x => x.Plant)
                .HasForeignKey(x => x.PlantID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SpeciesDto>()
                .HasMany(x => x.Guides)
                .WithOne(x => x.Species)
                .HasForeignKey(x => x.SpeciesID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SpeciesDto>()
               .HasMany(x => x.Plants)
               .WithOne(x => x.Species)
               .HasForeignKey(p => p.SpeciesID)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Users>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Users>()
                .HasMany(x => x.Devices)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Users>()
               .HasMany(x => x.Guides)
               .WithOne(x => x.User)
               .HasForeignKey(x => x.UserID)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Users>()
                .HasMany(x => x.Reviews)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserID)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
