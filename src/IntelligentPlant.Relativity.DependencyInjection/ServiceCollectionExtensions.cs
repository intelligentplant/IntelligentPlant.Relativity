using System;

using Microsoft.Extensions.DependencyInjection;

namespace IntelligentPlant.Relativity.DependencyInjection {

    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions {

        /// <summary>
        /// Adds Relativity services to the service collection.
        /// </summary>
        /// <param name="services">
        ///   The service collection.
        /// </param>
        /// <returns>
        ///   An <see cref="IRelativityBuilder"/> that can be used to configure Relativity services.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="services"/> is <see langword="null"/>.
        /// </exception>
        public static IRelativityBuilder AddRelativity(this IServiceCollection services) {
            if (services == null) {
                throw new ArgumentNullException(nameof(services));
            }

            var builder = new Internal.RelativityBuilder(services);
            builder.Services.AddSingleton<IRelativityParserFactory, RelativityParserFactory>();

            return builder;
        }

    }
}
