using Microsoft.EntityFrameworkCore;
using plant_api.Models;

namespace plant_api.Data.Builders
{
    public static class SpeciesBuilder
    {
        public static void buildSpeciesModel(ModelBuilder modelBuilder)
        {
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
        }
    }
}
