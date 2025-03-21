namespace Passenger.Infrastructure.DTO.HUETA;

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

public class TicketCrapFlightInfo
{
    public string? FlightId { get; set; }
    public string? Direction { get; set; }

    [JsonConverter(typeof(UtcDateTimeConverter))]
    public DateTime DepartureTime { get; set; }

    [JsonConverter(typeof(UtcDateTimeConverter))]
    public DateTime RegistrationStartTime { get; set; }

    public Dictionary<string, int>? AvailableSeats { get; set; }
}

// Custom converter to enforce UTC parsing
public class UtcDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Parse as UTC explicitly
        return DateTime.Parse(reader.GetString()!, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // Write as UTC ISO 8601 format
        writer.WriteStringValue(value.ToUniversalTime().ToString("o"));
    }
}