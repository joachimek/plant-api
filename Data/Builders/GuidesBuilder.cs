using Microsoft.EntityFrameworkCore;
using plant_api.Models;

namespace plant_api.Data.Builders
{
    public static class GuidesBuilder
    {
        public static void buildGuidesModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Guides>()
               .HasOne(x => x.Species)
               .WithMany(x => x.Guides);

            modelBuilder.Entity<Guides>()
               .HasOne(x => x.User)
               .WithMany(x => x.Guides);

            modelBuilder.Entity<Guides>()
                .HasMany(x => x.Reviews)
                .WithOne(x => x.Guide)
                .HasForeignKey(x => x.GuideID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Guides>()
                .HasMany(x => x.Plants)
                .WithOne(x => x.Guide)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
