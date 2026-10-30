using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var keycloakAuthority = builder.Configuration["Keycloak:Authority"];
if (!Uri.TryCreate(keycloakAuthority, UriKind.Absolute, out var keycloakAuthorityUri))
{
    throw new InvalidOperationException("La configuration Keycloak:Authority doit être une URL absolue.");
}

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
if (allowedOrigins.Length == 0)
{
    throw new InvalidOperationException("Au moins une origine doit être configurée dans Cors:AllowedOrigins.");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = keycloakAuthorityUri.ToString().TrimEnd('/');
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = keycloakAuthorityUri.ToString().TrimEnd('/'),
            ValidateAudience = true,
            ValidAudience = "gestionordinateurs-api",
            NameClaimType = "preferred_username",
            RoleClaimType = "roles"
        };

        var backchannelAuthority = builder.Configuration["Keycloak:BackchannelAuthority"];
        if (!string.IsNullOrWhiteSpace(backchannelAuthority))
        {
            if (!Uri.TryCreate(backchannelAuthority, UriKind.Absolute, out var backchannelUri))
            {
                throw new InvalidOperationException(
                    "La configuration Keycloak:BackchannelAuthority doit être une URL absolue.");
            }

            options.BackchannelHttpHandler = new KeycloakBackchannelHandler(
                keycloakAuthorityUri,
                backchannelUri,
                new HttpClientHandler());
        }
    });
builder.Services.AddAuthorization();
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseCors("AllowBlazorClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy().RequireAuthorization();

app.Run();
