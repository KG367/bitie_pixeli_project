using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyWasmApp;
using Service;
using BookingSystem;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<JSService>();
builder.Services.AddScoped<PostgresService>();
builder.Services.AddScoped<BookingService>(); // они как-то сами инициализируют друг-друга, знаю, магия, но оно компилируется, и даже вызываются конструкторы, то бишь работает!


await builder.Build().RunAsync();
