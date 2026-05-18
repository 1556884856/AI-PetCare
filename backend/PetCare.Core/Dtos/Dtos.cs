namespace PetCare.Core.Dtos;

public record LoginRequest(string Phone, string Code);
public record SendCodeRequest(string Phone);
public record LoginResponse(string Token, UserDto User);

public record UserDto(int Id, string Phone, string Nickname, string AvatarUrl, int Role);
public record UpdateProfileRequest(string? Nickname);

public record PetDto(int Id, string Name, string Type, string Breed, int Age, decimal Weight, string Notes, DateTime CreatedAt);
public record CreatePetRequest(string Name, string Type, string Breed, int Age, decimal Weight, string Notes);
public record UpdatePetRequest(string Name, string Type, string Breed, int Age, decimal Weight, string Notes);

public record ServiceDto(int Id, string Name, string Description, string Category, string PetType, decimal Price, int DurationMinutes, string ImageUrl, int SortOrder, bool IsActive);
public record CreateServiceRequest(string Name, string Description, string Category, string PetType, decimal Price, int DurationMinutes, int SortOrder);
public record UpdateServiceRequest(string Name, string Description, string Category, string PetType, decimal Price, int DurationMinutes, int SortOrder, bool IsActive);

public record CreateAppointmentRequest(int ServiceId, int PetId, DateTime AppointmentDate, string TimeSlot, string Notes);
public record AppointmentDto(int Id, int UserId, int PetId, int ServiceId, DateTime AppointmentDate, string TimeSlot, int Status, string Notes, DateTime CreatedAt, string PetName, string PetType, string ServiceName, decimal Price, string? CustomerName, string? CustomerPhone);

public record DashboardDto(DashboardStatsDto Stats, List<AppointmentDto> TodayAppointments);
public record DashboardStatsDto(int TodayAppointments, int PendingAppointments, decimal TodayRevenue, int MonthNewCustomers);

public record CustomerDto(int Id, string Phone, string Nickname, int Role, int PetCount, int AppointmentCount, DateTime CreatedAt);
