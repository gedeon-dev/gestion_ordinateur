using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using GestionOrdinateurs.Shared.Authentification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthentificationApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SymmetricSecurityKey _signingKey;

    public AuthenticationController(
        IHttpClientFactory httpClientFactory,
        SymmetricSecurityKey signingKey)
    {
        _httpClientFactory = httpClientFactory;
        _signingKey = signingKey;
    }

    [HttpPost]
    public async Task<ActionResult<TokenResponse>> CreateToken(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient("UsersApi");
        var userResponse = await httpClient.GetAsync(
            $"api/users/{Uri.EscapeDataString(request.Login)}",
            cancellationToken);

        if (userResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound("Utilisateur introuvable.");
        }

        userResponse.EnsureSuccessStatusCode();

        var user = await userResponse.Content.ReadFromJsonAsync<User>(
            cancellationToken: cancellationToken);
        if (user is null)
        {
            throw new JsonException("L’API utilisateurs a retourné une réponse vide.");
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Login),
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(15);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return Ok(new TokenResponse(new JwtSecurityTokenHandler().WriteToken(token), expires));
    }
}

public sealed class LoginRequest
{
    [System.ComponentModel.DataAnnotations.Required]
    public string Login { get; set; } = string.Empty;
}

public sealed record TokenResponse(string Token, DateTime ExpiresAt);
