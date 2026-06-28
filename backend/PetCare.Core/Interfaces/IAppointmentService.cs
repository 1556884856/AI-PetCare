using PetCare.Core.Dtos;

namespace PetCare.Core.Interfaces;

/// <summary>预约服务接口，负责用户端预约的创建和查询</summary>
public interface IAppointmentService
{
    /// <summary>获取用户的预约列表，可按状态筛选</summary>
    Task<List<AppointmentDto>> GetUserAppointmentsAsync(int userId, int? status);
    
    /// <summary>创建新预约（检查时段冲突并发布预约创建事件）</summary>
    Task<AppointmentDto> CreateAppointmentAsync(int userId, CreateAppointmentRequest request);
    
    /// <summary>取消预约</summary>
    Task CancelAppointmentAsync(int userId, int appointmentId);
}
