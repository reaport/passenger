using Microsoft.OpenApi.Models;
using Passenger.Models;
using Passenger.Services;

namespace Passenger;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddControllersWithViews();
        builder.Services.AddSingleton<IPassengerService, PassengerService>();
        builder.Services.AddSingleton<IFlightRefreshService, FlightRefreshService>();
        builder.Services.AddSingleton<IPassengerInteractionService, PassengerInteractionService>();
        builder.Services.AddSingleton<ILoggingService, LoggingService>();

        #region Background service hacks

        builder.Services.AddSingleton<DriverService>();
        builder.Services.AddSingleton<IDriverService>(provider => provider.GetRequiredService<DriverService>());        
        builder.Services.AddHostedService<DriverService>(provider => provider.GetRequiredService<DriverService>());

        #endregion

        #region Keyed service registration
        //builder.Services.AddKeyedScoped<IPassengerStrategy, AirportStartPassengerStrategy>("Airport");
        //builder.Services.AddKeyedScoped<IPassengerStrategy, PlaneStartPassengerStrategy>("Plane");

        builder.Services.AddKeyedTransient<IPassengerFactory, AirportStartPassengerFactory>("Airport");
        builder.Services.AddKeyedTransient<IPassengerFactory, PlaneStartPassengerFactory>("Plane");
        #endregion

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Passenger API", Version = "v1" });
        });

        builder.Services.AddSwaggerGenNewtonsoftSupport(); // Required for YAML support

        #region HTTP client registration
        builder.Services.AddHttpClient(
            "board",
            client =>
            {
                client.BaseAddress = new Uri("https://flight-board.reaport.ru/");
            });

        builder.Services.AddHttpClient(
            "tickets",
            client =>
            {
                client.BaseAddress = new Uri("https://tickets.reaport.ru/");
            });

        builder.Services.AddHttpClient(
            "register",
            client =>
            {
                client.BaseAddress = new Uri("https://register.reaport.ru/");
            });
        
        #endregion

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        //{
        app.UseDeveloperExceptionPage();
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "swagger/{documentName}/swagger.{json|yaml}";
        });

        // Enable middleware to serve swagger-ui 
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Passenger module API V1 (JSON)");
            c.SwaggerEndpoint("/swagger/v1/swagger.yaml", "Passenger module API V1 (YAML)");
        });

        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.MapControllers();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.Run();
    }
}
