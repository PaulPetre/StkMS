using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PdfSharpCore.Fonts;
using StkMS.Contracts;
using StkMS.Library.Contracts;
using StkMS.Library.Models;
using StkMS.Services;
using StkMS.SqlData.Services;

namespace StkMS
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            GlobalFontSettings.FontResolver = new FontResolver();

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<IMapper, Mapper>();
            builder.Services.AddScoped<IStock, Stock>();
            builder.Services.AddScoped<IInventory, Inventory>();
            builder.Services.AddScoped<IReportGenerator, ReportGenerator>();
            builder.Services.AddScoped<IStorage<ProductStock>, Storage<ProductStock>>();

            return builder.Build().RunAsync();
        }
    }
}