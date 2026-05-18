using PetCare.Core.Dtos;

namespace PetCare.Core.Interfaces;

public interface IAdminService
{
    Task<DashboardDto> GetDashboardAsync();
    Task<List<ServiceDto>> GetServicesAsync(string? category, string? petType);
    Task<ServiceDto?> GetServiceAsync(int id);
    Task<ServiceDto> CreateServiceAsync(CreateServiceRequest request);
    Task<ServiceDto> UpdateServiceAsync(int id, UpdateServiceRequest request);
    Task DeleteServiceAsync(int id);
    Task<List<AppointmentDto>> GetAllAppointmentsAsync(string? date, int? status);
    Task<AppointmentDto> ConfirmAppointmentAsync(int id);
    Task<AppointmentDto> CompleteAppointmentAsync(int id);
    Task<AppointmentDto> CancelAppointmentAsync(int id);
    Task<List<CustomerDto>> GetCustomersAsync();
}
