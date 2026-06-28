using PetCare.Core.Dtos;

namespace PetCare.Core.Interfaces;

/// <summary>支付服务接口，管理支付单的创建、支付和退款</summary>
public interface IPaymentService
{
    /// <summary>创建支付单（计算优惠券抵扣金额）</summary>
    Task<PaymentDto> CreatePaymentAsync(int userId, CreatePaymentRequest request);
    
    /// <summary>模拟支付（开发环境用，1.5秒延迟模拟支付过程）</summary>
    Task<PaymentDto> MockPayAsync(int userId, int paymentId);
    
    /// <summary>退款（管理员操作）</summary>
    Task<PaymentDto> RefundAsync(int paymentId);
    
    /// <summary>获取支付单详情</summary>
    Task<PaymentDto?> GetPaymentAsync(int paymentId);
    
    /// <summary>获取用户的支付记录列表</summary>
    Task<List<PaymentDto>> GetUserPaymentsAsync(int userId);
    
    /// <summary>获取所有支付记录（管理员）</summary>
    Task<List<PaymentDto>> GetAllPaymentsAsync();
}
