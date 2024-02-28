using System;

using Microsoft.Extensions.DependencyInjection;

namespace IntelligentPlant.Relativity.DependencyInjection.Internal {

    /// <summary>
    /// Default <see cref="IRelativityBuilder"/> implementation.
    /// </summary>
    internal class RelativityBuilder : IRelativityBuilder {

        /// <inheritdoc/>
        public IServiceCollection Services { get; }


        /// <summary>
        /// Creates a new <see cref="RelativityBuilder"/> instance.
        /// </summary>
        /// <param name="services">
        ///   The service collection.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="services"/> is <see langword="null"/>.
        /// </exception>
        public RelativityBuilder(IServiceCollection services) {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

    }
}
