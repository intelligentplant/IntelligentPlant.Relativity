using IntelligentPlant.Relativity.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace IntelligentPlant.Relativity.AspNetCore {

    /// <summary>
    /// Extension methods for <see cref="IRelativityBuilder"/>.
    /// </summary>
    public static class RelativityBuilderExtensions {

        /// <summary>
        /// Adds a query string time zone provider to the Relativity services.
        /// </summary>
        /// <param name="builder">
        ///   The builder.
        /// </param>
        /// <param name="key">
        ///   The query string key to use for the time zone. Specify <see langword="null"/> or 
        ///   white space to use <see cref="QueryStringTimeZoneProvider.DefaultQueryStringKey"/>.
        /// </param>
        /// <returns>
        ///   The builder.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="builder"/> is <see langword="null"/>.
        /// </exception>
        public static IRelativityBuilder AddQueryStringTimeZoneProvider(this IRelativityBuilder builder, string? key = null) {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddSingleton(sp => {
                return new QueryStringTimeZoneProvider(key);
            });

            return builder;
        }


        /// <summary>
        /// Adds a cookie time zone provider to the Relativity services.
        /// </summary>
        /// <param name="builder">
        ///   The builder.
        /// </param>
        /// <param name="cookieName">
        ///   The cookie name to use for the time zone. Specify <see langword="null"/> or 
        ///   white space to use <see cref="CookieTimeZoneProvider.DefaultCookieName"/>.
        /// </param>
        /// <returns>
        ///   The builder.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="builder"/> is <see langword="null"/>.
        /// </exception>
        public static IRelativityBuilder AddCookieTimeZoneProvider(this IRelativityBuilder builder, string? cookieName = null) {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddSingleton(sp => {
                return new CookieTimeZoneProvider(cookieName);
            });

            return builder;
        }


        /// <summary>
        /// Adds a request header time zone provider to the Relativity services.
        /// </summary>
        /// <param name="builder">
        ///   The builder.
        /// </param>
        /// <param name="headerName">
        ///   The header name to use for the time zone. Specify <see langword="null"/> or 
        ///   white space to use <see cref="RequestHeaderTimeZoneProvider.DefaultHeaderName"/>.
        /// </param>
        /// <returns>
        ///   The builder.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="builder"/> is <see langword="null"/>.
        /// </exception>
        public static IRelativityBuilder AddRequestHeaderTimeZoneProvider(this IRelativityBuilder builder, string? headerName = null) {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddSingleton(sp => {
                return new RequestHeaderTimeZoneProvider(headerName);
            });

            return builder;
        }


        /// <summary>
        /// Adds a user claim time zone provider to the Relativity services.
        /// </summary>
        /// <param name="builder">
        ///   The builder.
        /// </param>
        /// <param name="claimType">
        ///   The claim type to use for the time zone. Specify <see langword="null"/> or white 
        ///   space to use <see cref="UserClaimTimeZoneProvider.ClaimType"/>.
        /// </param>
        /// <returns>
        ///   The builder.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="builder"/> is <see langword="null"/>.
        /// </exception>
        public static IRelativityBuilder AddUserClaimTimeZoneProvider(this IRelativityBuilder builder, string? claimType = null) {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddSingleton(sp => {
                return new UserClaimTimeZoneProvider(claimType);
            });

            return builder;
        }


        /// <summary>
        /// Adds a custom time zone provider to the Relativity services.
        /// </summary>
        /// <typeparam name="T">
        ///   The type of the time zone provider.
        /// </typeparam>
        /// <param name="builder">
        ///   The builder.
        /// </param>
        /// <returns>
        ///   The builder.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="builder"/> is <see langword="null"/>.
        /// </exception>
        public static IRelativityBuilder AddTimeZoneProvider<T>(this IRelativityBuilder builder) where T : CustomTimeZoneProvider {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddScoped<CustomTimeZoneProvider, T>();

            return builder;
        }


        /// <summary>
        /// Adds a custom time zone provider to the Relativity services.
        /// </summary>
        /// <typeparam name="T">
        ///   The type of the time zone provider.
        /// </typeparam>
        /// <param name="builder">
        ///   The builder.
        /// </param>
        /// <param name="instanceValue">
        ///   The instance of the time zone provider to add to the services.
        /// </param>
        /// <returns>
        ///   The builder.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="builder"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="instanceValue"/> is <see langword="null"/>.
        /// </exception>
        public static IRelativityBuilder AddTimeZoneProvider<T>(this IRelativityBuilder builder, T instanceValue) where T : CustomTimeZoneProvider {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }
            if (instanceValue == null) {
                throw new ArgumentNullException(nameof(instanceValue));
            }

            builder.Services.AddSingleton<CustomTimeZoneProvider>(instanceValue);

            return builder;
        }


        /// <summary>
        /// Adds a custom time zone provider to the Relativity services.
        /// </summary>
        /// <typeparam name="T">
        ///   The type of the time zone provider.
        /// </typeparam>
        /// <param name="builder">
        ///   The builder.
        /// </param>
        /// <param name="instanceFactory">
        ///   The instance factory for the time zone provider to add to the services.
        /// </param>
        /// <returns>
        ///   The builder.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="builder"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="instanceFactory"/> is <see langword="null"/>.
        /// </exception>
        public static IRelativityBuilder AddTimeZoneProvider<T>(this IRelativityBuilder builder, Func<IServiceProvider, T> instanceFactory) where T : CustomTimeZoneProvider {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }
            if (instanceFactory == null) {
                throw new ArgumentNullException(nameof(instanceFactory));
            }

            builder.Services.AddScoped<CustomTimeZoneProvider>(instanceFactory);

            return builder;
        }

    }
}
