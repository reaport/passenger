using Passenger.Infrastructure;
using Passenger.Infrastructure.DTO;
using Passenger.Infrastructure.DTO.HUETA;

namespace Passenger.Services;

public class PassengerInteractionService : IPassengerInteractionService
{
    private IHttpClientFactory _httpClientFactory;
    private ILogger _logger;
    public PassengerInteractionService(IHttpClientFactory httpClientFactory, ILogger<PassengerInteractionService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }
    public async Task<ApiResponse<TicketPurchaseResponse>> BuyTicketAsync(BuyTicketRequest request)
    {
        var httpClient = _httpClientFactory.CreateClient("tickets");
        var response = await httpClient.PostAsJsonAsync("buy", request);

        if (response.IsSuccessStatusCode)
        {
            var ticket = await response.Content.ReadFromJsonAsync<TicketPurchaseResponse>();
            return new ApiResponse<TicketPurchaseResponse>
            {
                Data = ticket,
                IsSuccessful = true
            };
        }

        var errorMessage = await response.Content.ReadAsStringAsync();
        return new ApiResponse<TicketPurchaseResponse>
        {
            IsSuccessful = false,
            ErrorMessage = $"Error {response.StatusCode}: {errorMessage}"
        };
    }
    public async Task<ApiResponse<RegisterPassengerResponse>> RegisterPassengerAsync(RegisterPassengerRequest request)
    {
        var httpClient = _httpClientFactory.CreateClient("register");
        var response = await httpClient.PostAsJsonAsync("passenger", request);

        if (response.IsSuccessStatusCode)
        {
            var registerResponse = await response.Content.ReadFromJsonAsync<RegisterPassengerResponse>();
            return new ApiResponse<RegisterPassengerResponse>
            {
                Data = registerResponse,
                IsSuccessful = true
            };
        }

        var errorMessage = await response.Content.ReadAsStringAsync();

        return new ApiResponse<RegisterPassengerResponse>
        {
            IsSuccessful = false,
            ErrorMessage = $"Error {response.StatusCode}: {errorMessage}"
        };
    }
}