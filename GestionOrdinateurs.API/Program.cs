using GestionOrdinateurs.API.Services;
using GestionOrdinateurs.API.Services.Interfaces;
using GestionOrdinateurs.API.Models;
using GestionOrdinateurs.Shared.Authentification;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text.Json.Serialization;

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

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GestionOrdinateurs",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrez : Bearer {token}"
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = keycloakAuthorityUri.ToString().TrimEnd('/');
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.SaveToken = true;
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

            options.BackchannelHttpHandler = new GestionOrdinateurs.Shared.KeycloakBackchannelHandler(
                keycloakAuthorityUri,
                backchannelUri,
                new HttpClientHandler());
        }
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JwtBearer");
                logger.LogInformation(
                    "Utilisateur utilisant un token valide : {User}",
                    context.Principal?.Identity?.Name ?? "Anonymous");
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JwtBearer");
                logger.LogWarning(context.Exception, "Token JWT invalide.");
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddScoped<IOrdinateurService, OrdinateurService>();
builder.Services.AddScoped<IReparationService, ReparationService>();
builder.Services.AddSingleton<JournalService>();
builder.Services.AddSingleton<StockService>();
builder.Services.AddLogging();
builder.Services.AddDbContext<ParcInformatiqueContext>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowBlazorClient");
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Initialisation de la base de données
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ParcInformatiqueContext>();

    db.Database.Migrate();

    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new User { Login = "user1", Role = RoleEnum.Admin },
            new User { Login = "user2", Role = RoleEnum.Gestionnaire },
            new User { Login = "user3", Role = RoleEnum.Gestionnaire }
        );

        db.SaveChanges();
    }
}

app.Run();