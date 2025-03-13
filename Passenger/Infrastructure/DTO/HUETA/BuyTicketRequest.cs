namespace Passenger.Infrastructure.DTO.HUETA;

public class BuyTicketRequest
{
    public string ?PassengerId { get; set; }
    public string ?FlightId { get; set; }
    public string ?SeatClass { get; set; } // economy, business
    public string ?MealType { get; set; }  // выбранный тип питания
    public string ?Baggage { get; set; }   // "да" или "нет"
}