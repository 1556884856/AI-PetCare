namespace PetCare.Core.Entities;

public class Review
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AppointmentId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Appointment Appointment { get; set; } = null!;
}
