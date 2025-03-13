using Passenger.Models;

namespace Passenger.Infrastructure.DTO;

public class TicketPurchaseRequest
{
    public required Guid PassengerId { get; set; }
    public required Guid FlightId { get; set; }
    public required PassengerClass SeatClass { get; set; }
    public required string MealType { get; set; }
    public required bool BaggagePresent { get; set; }
}
