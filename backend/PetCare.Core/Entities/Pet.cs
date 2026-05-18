namespace PetCare.Core.Entities;

public class Pet
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "Dog";
    public string Breed { get; set; } = string.Empty;
    public int Age { get; set; }
    public decimal Weight { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
