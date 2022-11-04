using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ParkingLotApi.Repository;
using ParkingLotApi.Services;

public partial class Program
{
  private static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    //builder.Services.AddScoped<IParkingLotService, ParkingLotService>();
    builder.Services.AddScoped<ParkingLotService>();

    builder.Services.AddDbContext<ParkingLotDbContext>(options =>
    {
      var connectionString = builder.Configuration.GetConnectionString("Default");
      options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    });
    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
      var dbContext = scope.ServiceProvider.GetRequiredService<ParkingLotDbContext>();
      using var context = scope.ServiceProvider.GetService<ParkingLotDbContext>();
      context.Database.EnsureDeleted();
      context.Database.EnsureCreated();
      //if (dbContext.Database.ProviderName.ToLower().Contains("mysql"))
      //{
      //  dbContext.Database.Migrate();
      //}
    }

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
  }
}