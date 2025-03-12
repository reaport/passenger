namespace Passenger.Infrastructure.DTO;

public class TicketPurchaseResponse
{
    public required Guid TicketId { get; set; }
    public required string Direction { get; set; }
    public required DateTime DepartureTime { get; set; }
    public required string Status { get; set; }
}
