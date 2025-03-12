using Passenger.Infrastructure;
using Passenger.Infrastructure.DTO;

namespace Passenger.Services;

public interface IPassengerInteractionService
{
    public Task<ApiResponse<TicketPurchaseResponse>> BuyTicketAsync(TicketPurchaseRequest request);
    Task<ApiResponse<RegisterPassengerResponse>> RegisterPassengerAsync(RegisterPassengerRequest request);
}