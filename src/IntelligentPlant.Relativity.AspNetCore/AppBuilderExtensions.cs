using Microsoft.AspNetCore.Builder;

namespace IntelligentPlant.Relativity.AspNetCore {

    /// <summary>
    /// Extension methods for <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class ApplicationBuilderExtensions {

        /// <summary>
        /// Adds the Relativity middleware to the application pipeline.
        /// </summary>
        /// <param name="app">
        ///   The application builder.
        /// </param>
        /// <returns>
        ///   The application builder.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="app"/> is <see langword="null"/>.
        /// </exception>
        public static IApplicationBuilder UseRelativity(this IApplicationBuilder app) {
            if (app == null) {
                throw new System.ArgumentNullException(nameof(app));
            }

            app.UseMiddleware<RelativityMiddleware>();
            return app;
        }

    }
}
