using ClinicSaaS.Api.Services;
using ClinicSaaS.Api.Swagger;
using ClinicSaaS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Clinic SaaS API",
        Version = "v1",
        Description = "Backend API for clinic scheduling, patient management, and SaaS-ready operations."
    });

    options.OperationFilter<AddClinicHeaderOperationFilter>();
});

builder.Services.AddDbContext<ClinicSaaSDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentClinicService>();
builder.Services.AddScoped<DashboardService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();