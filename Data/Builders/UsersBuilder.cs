using Microsoft.EntityFrameworkCore;
using plant_api.Models;

namespace plant_api.Data.Builders
{
    public static class UsersBuilder
    {
        public static void buildUsersModel(ModelBuilder modelBuilder)
        {
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
