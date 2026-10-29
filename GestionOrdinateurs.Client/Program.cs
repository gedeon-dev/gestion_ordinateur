using Blazored.SessionStorage;
using GestionOrdinateurs.Client.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddBlazoredSessionStorage();

var authenticationApiBaseAddress = builder.Configuration["AuthenticationApi:BaseAddress"];
if (!Uri.TryCreate(authenticationApiBaseAddress, UriKind.Absolute, out var authenticationApiUri))
{
    throw new InvalidOperationException("La configuration AuthenticationApi:BaseAddress doit être une URL absolue.");
}

builder.Services.AddHttpClient("AuthenticationApi", client =>
    client.BaseAddress = authenticationApiUri);

var managementApiBaseAddress = builder.Configuration["ManagementApi:BaseAddress"];
if (!Uri.TryCreate(managementApiBaseAddress, UriKind.Absolute, out var managementApiUri))
{
    throw new InvalidOperationException("La configuration ManagementApi:BaseAddress doit être une URL absolue.");
}

builder.Services.AddHttpClient("ManagementApi", client =>
    client.BaseAddress = managementApiUri);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();