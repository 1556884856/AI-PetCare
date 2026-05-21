namespace PetCare.Core.Events;

public record AppointmentEvent(int AppointmentId, int UserId, string? Type);

public record AppointmentCreatedEvent : AppointmentEvent
{
    public string PetName { get; init; } = string.Empty;
    public string ServiceName { get; init; } = string.Empty;
    public DateTime AppointmentDate { get; init; }
    public string TimeSlot { get; init; } = string.Empty;

    public AppointmentCreatedEvent(int appointmentId, int userId, string petName, string serviceName, DateTime appointmentDate, string timeSlot)
        : base(appointmentId, userId, "AppointmentCreated")
    {
        PetName = petName;
        ServiceName = serviceName;
        AppointmentDate = appointmentDate;
        TimeSlot = timeSlot;
    }
}

public record AppointmentStatusEvent : AppointmentEvent
{
    public int Status { get; init; }
    public string Message { get; init; } = string.Empty;

    public AppointmentStatusEvent(int appointmentId, int userId, int status, string message)
        : base(appointmentId, userId, "AppointmentStatus")
    {
        Status = status;
        Message = message;
    }
}

public record PaymentCompletedEvent
{
    public int PaymentId { get; init; }
    public int AppointmentId { get; init; }
    public int UserId { get; init; }
    public decimal Amount { get; init; }

    public PaymentCompletedEvent(int paymentId, int appointmentId, int userId, decimal amount)
    {
        PaymentId = paymentId;
        AppointmentId = appointmentId;
        UserId = userId;
        Amount = amount;
    }
}
