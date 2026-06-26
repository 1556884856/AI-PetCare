using PetCare.Core.Enums;
using PetCare.Data.Repositories;
using PetCare.Core.Interfaces;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetCare.Core.Interfaces;
using PetCare.Data;
using PetCare.Service;
using PetCare.Service.RabbitMQ;
using PetCare.Service.Redis;

using PetCare.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Pet Care API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new()
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new()
    {
        {
            new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

var jwtKey = "PetCareSecretKey2026ForJwtTokenGeneration!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<PetCareDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// RabbitMQ & Redis
builder.Services.AddSingleton<RabbitMQConnection>();
builder.Services.AddSingleton<RedisConnection>();
builder.Services.AddSingleton<IMessageBus, MessageBus>();

// RabbitMQ Consumers (BackgroundService)
builder.Services.AddHostedService<NotificationConsumer>();
builder.Services.AddHostedService<PaymentTimeoutConsumer>();

// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// AutoMapper
builder.Services.AddAutoMapper(typeof(PetCare.Service.MappingProfile).Assembly);

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPetService, PetService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.Configure<HostOptions>(options => options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PetCareDbContext>();
    db.Database.EnsureCreated();
    if (!db.Users.Any(u => u.Phone == "admin"))
    {
        db.Users.Add(new PetCare.Core.Entities.User
        {
            Phone = "admin",
            Nickname = "Admin",
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        });
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
