using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Web.Client.ApiServices;
using Blazored.LocalStorage;
namespace Web.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddAntDesign();
            var config = builder.Configuration;
            var apiBaseAddress = config["ApiSettings:BaseAddress"];
            HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiBaseAddress)
            };
            builder.Services.AddScoped(sp => httpClient);
            builder.Services.AddScoped<CourtScheduleService>();
            builder.Services.AddScoped<SlotService>();
            builder.Services.AddScoped<BookingService>();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseAddress) });
            builder.Services.AddBlazoredLocalStorage();
            await builder.Build().RunAsync();
        }
    }
}
