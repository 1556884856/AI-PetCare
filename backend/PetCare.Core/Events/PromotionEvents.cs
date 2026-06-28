namespace PetCare.Core.Events;

// ===================================================================
// 促销和优惠券相关领域事件
// ===================================================================

/// <summary>促销通知事件：管理员群发通知时发布</summary>
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

/// <summary>优惠券领取事件：用户成功领取优惠券后发布，用于通知用户</summary>
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

/// <summary>优惠券分发事件：管理员批量分发优惠券时发布</summary>
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

/// <summary>支付超时事件：支付单创建时发布，30分钟后通过死信队列触发，自动取消未支付订单</summary>
public record PaymentTimeoutEvent
{
    /// <summary>支付单ID</summary>
    public int PaymentId { get; init; }
    
    /// <summary>关联的预约ID</summary>
    public int AppointmentId { get; init; }
    
    /// <summary>使用的优惠券ID（需退还）</summary>
    public int? CouponId { get; init; }

    public PaymentTimeoutEvent(int paymentId, int appointmentId, int? couponId)
    {
        PaymentId = paymentId;
        AppointmentId = appointmentId;
        CouponId = couponId;
    }
}
