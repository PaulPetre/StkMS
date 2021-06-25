using Microsoft.Extensions.DependencyInjection;
using StkMS.Data.Contracts;
using StkMS.Data.Models;
using StkMS.Data.Services;
using StkMS.Library.Contracts;

namespace StkMS.Data
{
    public static class Bootstrapper
    {
        public static void SetUp(IServiceCollection services)
        {
            services.AddDbContext<StkMSContext>();

            services.AddSingleton<IDataMapper, DataMapper>();

            services.AddScoped<IStkMSContext, StkMSContext>();
            services.AddScoped<IRepository, ProductStockStorage>();
        }
    }
}