
namespace ProfiDesktop.Models;

public record LoghinRequest(string Email, string Password);

public record ApiResponce<T>(
    bool success,
    T? data,
    string message,
    string? errorCode = null
);

public record AuthData(string AccessToken, string RefreshToken, string Role);

