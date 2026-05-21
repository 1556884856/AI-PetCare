using Microsoft.EntityFrameworkCore;
using PetCare.Core.Entities;

namespace PetCare.Data;

public class PetCareDbContext : DbContext
{
    public PetCareDbContext(DbContextOptions<PetCareDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<Core.Entities.Service> Services => Set<Core.Entities.Service>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<UserCoupon> UserCoupons => Set<UserCoupon>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Phone).IsUnique();
        });

        modelBuilder.Entity<Pet>(e =>
        {
            e.HasQueryFilter(p => !p.IsDeleted);
            e.HasOne(p => p.User).WithMany(u => u.Pets).HasForeignKey(p => p.UserId);
        });

        modelBuilder.Entity<Appointment>(e =>
        {
            e.HasOne(a => a.User).WithMany(u => u.Appointments).HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(a => a.Pet).WithMany(p => p.Appointments).HasForeignKey(a => a.PetId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(a => a.Service).WithMany(s => s.Appointments).HasForeignKey(a => a.ServiceId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Review>(e =>
        {
            e.HasOne(r => r.User).WithMany(u => u.Reviews).HasForeignKey(r => r.UserId);
            e.HasOne(r => r.Appointment).WithOne(a => a.Review).HasForeignKey<Review>(r => r.AppointmentId);
        });

        modelBuilder.Entity<Notification>(e =>
        {
            e.HasOne(n => n.User).WithMany(u => u.Notifications).HasForeignKey(n => n.UserId);
            e.HasIndex(n => new { n.UserId, n.IsRead });
        });

        modelBuilder.Entity<Coupon>(e =>
        {
            e.HasIndex(c => c.IsActive);
        });

        modelBuilder.Entity<UserCoupon>(e =>
        {
            e.HasOne(uc => uc.User).WithMany(u => u.UserCoupons).HasForeignKey(uc => uc.UserId);
            e.HasOne(uc => uc.Coupon).WithMany(c => c.UserCoupons).HasForeignKey(uc => uc.CouponId);
            e.HasIndex(uc => new { uc.UserId, uc.CouponId });
        });

        modelBuilder.Entity<Payment>(e =>
        {
            e.HasOne(p => p.User).WithMany(u => u.Payments).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.Appointment).WithOne().HasForeignKey<Payment>(p => p.AppointmentId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.Coupon).WithMany().HasForeignKey(p => p.CouponId).OnDelete(DeleteBehavior.SetNull);
        });
    }
}
