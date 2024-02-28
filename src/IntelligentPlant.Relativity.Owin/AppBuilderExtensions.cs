using System;
using System.Collections.Generic;
using System.Globalization;

using Owin;

namespace IntelligentPlant.Relativity.Owin {

    /// <summary>
    /// Extension methods for <see cref="IAppBuilder"/>.
    /// </summary>
    public static class AppBuilderExtensions {

        /// <summary>
        /// Adds the Relativity middleware to the application pipeline.
        /// </summary>
        /// <param name="app">
        ///   The application builder.
        /// </param>
        /// <param name="factory">
        ///   The parser factory to use for the middleware.
        /// </param>
        /// <param name="timeZoneProviders">
        ///   The time zone providers to use for the middleware.
        /// </param>
        /// <returns>
        ///   The application builder.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="app"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="factory"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        /// 
        /// <para>
        ///   Adding the Relativity middleware to the pipeline will set <see cref="RelativityParser.Current"/> 
        ///   for each HTTP request.
        /// </para>
        /// 
        /// <para>
        ///   Note that, unlike ASP.NET Core, OWIN does not have built-in capababilities for setting <see cref="CultureInfo.CurrentCulture"/> 
        ///   or <see cref="CultureInfo.CurrentUICulture"/> for the request. Therefore, you will 
        ///   need to set these manually in the application pipeline prior to the Relativity 
        ///   middleware if you want to allow culture-specific Relativity parsers to be used.
        /// </para>
        ///    
        /// </remarks>
        public static IAppBuilder UseRelativity(this IAppBuilder app, IRelativityParserFactory factory, params TimeZoneProvider[] timeZoneProviders)
            => UseRelativity(app, factory, (IEnumerable<TimeZoneProvider>) timeZoneProviders);


        /// <summary>
        /// Adds the Relativity middleware to the application pipeline.
        /// </summary>
        /// <param name="app">
        ///   The application builder.
        /// </param>
        /// <param name="factory">
        ///   The parser factory to use for the middleware.
        /// </param>
        /// <param name="timeZoneProviders">
        ///   The time zone providers to use for the middleware.
        /// </param>
        /// <returns>
        ///   The application builder.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="app"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="factory"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        /// 
        /// <para>
        ///   Adding the Relativity middleware to the pipeline will set <see cref="RelativityParser.Current"/> 
        ///   for each HTTP request.
        /// </para>
        /// 
        /// <para>
        ///   Note that, unlike ASP.NET Core, OWIN does not have built-in capababilities for setting <see cref="CultureInfo.CurrentCulture"/> 
        ///   or <see cref="CultureInfo.CurrentUICulture"/> for the request. Therefore, you will 
        ///   need to set these manually in the application pipeline prior to the Relativity 
        ///   middleware if you want to allow culture-specific Relativity parsers to be used.
        /// </para>
        ///    
        /// </remarks>
        public static IAppBuilder UseRelativity(this IAppBuilder app, IRelativityParserFactory factory, IEnumerable<TimeZoneProvider> timeZoneProviders) {
            if (app == null) {
                throw new ArgumentNullException(nameof(app));
            }
            if (factory == null) {
                throw new ArgumentNullException(nameof(app));
            }
            app.Use<RelativityMiddleware>(app, factory, timeZoneProviders);

            return app;
        }

    }
}
