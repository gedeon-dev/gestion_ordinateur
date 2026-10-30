using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;

namespace AuthentificationApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AuthenticationController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<ActionResult<TokenResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var clientId = _configuration["Keycloak:ClientId"]
            ?? throw new InvalidOperationException("La configuration Keycloak:ClientId est obligatoire.");
        var form = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = clientId,
            ["username"] = request.Login,
            ["password"] = request.Password
        };
        var clientSecret = _configuration["Keycloak:ClientSecret"];
        if (!string.IsNullOrWhiteSpace(clientSecret))
        {
            form["client_secret"] = clientSecret;
        }

        var httpClient = _httpClientFactory.CreateClient("Keycloak");
        using var keycloakResponse = await httpClient.PostAsync(
            string.Empty,
            new FormUrlEncodedContent(form),
            cancellationToken);

        if (keycloakResponse.StatusCode == HttpStatusCode.BadRequest)
        {
            var error = await keycloakResponse.Content.ReadFromJsonAsync<KeycloakErrorResponse>(
                cancellationToken: cancellationToken);
            if (error?.Error == "invalid_grant")
            {
                return Unauthorized("Identifiant ou mot de passe invalide.");
            }
        }

        keycloakResponse.EnsureSuccessStatusCode();
        var keycloakToken = await keycloakResponse.Content.ReadFromJsonAsync<KeycloakTokenResponse>(
            cancellationToken: cancellationToken);
        if (keycloakToken is null || string.IsNullOrWhiteSpace(keycloakToken.AccessToken) ||
            keycloakToken.ExpiresIn <= 0)
        {
            throw new InvalidDataException("Keycloak a retourné une réponse de token invalide.");
        }

        var expiresAt = DateTime.UtcNow.AddSeconds(keycloakToken.ExpiresIn);
        return Ok(new TokenResponse(keycloakToken.AccessToken, expiresAt));
    }
}

public sealed class LoginRequest
{
    [System.ComponentModel.DataAnnotations.Required]
    public string Login { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required]
    public string Password { get; set; } = string.Empty;
}

public sealed record TokenResponse(string Token, DateTime ExpiresAt);

internal sealed class KeycloakTokenResponse
{
    [System.Text.Json.Serialization.JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [System.Text.Json.Serialization.JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}

internal sealed class KeycloakErrorResponse
{
    [System.Text.Json.Serialization.JsonPropertyName("error")]
    public string? Error { get; set; }
}
