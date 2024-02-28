using System;

using Microsoft.Extensions.DependencyInjection;

namespace IntelligentPlant.Relativity.DependencyInjection {

    /// <summary>
    /// Extension methods for <see cref="IRelativityBuilder"/>.
    /// </summary>
    public static class RelativityBuilderExtensions {

        /// <summary>
        /// Adds a Relativity parser configuration.
        /// </summary>
        /// <param name="builder">
        ///   The builder.
        /// </param>
        /// <param name="configuration">
        ///   The parser configuration to add.
        /// </param>
        /// <returns>
        ///   The builder.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="builder"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="configuration"/> is <see langword="null"/>.
        /// </exception>
        public static IRelativityBuilder AddParserConfiguration(this IRelativityBuilder builder, RelativityParserConfiguration configuration) {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }
            if (configuration == null) {
                throw new ArgumentNullException(nameof(configuration));
            }

            builder.Services.AddSingleton(configuration);

            return builder;
        }

    }
}
