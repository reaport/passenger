namespace Passenger.Infrastructure.DTO.HUETA;

public class TicketCrapFlightInfo
{
    public string? FlightId { get; set; }
    public string? Direction { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime RegistrationStartTime { get; set; }
    public Dictionary<string, int>? AvailableSeats { get; set; }
}