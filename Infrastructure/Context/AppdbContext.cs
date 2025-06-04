using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Model;
using Domain.Enum;
namespace Infrastructure.Context
{
   public class AppdbContext:DbContext
    {
        public AppdbContext(DbContextOptions<AppdbContext> options) : base(options) { }

        public DbSet<User> users { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingItem> BookItems { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<UserPlan> Userplans { get; set; }
        public DbSet<UserAddress> UserAddress { get;set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(x => x.Role)
                .HasDefaultValue(UserRole.User)
                .HasConversion<string>();
            modelBuilder.Entity<User>()
                .Property(i => i.IsBlocked)
                .HasDefaultValue(false);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Phone)
                .IsUnique();
            modelBuilder.Entity<Plant>()
                .HasIndex(p => new { p.Name, p.CategoryId })
                .IsUnique();
            modelBuilder.Entity<User>()
               .HasMany(u => u.Providers)
               .WithOne(p => p.User)
               .HasForeignKey(p => p.UserId)
               .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.UserAddress)
                .WithMany(a => a.Bookings)
                .HasForeignKey(b => b.UserAddressId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UserAddress>()
                .HasMany(a => a.Bookings)
                .WithOne(b => b.UserAddress)
                .HasForeignKey(b => b.UserAddressId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>()
                .Property(u => u.loginType)
                .HasConversion<string>();
            modelBuilder.Entity<UserPlan>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPlans)
                .HasForeignKey(up => up.UserId);
            modelBuilder.Entity<UserPlan>()
                .HasOne(up => up.Plan)
                .WithMany(p => p.UserPlans)
                .HasForeignKey(up => up.PlanId);
                

            // BookingItem -> Booking (Cascade delete OK)
            modelBuilder.Entity<BookingItem>()
                .HasOne(b => b.Booking)
                .WithMany(bk => bk.BookingItems)
                .HasForeignKey(b => b.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // BookingItem -> Plant (Restrict delete to avoid multiple cascade paths)
            modelBuilder.Entity<BookingItem>()
                .HasOne(b => b.Plant)
                .WithMany()
                .HasForeignKey(b => b.PlantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Plant -> Category (Restrict delete)
            modelBuilder.Entity<Plant>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Plants)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Plant -> Provider (Restrict delete)
            modelBuilder.Entity<Plant>()
                .HasOne(p => p.Provider)
                .WithMany(prov => prov.Plants)
                .HasForeignKey(p => p.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            //// Provider -> User (Restrict delete) - If Provider has User relationship
            //modelBuilder.Entity<Provider>()
            //    .HasOne(prov => prov.User)
            //    .WithMany(u => u.Provider)
            //    .HasForeignKey(prov => prov.UserId)
            //    .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
