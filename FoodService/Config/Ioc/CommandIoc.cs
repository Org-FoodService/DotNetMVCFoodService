﻿using FoodService.Core.Command;
using FoodService.Core.Interface.Command;

namespace FoodService.Config.Ioc
{
    /// <summary>
    /// IoC configuration class for command services.
    /// </summary>
    public static class CommandIoc
    {
        /// <summary>
        /// Configures IoC container for command services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public static void ConfigureCommandIoc(this IServiceCollection services)
        {
            services.AddScoped<IProductCommand, ProductCommand>();
            services.AddScoped<IAuthCommand, AuthCommand>();
        }
    }
}
