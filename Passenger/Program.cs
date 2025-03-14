using Passenger.Services;

namespace Passenger;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddHostedService<DriverService>();
        builder.Services.AddSingleton<IPassengerService, PassengerService>();
        builder.Services.AddSingleton<IFlightRefreshService, FlightRefreshService>();
        builder.Services.AddSingleton<IPassengerInteractionService, PassengerInteractionService>();
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        #region HTTP client registration
        builder.Services.AddHttpClient(
            "board",
            client =>
            {
                client.BaseAddress = new Uri("https://flight-board.rea.ru/");
            });

        builder.Services.AddHttpClient(
            "tickets",
            client =>
            {
                client.BaseAddress = new Uri("https://tickets.rea.ru/");
            });

        builder.Services.AddHttpClient(
            "register",
            client =>
            {
                client.BaseAddress = new Uri("https://register.rea.ru/");
            });
        
        #endregion

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
    }
}
