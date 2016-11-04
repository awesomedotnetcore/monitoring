﻿using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Greentube.Monitoring
{
    /// <summary>
    /// Registers services required by Monitoring library
    /// </summary>
    public static class MonitoringServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the monitoring services
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddMonitoring(
            this IServiceCollection services,
            Action<MonitoringOptions> optionsAction = null)
        {
            var options = new MonitoringOptions();
            optionsAction?.Invoke(options);

            return services
                .AddSingleton<IResourceMonitorConfiguration>(options)
                .AddSingleton<IResourceStateCollector>(p =>
                {
                    var resourceMonitors = options
                        .Factories
                        .Select(f => f(options, p))
                        .Concat(p.GetServices<IResourceMonitor>());

                    var logger = p.GetRequiredService<ILogger<ResourceStateCollector>>();
                    return new ResourceStateCollector(resourceMonitors, options.MaxStatePerResource, logger);
                });
        }
    }
}
