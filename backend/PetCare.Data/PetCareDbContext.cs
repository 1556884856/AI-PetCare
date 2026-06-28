using Microsoft.EntityFrameworkCore;
using PetCare.Core.Entities;

namespace PetCare.Data;

/// <summary>
/// 宠物护理系统数据库上下文，定义所有实体的 DbSet 和关系映射配置。
/// 使用 SQL Server 作为数据库提供程序。
/// </summary>
public class PetCareDbContext : DbContext
{
    public PetCareDbContext(DbContextOptions<PetCareDbContext> options) : base(options) { }

    // ===== 实体数据集 =====
    public DbSet<User> Users => Set<User>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<Core.Entities.Service> Services => Set<Core.Entities.Service>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<UserCoupon> UserCoupons => Set<UserCoupon>();
    public DbSet<Payment> Payments => Set<Payment>();

    /// <summary>
    /// 配置实体关系、索引和全局查询过滤器。
    /// 通过 Fluent API 配置所有关联关系和级联删除行为。
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User：手机号唯一索引
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Phone).IsUnique();
        });

        // Pet：全局查询过滤器（隐藏已删除宠物），一对多关联
        modelBuilder.Entity<Pet>(e =>
        {
            e.HasQueryFilter(p => !p.IsDeleted);
            e.HasOne(p => p.User).WithMany(u => u.Pets).HasForeignKey(p => p.UserId);
        });

        // Appointment：多对一关联（User、Pet、Service），外键删除行为为 Restrict
        modelBuilder.Entity<Appointment>(e =>
        {
            e.HasOne(a => a.User).WithMany(u => u.Appointments).HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(a => a.Pet).WithMany(p => p.Appointments).HasForeignKey(a => a.PetId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(a => a.Service).WithMany(s => s.Appointments).HasForeignKey(a => a.ServiceId).OnDelete(DeleteBehavior.Restrict);
        });

        // Review：一对多（User），一对一（Appointment）
        modelBuilder.Entity<Review>(e =>
        {
            e.HasOne(r => r.User).WithMany(u => u.Reviews).HasForeignKey(r => r.UserId);
            e.HasOne(r => r.Appointment).WithOne(a => a.Review).HasForeignKey<Review>(r => r.AppointmentId);
        });

        // Notification：复合索引加速未读查询
        modelBuilder.Entity<Notification>(e =>
        {
            e.HasOne(n => n.User).WithMany(u => u.Notifications).HasForeignKey(n => n.UserId);
            e.HasIndex(n => new { n.UserId, n.IsRead });
        });

        // Coupon：IsActive 索引加速筛选
        modelBuilder.Entity<Coupon>(e =>
        {
            e.HasIndex(c => c.IsActive);
        });

        // UserCoupon：复合唯一索引，防止重复领取
        modelBuilder.Entity<UserCoupon>(e =>
        {
            e.HasOne(uc => uc.User).WithMany(u => u.UserCoupons).HasForeignKey(uc => uc.UserId);
            e.HasOne(uc => uc.Coupon).WithMany(c => c.UserCoupons).HasForeignKey(uc => uc.CouponId);
            e.HasIndex(uc => new { uc.UserId, uc.CouponId });
        });

        // Payment：一对多（User），一对一（Appointment），优惠券外键为 SetNull
        modelBuilder.Entity<Payment>(e =>
        {
            e.HasOne(p => p.User).WithMany(u => u.Payments).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.Appointment).WithOne().HasForeignKey<Payment>(p => p.AppointmentId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.Coupon).WithMany().HasForeignKey(p => p.CouponId).OnDelete(DeleteBehavior.SetNull);
        });
    }
}
