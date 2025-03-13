using Passenger.Models;

namespace Passenger.Infrastructure.DTO;

public class RegisterPassengerRequest
{
    public required Guid PassengerId { get; set; }
    public float BaggageWeight { get; set; }
    public string MealType { get; set; }
}
