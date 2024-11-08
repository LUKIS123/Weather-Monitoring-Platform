using WeatherMonitor.Server.DataView;
using WeatherMonitor.Server.DeviceManagement;
using WeatherMonitor.Server.Infrastructure;
using WeatherMonitor.Server.Middleware;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.UserAuthentication;
using WeatherMonitor.Server.UserAuthorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor(); // add instance of http context accessor

builder.Services.AddSharedKernelModule(builder.Configuration);
builder.Services.AddInfrastructureModule(builder.Configuration);
builder.Services.AddUserModule(builder.Configuration);
builder.Services.AddUserAuthorizationModule();
builder.Services.AddDeviceManagementModule();
builder.Services.AddDataViewModule();


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
    opt.AddPolicy("FrontEndClient", corsPolicyBuilder => corsPolicyBuilder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(_ => true)
        .AllowCredentials());
});

var app = builder.Build();


app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontEndClient");

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeMiddleware>();

app.RegisterUserEndpoints();
app.RegisterDeviceManagementEndpoints();
app.RegisterDataViewEndpoints();

app.MapFallbackToFile("/index.html");

app.Run();
