using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StkMS.Client.Services.Interfaces;

namespace StkMS.Client.Services
{
    public static class DependencyInjectionExtensions
    {

        public static IServiceCollection AddHttpClientServices(this IServiceCollection services)
        {
            return services.AddScoped<IAuthenticationService, HttpAuthenticationService>();
        }

    }
}
