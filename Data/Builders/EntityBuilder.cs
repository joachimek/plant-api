using Microsoft.EntityFrameworkCore;
using plant_api.Models;

namespace plant_api.Data.Builders
{
    public static class EntityBuilder
    {
        public static void buildEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlantsHist>().ToTable("PlantsHist");
            modelBuilder.Entity<Devices>().ToTable("Devices");
            modelBuilder.Entity<Guides>().ToTable("Guides");
            modelBuilder.Entity<Plants>().ToTable("Plants");
            modelBuilder.Entity<Reviews>().ToTable("Reviews");
            modelBuilder.Entity<SpeciesDto>().ToTable("Species");
            modelBuilder.Entity<Users>().ToTable("Users");
        }
    }
}
