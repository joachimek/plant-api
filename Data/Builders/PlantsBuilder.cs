using Microsoft.EntityFrameworkCore;
using plant_api.Models;

namespace plant_api.Data.Builders
{
    public static class PlantsBuilder
    {
        public static void buildPlantsModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Plants>()
               .HasOne(x => x.Species)
               .WithMany(x => x.Plants);

            modelBuilder.Entity<Plants>()
               .HasOne(x => x.Guide)
               .WithMany(x => x.Plants)
               .HasForeignKey(x => x.GuideID);

            modelBuilder.Entity<Plants>()
                .HasMany(x => x.PlantsHists)
                .WithOne(x => x.Plant)
                .HasForeignKey(x => x.PlantID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
