using Passenger.Infrastructure;
using Passenger.Infrastructure.DTO;

namespace Passenger.Services;

public class PassengerInteractionService : IPassengerInteractionService
{
    private IHttpClientFactory _httpClientFactory;
    public PassengerInteractionService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    public async Task<ApiResponse<TicketPurchaseResponse>> BuyTicketAsync(TicketPurchaseRequest request)
    {
        throw new NotImplementedException();
    }
    public async Task<ApiResponse<RegisterPassengerResponse>> RegisterPassengerAsync(RegisterPassengerRequest request)
    {
        throw new NotImplementedException();
    }
}