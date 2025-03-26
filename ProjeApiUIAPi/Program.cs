using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjeApiBusiness.Extension;
using ProjeApiBusiness.Mapping_1;
using ProjeApiDal.Context;
using ProjeApiModel.ViewModel.Identity;

var builder = WebApplication.CreateBuilder(args);

// DbContext kaydý
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApiContext>(options =>
    options.UseSqlServer(connectionString));

// Identity servislerini ekleyin:
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<ApiContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Business Layer ve diðer servis kayýtlarý
builder.Services.LoadBllLayerExtension(builder.Configuration);
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


app.UseCors("AllowMobileApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
