using Passenger.Infrastructure;
using Passenger.Infrastructure.DTO;
using Passenger.Infrastructure.DTO.HUETA;

namespace Passenger.Services;

public class PassengerInteractionServiceFake : IPassengerInteractionService
{
    public Task<ApiResponse<TicketPurchaseResponse>> BuyTicketAsync(BuyTicketRequest request)
    {
        var result = new ApiResponse<TicketPurchaseResponse>()
        {
            Data = new TicketPurchaseResponse
            {
                TicketId = Guid.NewGuid(),
                Direction = "asdaf",
                DepartureTime = DateTime.Now + TimeSpan.FromSeconds(400),
                Status = "asdafg"
            },
            IsSuccessful = true,
            ErrorMessage = null
        };

        return Task.FromResult(result);
    }

    public Task<ApiResponse<RegisterPassengerResponse>> RegisterPassengerAsync(RegisterPassengerRequest request)
    {
        var result = new ApiResponse<RegisterPassengerResponse>()
        {
            Data = new RegisterPassengerResponse
            {
                FlightName = Guid.NewGuid(),
                DepartureTime = DateTime.Now + TimeSpan.FromSeconds(400),
                StartPlantingTime = DateTime.Now + TimeSpan.FromSeconds(20),
                Seat = "ABC2345"
            },
            IsSuccessful = true,
            ErrorMessage = null
        };

        return Task.FromResult(result);
    }
}
