using Microsoft.EntityFrameworkCore;
using plant_api.Models;

namespace plant_api.Data.Builders
{
    public static class DevicesBuilder
    {
        public static void buildDevicesModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Devices>()
                .HasOne(x => x.Plant)
                .WithOne(x => x.Device)
                .HasForeignKey<Plants>(x => x.DeviceID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Devices>()
               .HasOne(x => x.User)
               .WithMany(x => x.Devices);
        }
    }
}
