var builder = WebApplication.CreateBuilder(args);

var tokenEndpoint = builder.Configuration["Keycloak:TokenEndpoint"];
if (!Uri.TryCreate(tokenEndpoint, UriKind.Absolute, out var tokenEndpointUri))
{
    throw new InvalidOperationException("La configuration Keycloak:TokenEndpoint doit être une URL absolue.");
}
if (!builder.Environment.IsDevelopment() && tokenEndpointUri.Scheme != Uri.UriSchemeHttps)
{
    throw new InvalidOperationException("Keycloak:TokenEndpoint doit utiliser HTTPS hors développement.");
}

var clientId = builder.Configuration["Keycloak:ClientId"];
if (string.IsNullOrWhiteSpace(clientId))
{
    throw new InvalidOperationException("La configuration Keycloak:ClientId est obligatoire.");
}

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
if (allowedOrigins.Length == 0)
{
    throw new InvalidOperationException("Au moins une origine doit être configurée dans Cors:AllowedOrigins.");
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("Keycloak", client => client.BaseAddress = tokenEndpointUri);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowBlazorClient");
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();