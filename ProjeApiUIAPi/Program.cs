using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjeApiBusiness.Extension;
using ProjeApiBusiness.Mapping_1;
using ProjeApiDal.Context;
using ProjeApiModel.ViewModel.Identity;

var builder = WebApplication.CreateBuilder(args);

// DbContext kaydı
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApiContext>(options =>
    options.UseSqlServer(connectionString));

// Identity servislerini ekleyin:
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<ApiContext>()
    .AddDefaultTokenProviders();

// AutoMapper yapılandırması
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Business Layer ve diğer servis kayıtları
builder.Services.LoadBllLayerExtension(builder.Configuration);

// CORS yapılandırması
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMobileApp", policy =>
    {
        // Örneğin, UI projeniz http://localhost:7224 adresinde çalışıyor
        policy.WithOrigins("https://localhost:7224", "http://localhost:7224")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrnekApi Katmanli v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

// CORS middleware'i ekleyin (AllowMobileApp politikasıyla)
app.UseCors("AllowMobileApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
