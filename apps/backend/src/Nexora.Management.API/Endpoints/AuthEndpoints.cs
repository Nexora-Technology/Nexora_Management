using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexora.Management.Application.Authentication.Commands.Login;
using Nexora.Management.Application.Authentication.Commands.RefreshToken;
using Nexora.Management.Application.Authentication.Commands.Register;
using Nexora.Management.Application.Authentication.DTOs;

namespace Nexora.Management.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication")
            .WithOpenApi();

        group.MapPost("/register", async (RegisterRequest request, ISender sender) =>
        {
            var command = new RegisterCommand(request.Email, request.Password, request.Name);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("Register")
        .WithSummary("Register a new user")
        .WithDescription("Creates a new user account with a default workspace.");

        group.MapPost("/login", async (LoginRequest request, ISender sender) =>
        {
            var command = new LoginCommand(request.Email, request.Password);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("Login")
        .WithSummary("Login with email and password")
        .WithDescription("Authenticates user and returns JWT tokens.");

        group.MapPost("/refresh", async (RefreshTokenRequest request, ISender sender) =>
        {
            var command = new RefreshTokenCommand(request.Token, request.RefreshToken);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("RefreshToken")
        .WithSummary("Refresh access token")
        .WithDescription("Generates new access token using refresh token.");
    }
}
