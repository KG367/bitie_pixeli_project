using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyWasmApp;
using Service;
using BookingSystem;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped(sp => new HttpClient());

builder.Services.AddScoped<JSService>();


builder.Services.AddScoped<ApiClient>();

await builder.Build().RunAsync();