using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using SkillSnap.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<SkillService>();
builder.Services.AddScoped<ProjectService>();
var host = builder.Build();
// Get JSRuntime and HttpClient
var js = host.Services.GetRequiredService<IJSRuntime>();
var http = host.Services.GetRequiredService<HttpClient>();

// Try to get token from sessionStorage
var token = await js.InvokeAsync<string>("sessionStorage.getItem", "authToken");
if (!string.IsNullOrWhiteSpace(token))
{
    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
}
await host.RunAsync();
