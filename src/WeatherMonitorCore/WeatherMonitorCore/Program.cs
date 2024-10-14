using WeatherMonitorCore.DeviceManagement;
using WeatherMonitorCore.Infrastructure;
using WeatherMonitorCore.Middleware;
using WeatherMonitorCore.SharedKernel;
using WeatherMonitorCore.UserAuthentication;
using WeatherMonitorCore.UserAuthorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor(); // add instance of http context accessor
builder.Services.AddMemoryCache(); // add instance of memory cache

builder.Services.AddSharedKernelModule();
builder.Services.AddInfrastructureModule(builder.Configuration);
builder.Services.AddUserModule(builder.Configuration);
builder.Services.AddUserAuthorizationModule();
builder.Services.AddDeviceManagementModule();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationInsightsTelemetry();

// Middleware
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<RequestTimeMiddleware>();

// CORS
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("WeatherMonitorClient", corsPolicyBuilder => corsPolicyBuilder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(_ => true)
        .AllowCredentials());
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeMiddleware>();

app.UseCors("WeatherMonitorClient");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.RegisterUserEndpoints();
app.RegisterDeviceManagementEndpoints();

app.Run();
