namespace Passenger.Infrastructure;

public class ApiResponse<T>
{
    public T? Data {get;set;}
    public bool IsSuccessful {get;set;}
    public string? ErrorMessage {get;set;}
}