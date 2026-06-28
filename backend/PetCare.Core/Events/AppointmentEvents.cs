namespace PetCare.Core.Events;

// ===================================================================
// 预约相关领域事件，通过 RabbitMQ 发布，由 NotificationConsumer 消费
// ===================================================================

/// <summary>预约事件基类，包含预约ID、用户ID和事件类型</summary>
public record AppointmentEvent(int AppointmentId, int UserId, string? Type);

/// <summary>预约创建事件：用户提交新预约时发布</summary>
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

/// <summary>预约状态变更事件：管理员确认/完成/取消预约时发布</summary>
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

/// <summary>支付完成事件：用户支付成功后发布，用于通知用户</summary>
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
