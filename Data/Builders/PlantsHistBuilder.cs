using Microsoft.EntityFrameworkCore;
using plant_api.Models;

namespace plant_api.Data.Builders
{
    public static class PlantsHistBuilder
    {
        public static void buildPlantsHistModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlantsHist>()
               .HasOne(x => x.Plant)
               .WithMany(x => x.PlantsHists);
        }
    }
}
