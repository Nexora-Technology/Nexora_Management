namespace Nexora.Management.Application.Authentication.DTOs;

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User
);

public record UserDto(
    Guid Id,
    string Email,
    string? Name,
    string? AvatarUrl
);
