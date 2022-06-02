using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public DbSet<Species>? Species { get; set; }
        public DbSet<Users>? Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlantsHist>().ToTable("PlantsHist");
            modelBuilder.Entity<Devices>().ToTable("Devices");
            modelBuilder.Entity<Guides>().ToTable("Guides");
            modelBuilder.Entity<Plants>().ToTable("Plants");
            modelBuilder.Entity<Reviews>().ToTable("Reviews");
            modelBuilder.Entity<Species>().ToTable("Species");
            modelBuilder.Entity<Users>().ToTable("Users");
        }
    }
}
