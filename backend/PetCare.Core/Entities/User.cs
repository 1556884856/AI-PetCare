using PetCare.Core.Enums;
namespace PetCare.Core.Entities;

public class User
{
    public int Id { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Customer;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Pet> Pets { get; set; } = new List<Pet>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<UserCoupon> UserCoupons { get; set; } = new List<UserCoupon>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
