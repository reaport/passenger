using System.Text;
using System.Text.Json;
using Passenger.Infrastructure;
using Passenger.Infrastructure.DTO;
using Passenger.Infrastructure.DTO.HUETA;

namespace Passenger.Services;

public class FlightRefreshService : IFlightRefreshService
{
    private IHttpClientFactory _httpClientFactory;
    private ILogger _logger;
    public FlightRefreshService(IHttpClientFactory httpClientFactory, ILogger<FlightRefreshService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<IEnumerable<FlightInfo>> GetAvailableFlights()
    {
        // Tickets because ofc this makes sense...
        var httpClient = _httpClientFactory.CreateClient("tickets");

        var requestBodyObject = new BuyTicketRequest(); // All properties are null by default
        var requestBodyJson = JsonSerializer.Serialize(requestBodyObject, new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
        var requestBody = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await httpClient.PostAsync("buy", requestBody);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new ApiRequestException(httpClient.BaseAddress+"buy", response.StatusCode, response.Content.ToString());
        }

        string responseContent = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseContent);
        var root = doc.RootElement;

        if (!root.TryGetProperty("availableFlights", out JsonElement flightsElement))
        {
            throw new Exception("Invalid response format: 'availableFlights' not found");
        }

        var flights = JsonSerializer.Deserialize<List<TicketCrapFlightInfo>>(flightsElement.GetRawText(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        _logger.LogInformation($"Received {flights.Count} flights");
        
        List<string>? meals = new();

        if(flights is not null && flights.Count > 0)
        {
            meals = await GetAvailableMealTypesAsync(flights.First().FlightId!);
        }

        _logger.LogInformation($"Received {meals.Count} meal types");

        var infos  = flights?.Select( flightCrapInfo =>
            new FlightInfo
            {
                FlightId = flightCrapInfo.FlightId!,
                EconomySeats = flightCrapInfo.AvailableSeats!["Economy"],
                VIPSeats = flightCrapInfo.AvailableSeats["Business"],
                RegistrationStartTime = flightCrapInfo.RegistrationStartTime.ToUniversalTime(),
                DepartureTime = flightCrapInfo.DepartureTime,
                AvailableMealTypes = meals
            }
        ) 
        ?? [];

        return infos;

    }

    public async Task<List<string>> GetAvailableMealTypesAsync(string flightId)
    {
        var httpClient = _httpClientFactory.CreateClient("tickets");

        var requestBodyObject = new BuyTicketRequest { FlightId = flightId };
        var requestBodyJson = JsonSerializer.Serialize(requestBodyObject, new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
        var requestBody = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");
        
        HttpResponseMessage response = await httpClient.PostAsync("buy", requestBody);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error fetching meal types: {response.StatusCode}");
        }
        
        string responseContent = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseContent);
        var root = doc.RootElement;

        if (!root.TryGetProperty("availableMealOptions", out JsonElement mealElement))
        {
            throw new Exception("Invalid response format: 'availableMealOptions' not found");
        }

        var mealTypes = JsonSerializer.Deserialize<List<string>>(mealElement.GetRawText(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return mealTypes ?? new List<string>();
    }
}