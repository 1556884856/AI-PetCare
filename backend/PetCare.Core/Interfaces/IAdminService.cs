using PetCare.Core.Dtos;

namespace PetCare.Core.Interfaces;

/// <summary>管理员服务接口，提供仪表盘统计、预约管理、服务管理、客户查询等后台功能</summary>
public interface IAdminService
{
    /// <summary>获取管理仪表盘数据（今日预约数、待处理数、今日营收、本月新客户）</summary>
    Task<DashboardDto> GetDashboardAsync();
    
    /// <summary>获取服务列表（支持按类别和宠物类型筛选）</summary>
    Task<List<ServiceDto>> GetServicesAsync(string? category, string? petType);
    
    /// <summary>获取单个服务详情</summary>
    Task<ServiceDto?> GetServiceAsync(int id);
    
    /// <summary>创建新服务</summary>
    Task<ServiceDto> CreateServiceAsync(CreateServiceRequest request);
    
    /// <summary>更新服务信息</summary>
    Task<ServiceDto> UpdateServiceAsync(int id, UpdateServiceRequest request);
    
    /// <summary>删除服务</summary>
    Task DeleteServiceAsync(int id);
    
    /// <summary>获取所有预约列表（可按日期和状态筛选）</summary>
    Task<List<AppointmentDto>> GetAllAppointmentsAsync(string? date, int? status);
    
    /// <summary>确认预约（状态变为 Confirmed）</summary>
    Task<AppointmentDto> ConfirmAppointmentAsync(int id);
    
    /// <summary>完成预约（状态变为 Completed）</summary>
    Task<AppointmentDto> CompleteAppointmentAsync(int id);
    
    /// <summary>取消预约（管理员操作，状态变为 Cancelled）</summary>
    Task<AppointmentDto> CancelAppointmentAsync(int id);
    
    /// <summary>获取客户列表（含宠物数和预约数统计）</summary>
    Task<List<CustomerDto>> GetCustomersAsync();
}
