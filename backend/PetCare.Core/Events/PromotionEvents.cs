namespace PetCare.Core.Events;

public record PromotionNotificationEvent
{
    public int[] UserIds { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;

    public PromotionNotificationEvent(int[] userIds, string title, string content)
    {
        UserIds = userIds;
        Title = title;
        Content = content;
    }
}

public record CouponReceivedEvent
{
    public int UserId { get; init; }
    public string CouponName { get; init; } = string.Empty;
    public decimal Value { get; init; }

    public CouponReceivedEvent(int userId, string couponName, decimal value)
    {
        UserId = userId;
        CouponName = couponName;
        Value = value;
    }
}

public record CouponDistributeEvent
{
    public int[] UserIds { get; init; }
    public int CouponId { get; init; }

    public CouponDistributeEvent(int[] userIds, int couponId)
    {
        UserIds = userIds;
        CouponId = couponId;
    }
}

public record PaymentTimeoutEvent
{
    public int PaymentId { get; init; }
    public int AppointmentId { get; init; }
    public int? CouponId { get; init; }

    public PaymentTimeoutEvent(int paymentId, int appointmentId, int? couponId)
    {
        PaymentId = paymentId;
        AppointmentId = appointmentId;
        CouponId = couponId;
    }
}
