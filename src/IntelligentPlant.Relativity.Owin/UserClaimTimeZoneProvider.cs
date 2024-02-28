using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Owin;

namespace IntelligentPlant.Relativity.Owin {

    /// <summary>
    /// <see cref="TimeZoneProvider"/> that uses a claim from the current user to determine the 
    /// time zone to use.
    /// </summary>
    public sealed class UserClaimTimeZoneProvider : TimeZoneProvider {

        /// <summary>
        /// The default claim type to use for the time zone.
        /// </summary>
        public const string DefaultClaimType = "timezone";

        /// <summary>
        /// The claim type to use for the time zone.
        /// </summary>
        public string ClaimType { get; }


        /// <summary>
        /// Creates a new <see cref="UserClaimTimeZoneProvider"/> instance.
        /// </summary>
        /// <param name="claimType">
        ///   The claim type to use for the time zone. If <see langword="null"/> or white space, 
        ///   <see cref="DefaultClaimType"/> will be used.
        /// </param>
        public UserClaimTimeZoneProvider(string? claimType = null) {
            ClaimType = string.IsNullOrWhiteSpace(claimType)
                ? DefaultClaimType
                : claimType!;
        }


        /// <inheritdoc/>
        public override Task<TimeZoneInfo?> GetTimeZoneAsync(IOwinContext context) {
            if (context.Authentication.User == null || !context.Authentication.User.Identities.All(x => x.IsAuthenticated)) {
                return Task.FromResult<TimeZoneInfo?>(null);
            }

            var claim = context.Authentication.User.FindFirst(ClaimType);
            if (!string.IsNullOrWhiteSpace(claim?.Value)) {
                try {
                    var tz = TimeZoneInfo.FindSystemTimeZoneById(claim!.Value);
                    return Task.FromResult<TimeZoneInfo?>(tz);
                }
                catch { }
            }

            return Task.FromResult<TimeZoneInfo?>(null);
        }
    }
}
