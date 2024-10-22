using System.Security.Claims;

using Microsoft.AspNetCore.Http;

namespace IntelligentPlant.Relativity.AspNetCore {

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
        internal UserClaimTimeZoneProvider(string? claimType = null) {
            ClaimType = string.IsNullOrWhiteSpace(claimType)
                ? DefaultClaimType
                : claimType!;
        }


        /// <inheritdoc/>
        public override ValueTask<TimeZoneInfo?> GetTimeZoneAsync(HttpContext context) {
            if (context.User == null || !context.User.Identities.All(x => x.IsAuthenticated)) {
                return new ValueTask<TimeZoneInfo?>((TimeZoneInfo?) null);
            }

            var claim = context.User.FindFirstValue(ClaimType);
            if (string.IsNullOrWhiteSpace(claim) || !TimeZoneInfo.TryFindSystemTimeZoneById(claim, out var tz)) {
                return new ValueTask<TimeZoneInfo?>((TimeZoneInfo?) null);
            }

            return new ValueTask<TimeZoneInfo?>(tz);
        }

    }
}
