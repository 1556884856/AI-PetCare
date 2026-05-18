namespace PetCare.Core.Entities;

public class Appointment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PetId { get; set; }
    public int ServiceId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string TimeSlot { get; set; } = string.Empty;
    public int Status { get; set; } = 0;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Pet Pet { get; set; } = null!;
    public Service Service { get; set; } = null!;
    public Review? Review { get; set; }
}
