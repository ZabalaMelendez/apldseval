using Microsoft.Extensions.DependencyInjection;
using System;

namespace Security.API.Application
{
    public static class ApplicationApiModule
    {

        /// <summary>
        /// Add Applications commons service to DI
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplicationModule(this IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));


            return services;
        }

    }
}
