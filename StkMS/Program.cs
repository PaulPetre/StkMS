using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using PdfSharpCore.Fonts;
using Radzen;
using StkMS.Contracts;
using StkMS.Library.Contracts;
using StkMS.Library.Services;
using StkMS.Client.Services;
using StkMS.Client.Services.Interfaces;
using StkMS.Services;

namespace StkMS
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            //Authentication
            builder.Services
                .AddHttpClient("PlannerApp.Api",
                    client => { client.BaseAddress = new Uri("https://plannerapp-api.azurewebsites.net"); })
                .AddHttpMessageHandler<AuthorizationMessageHandler>();
            builder.Services.AddTransient<AuthorizationMessageHandler>();

            builder.Services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("PlannerApp.Api"));
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
            builder.Services.AddHttpClientServices();

            GlobalFontSettings.FontResolver = new FontResolver();

            builder.Services.AddBlazoredLocalStorage();

            

            builder.Services.AddScoped<IMapper, Mapper>();
            builder.Services.AddScoped<ICache, LocalStorageCache>();
            builder.Services.AddScoped<IApiClient>(
                sp => new ApiClientThreadSafeDecorator(
                    new ApiClientCachingDecorator(
                        new ApiClient(),
                        sp.GetRequiredService<ICache>()
                    )
                )
            );
            builder.Services.AddScoped<IInventory, Inventory>();
            builder.Services.AddScoped<IReportGenerator, ReportGenerator>();
            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<TooltipService>();
            builder.Services.AddScoped<ContextMenuService>();
            builder.Services.AddMudServices();
            builder.Services.AddMudBlazorDialog();

            return builder.Build().RunAsync();
        }

    }
}

