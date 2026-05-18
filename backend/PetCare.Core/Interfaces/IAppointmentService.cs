using PetCare.Core.Dtos;

namespace PetCare.Core.Interfaces;

public interface IAppointmentService
{
    Task<List<AppointmentDto>> GetUserAppointmentsAsync(int userId, int? status);
    Task<AppointmentDto> CreateAppointmentAsync(int userId, CreateAppointmentRequest request);
    Task CancelAppointmentAsync(int userId, int appointmentId);
}
