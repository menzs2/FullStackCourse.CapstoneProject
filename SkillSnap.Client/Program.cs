using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using SkillSnap.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5165") });
builder.Services.AddScoped<SkillService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<PortfolioService>();
builder.Services.AddAuthentication();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

var host = builder.Build();
// Get JSRuntime and HttpClient
var js = host.Services.GetRequiredService<IJSRuntime>();
var http = host.Services.GetRequiredService<HttpClient>();

// Try to get token from sessionStorage with simple in-memory caching to avoid repeated JS interop
string? token = null;
if (token == null)
{
    token = await js.InvokeAsync<string>("sessionStorage.getItem", "authToken");
}
if (!string.IsNullOrWhiteSpace(token))
{
    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
}

await host.RunAsync();
