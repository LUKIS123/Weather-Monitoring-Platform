using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var sqlConnectionString = builder.Configuration.GetConnectionString("MS-SQL")
                          ?? throw new ArgumentNullException("MS-SQL");

// Add services to the container.
builder.Services.AddHttpContextAccessor(); //  add instance of http context accessor
builder.Services.AddMemoryCache(); // add instance of memory cache
builder.Services.AddAuthentication()
    .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
