using System.Globalization;

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
        /// <remarks>
        /// 
        /// <para>
        ///   Adding the Relativity middleware to the pipeline will set <see cref="RelativityParser.Current"/> 
        ///   for each HTTP request.
        /// </para>
        /// 
        /// <para>
        ///   To enable culture-specific Relativity parsers to be used, you must use ASP.NET Core's 
        ///   request localisation middleware to set <see cref="CultureInfo.CurrentCulture"/> and 
        ///   <see cref="CultureInfo.CurrentUICulture"/> prior to invoking the Relativity middleware.
        /// </para>
        /// 
        /// </remarks>
        public static IApplicationBuilder UseRelativity(this IApplicationBuilder app) {
            if (app == null) {
                throw new ArgumentNullException(nameof(app));
            }

            app.UseMiddleware<RelativityMiddleware>();
            return app;
        }

    }
}
