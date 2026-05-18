using Microsoft.EntityFrameworkCore;
using PetCare.Core.Dtos;
using PetCare.Core.Entities;
using PetCare.Core.Interfaces;
using PetCare.Data;

namespace PetCare.Service;

public class AdminService : IAdminService
{
    private readonly PetCareDbContext _db;
    public AdminService(PetCareDbContext db) { _db = db; }

    public async Task<DashboardDto> GetDashboardAsync()
    {
        var today = DateTime.UtcNow.Date;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);

        var todayApps = await _db.Appointments.CountAsync(a => a.AppointmentDate.Date == today);
        var pendingApps = await _db.Appointments.CountAsync(a => a.Status == 0);
        var todayRevenue = await _db.Appointments.Where(a => a.AppointmentDate.Date == today && a.Status != 3).SumAsync(a => a.Service.Price);
        var monthNew = await _db.Users.CountAsync(u => u.CreatedAt >= startOfMonth);

        var todayList = await GetTodayAppointmentsAsync(today);

        return new DashboardDto(
            new DashboardStatsDto(todayApps, pendingApps, todayRevenue, monthNew),
            todayList
        );
    }

    private async Task<List<AppointmentDto>> GetTodayAppointmentsAsync(DateTime today)
    {
        return await _db.Appointments
            .Where(a => a.AppointmentDate.Date == today)
            .Include(a => a.User).Include(a => a.Pet).Include(a => a.Service)
            .OrderBy(a => a.TimeSlot)
            .Select(a => MapAppointment(a))
            .ToListAsync();
    }

    public async Task<List<ServiceDto>> GetServicesAsync(string? category, string? petType)
    {
        var q = _db.Services.AsQueryable();
        if (!string.IsNullOrEmpty(category)) q = q.Where(s => s.Category == category);
        if (!string.IsNullOrEmpty(petType)) q = q.Where(s => s.PetType == petType || s.PetType == "All");
        return await q.OrderBy(s => s.SortOrder).Select(s => MapService(s)).ToListAsync();
    }

    public async Task<ServiceDto?> GetServiceAsync(int id)
    {
        var s = await _db.Services.FindAsync(id);
        return s == null ? null : MapService(s);
    }

    public async Task<ServiceDto> CreateServiceAsync(CreateServiceRequest r)
    {
        var s = new Core.Entities.Service { Name = r.Name, Description = r.Description, Category = r.Category, PetType = r.PetType, Price = r.Price, DurationMinutes = r.DurationMinutes, SortOrder = r.SortOrder };
        _db.Services.Add(s);
        await _db.SaveChangesAsync();
        return MapService(s);
    }

    public async Task<ServiceDto> UpdateServiceAsync(int id, UpdateServiceRequest r)
    {
        var s = await _db.Services.FindAsync(id) ?? throw new Exception("服务不存在");
        s.Name = r.Name; s.Description = r.Description; s.Category = r.Category; s.PetType = r.PetType; s.Price = r.Price; s.DurationMinutes = r.DurationMinutes; s.SortOrder = r.SortOrder; s.IsActive = r.IsActive;
        await _db.SaveChangesAsync();
        return MapService(s);
    }

    public async Task DeleteServiceAsync(int id)
    {
        var s = await _db.Services.FindAsync(id) ?? throw new Exception("服务不存在");
        _db.Services.Remove(s);
        await _db.SaveChangesAsync();
    }

    public async Task<List<AppointmentDto>> GetAllAppointmentsAsync(string? date, int? status)
    {
        var q = _db.Appointments.Include(a => a.User).Include(a => a.Pet).Include(a => a.Service).AsQueryable();
        if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var d)) q = q.Where(a => a.AppointmentDate.Date == d.Date);
        if (status.HasValue) q = q.Where(a => a.Status == status.Value);
        return await q.OrderByDescending(a => a.AppointmentDate).ThenBy(a => a.TimeSlot).Select(a => MapAppointment(a)).ToListAsync();
    }

    public async Task<AppointmentDto> ConfirmAppointmentAsync(int id)
    {
        var a = await _db.Appointments.Include(x => x.User).Include(x => x.Pet).Include(x => x.Service).FirstOrDefaultAsync(x => x.Id == id) ?? throw new Exception("预约不存在");
        a.Status = 1;
        await _db.SaveChangesAsync();
        return MapAppointment(a);
    }

    public async Task<AppointmentDto> CompleteAppointmentAsync(int id)
    {
        var a = await _db.Appointments.Include(x => x.User).Include(x => x.Pet).Include(x => x.Service).FirstOrDefaultAsync(x => x.Id == id) ?? throw new Exception("预约不存在");
        a.Status = 2;
        await _db.SaveChangesAsync();
        return MapAppointment(a);
    }

    public async Task<AppointmentDto> CancelAppointmentAsync(int id)
    {
        var a = await _db.Appointments.Include(x => x.User).Include(x => x.Pet).Include(x => x.Service).FirstOrDefaultAsync(x => x.Id == id) ?? throw new Exception("预约不存在");
        a.Status = 3;
        await _db.SaveChangesAsync();
        return MapAppointment(a);
    }

    public async Task<List<CustomerDto>> GetCustomersAsync()
    {
        return await _db.Users.Select(u => new CustomerDto(
            u.Id, u.Phone, u.Nickname, u.Role,
            u.Pets.Count,
            u.Appointments.Count,
            u.CreatedAt
        )).OrderByDescending(c => c.CreatedAt).ToListAsync();
    }

    private static ServiceDto MapService(Core.Entities.Service s) => new(s.Id, s.Name, s.Description, s.Category, s.PetType, s.Price, s.DurationMinutes, s.ImageUrl, s.SortOrder, s.IsActive);

    private static AppointmentDto MapAppointment(Appointment a) => new(
        a.Id, a.UserId, a.PetId, a.ServiceId, a.AppointmentDate, a.TimeSlot, a.Status, a.Notes, a.CreatedAt,
        a.Pet.Name, a.Pet.Type, a.Service.Name, a.Service.Price,
        a.User.Nickname, a.User.Phone
    );
}
