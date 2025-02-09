using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using PicusPicks.Web.Components;
using PicusPicks.Web.Configuration;
using PicusPicks.Web.Services;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add configuration loading - environment variables last to override file settings
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Debug log all configuration values
var logger = LoggerFactory.Create(config =>
{
    config.AddConsole();
}).CreateLogger("Program");

logger.LogInformation("Configuration values:");
foreach (var config in builder.Configuration.AsEnumerable().OrderBy(x => x.Key))
{
    logger.LogInformation("{Key}: {Value}", config.Key, config.Value);
}

// Get API base URL with environment variables taking precedence
var apiBaseUrl = builder.Configuration["API_BASE_URL"]  // Try environment variable first
    ?? builder.Configuration["APIBASEURL"]
    ?? builder.Configuration["ApiBaseUrl"]
    ?? "http://api:8080";

logger.LogInformation("Selected API Base URL: {BaseUrl}", apiBaseUrl);

// Configure Auth0 settings
var auth0Settings = builder.Configuration.GetSection(Auth0Settings.SectionName).Get<Auth0Settings>();
builder.Services.Configure<Auth0Settings>(builder.Configuration.GetSection(Auth0Settings.SectionName));

// Add MudBlazor services
builder.Services.AddMudServices();

// Add authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.Name = "PicusPicks.Auth";
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/LogOut";
})
.AddOpenIdConnect(options =>
{
    options.Authority = $"https://{auth0Settings?.Domain}";
    options.ClientId = auth0Settings?.ClientId;
    options.ClientSecret = auth0Settings?.ClientSecret;
    options.ResponseType = "code";
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.CallbackPath = auth0Settings?.CallbackPath ?? "/callback";
    options.ClaimsIssuer = "Auth0";
    options.SaveTokens = true;
    
    options.Events = new OpenIdConnectEvents
    {
        OnRedirectToIdentityProvider = context =>
        {
            context.ProtocolMessage.SetParameter("audience", auth0Settings?.Audience);
            return Task.CompletedTask;
        }
    };
});

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add authorization services
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, PicusAuthenticationStateProvider>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add controller support for Account controller
builder.Services.AddControllers();

// Configure HTTP client for API communication
var jsonOptions = new System.Text.Json.JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
};

builder.Services.AddHttpClient<IGamesService, GamesService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    logger.LogInformation("Configuring GamesService with base URL: {BaseUrl}", apiBaseUrl);
}).ConfigureHttpClient(client =>
{
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

builder.Services.AddHttpClient<IPicksService, PicksService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    logger.LogInformation("Configuring PicksService with base URL: {BaseUrl}", apiBaseUrl);
}).ConfigureHttpClient(client =>
{
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

builder.Services.AddHttpClient<ILeagueTableService, LeagueTableService>((serviceProvider, client) =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    logger.LogInformation("Configuring LeagueTableService with base URL: {BaseUrl}", apiBaseUrl);
}).ConfigureHttpClient(client =>
{
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | 
                              ForwardedHeaders.XForwardedProto | 
                              ForwardedHeaders.XForwardedHost;
    // Clear known networks and proxies since we're behind Render's proxy
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// Add this as early as possible in the middleware pipeline
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// Map controllers
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
