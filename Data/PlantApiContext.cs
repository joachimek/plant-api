using Microsoft.EntityFrameworkCore;
using plant_api.Data.Builders;
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
            EntityBuilder.buildEntities(modelBuilder);

            DevicesBuilder.buildDevicesModel(modelBuilder);
            GuidesBuilder.buildGuidesModel(modelBuilder);
            PlantsBuilder.buildPlantsModel(modelBuilder);
            PlantsHistBuilder.buildPlantsHistModel(modelBuilder);
            SpeciesBuilder.buildSpeciesModel(modelBuilder);
            UsersBuilder.buildUsersModel(modelBuilder);
        }
    }
}
