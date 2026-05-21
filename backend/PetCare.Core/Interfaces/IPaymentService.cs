using PetCare.Core.Dtos;

namespace PetCare.Core.Interfaces;

public interface IPaymentService
{
    Task<PaymentDto> CreatePaymentAsync(int userId, CreatePaymentRequest request);
    Task<PaymentDto> MockPayAsync(int userId, int paymentId);
    Task<PaymentDto> RefundAsync(int paymentId);
    Task<PaymentDto?> GetPaymentAsync(int paymentId);
    Task<List<PaymentDto>> GetUserPaymentsAsync(int userId);
    Task<List<PaymentDto>> GetAllPaymentsAsync();
}
