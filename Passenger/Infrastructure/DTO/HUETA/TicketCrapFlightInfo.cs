namespace Passenger.Infrastructure.DTO.HUETA;

public class TicketCrapFlightInfo
{
    public string? FlightId { get; set; }
    public string? Direction { get; set; }
    public DateTime DepartureTime { get; set; }
    // Доступные места по классам (economy, business)
    public Dictionary<string, int>? AvailableSeats { get; set; }
}