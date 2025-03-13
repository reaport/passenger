namespace Passenger.Infrastructure.DTO;

public class RegisterPassengerResponse
{
    public required Guid FlightName { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime StartPlantingTime { get; set; }
    public required string Seat { get; set; }
}
