using Passenger.Infrastructure;
using Passenger.Infrastructure.DTO;
using Passenger.Infrastructure.DTO.HUETA;

namespace Passenger.Services;

public class PassengerInteractionServiceFake : IPassengerInteractionService
{
    public Task<ApiResponse<TicketPurchaseResponse>> BuyTicketAsync(BuyTicketRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<RegisterPassengerResponse>> RegisterPassengerAsync(RegisterPassengerRequest request)
    {
        throw new NotImplementedException();
    }
}
