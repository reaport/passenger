using Passenger.Infrastructure;
using Passenger.Infrastructure.DTO;
using Passenger.Infrastructure.DTO.HUETA;

namespace Passenger.Services;

public interface IPassengerInteractionService
{
    public Task<ApiResponse<TicketPurchaseResponse>> BuyTicketAsync(BuyTicketRequest request);
    Task<ApiResponse<RegisterPassengerResponse>> RegisterPassengerAsync(RegisterPassengerRequest request);
}