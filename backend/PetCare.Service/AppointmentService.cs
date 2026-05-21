using Microsoft.EntityFrameworkCore;
using PetCare.Core.Dtos;
using PetCare.Core.Entities;
using PetCare.Core.Events;
using PetCare.Core.Interfaces;
using PetCare.Data;

namespace PetCare.Service;

public class AppointmentService : IAppointmentService
{
    private readonly PetCareDbContext _db;
    private readonly IMessageBus _messageBus;
    public AppointmentService(PetCareDbContext db, IMessageBus messageBus) { _db = db; _messageBus = messageBus; }

    public async Task<List<AppointmentDto>> GetUserAppointmentsAsync(int userId, int? status)
    {
        var q = _db.Appointments.Where(a => a.UserId == userId).Include(a => a.Pet).Include(a => a.Service).AsQueryable();
        if (status.HasValue) q = q.Where(a => a.Status == status.Value);
        return await q.OrderByDescending(a => a.AppointmentDate).ThenBy(a => a.TimeSlot).Select(a => new AppointmentDto(
            a.Id, a.UserId, a.PetId, a.ServiceId, a.AppointmentDate, a.TimeSlot, a.Status, a.Notes, a.CreatedAt,
            a.Pet.Name, a.Pet.Type, a.Service.Name, a.Service.Price, null, null
        )).ToListAsync();
    }

    public async Task<AppointmentDto> CreateAppointmentAsync(int userId, CreateAppointmentRequest r)
    {
        var pet = await _db.Pets.FirstOrDefaultAsync(p => p.Id == r.PetId && p.UserId == userId) ?? throw new Exception("宠物不存在");
        var service = await _db.Services.FirstOrDefaultAsync(s => s.Id == r.ServiceId && s.IsActive) ?? throw new Exception("服务不存在");

        var existing = await _db.Appointments.AnyAsync(a => a.AppointmentDate.Date == r.AppointmentDate.Date && a.TimeSlot == r.TimeSlot && a.Status != 3);
        if (existing) throw new Exception("该时段已被预约");

        var app = new Appointment
        {
            UserId = userId, PetId = r.PetId, ServiceId = r.ServiceId,
            AppointmentDate = r.AppointmentDate.Date, TimeSlot = r.TimeSlot,
            Status = 0, Notes = r.Notes
        };
        _db.Appointments.Add(app);
        await _db.SaveChangesAsync();

        // 发布预约创建事件 → 通知系统异步处理
        _messageBus.Publish("appointment.created", new AppointmentCreatedEvent(
            app.Id, userId, pet.Name, service.Name, app.AppointmentDate, app.TimeSlot
        ));

        return new AppointmentDto(app.Id, app.UserId, app.PetId, app.ServiceId, app.AppointmentDate, app.TimeSlot, app.Status, app.Notes, app.CreatedAt,
            pet.Name, pet.Type, service.Name, service.Price, null, null);
    }

    public async Task CancelAppointmentAsync(int userId, int appointmentId)
    {
        var app = await _db.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId && a.UserId == userId) ?? throw new Exception("预约不存在");
        if (app.Status == 2) throw new Exception("已完成的预约不能取消");
        app.Status = 3;
        await _db.SaveChangesAsync();
    }
}
