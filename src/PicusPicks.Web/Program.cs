using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using PicusPicks.Web.Components;
using PicusPicks.Web.Configuration;
using PicusPicks.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Auth0 settings
var auth0Settings = builder.Configuration.GetSection(Auth0Settings.SectionName).Get<Auth0Settings>();
builder.Services.Configure<Auth0Settings>(builder.Configuration.GetSection(Auth0Settings.SectionName));

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
builder.Services.AddHttpClient<IGamesService, GamesService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5172/");
});

var app = builder.Build();

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
