using System.Text;
using System.Text.Json;
using Passenger.Infrastructure.DTO;

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

        var requestBody = new StringContent("\"\"", Encoding.UTF8, "application/json"); // Empty string as per API spec
        HttpResponseMessage response = await httpClient.PostAsync("buy", requestBody);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error fetching flights: {response.StatusCode}");
        }
        
        string responseContent = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseContent);
        var root = doc.RootElement;

        if (!root.TryGetProperty("availableFlights", out JsonElement flightsElement))
        {
            throw new Exception("Invalid response format: 'availableFlights' not found");
        }

        var flights = JsonSerializer.Deserialize<List<FlightInfo>>(flightsElement.GetRawText(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return flights ?? [];

    }
}