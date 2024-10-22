using System;
using System.Globalization;
using System.Threading;

using IntelligentPlant.Relativity.Internal;

namespace IntelligentPlant.Relativity {

    /// <summary>
    /// <see cref="RelativityParser"/> defines static properties for working with <see cref="IRelativityParser"/> 
    /// instances.
    /// </summary>
    public static class RelativityParser {

        /// <summary>
        /// An <see cref="IRelativityParser"/> that uses the invariant culture and the time zone 
        /// of the local machine.
        /// </summary>
        public static IRelativityParser Invariant { get; } = new Parser(new RelativityParserConfiguration() {
            CultureInfo = CultureInfo.InvariantCulture,
            BaseTimeSettings = new RelativityBaseTimeSettings(),
            TimeOffsetSettings = new RelativityTimeOffsetSettings()
        }, TimeZoneInfo.Local);

        /// <summary>
        /// An <see cref="IRelativityParser"/> that uses the invariant culture and the UTC time 
        /// zone.
        /// </summary>
        public static IRelativityParser InvariantUtc { get; } = new Parser(new RelativityParserConfiguration() {
            CultureInfo = CultureInfo.InvariantCulture,
            BaseTimeSettings = new RelativityBaseTimeSettings(),
            TimeOffsetSettings = new RelativityTimeOffsetSettings()
        }, TimeZoneInfo.Utc);


        /// <summary>
        /// The <see cref="IRelativityParser"/> for the current asynchronous control flow.
        /// </summary>
        private static readonly AsyncLocal<IRelativityParser> s_current = new AsyncLocal<IRelativityParser>();

        /// <summary>
        /// The <see cref="IRelativityParser"/> for the current asynchronous control flow.
        /// </summary>
        /// <remarks>
        /// 
        /// <para>
        ///   By default, <see cref="Current"/> is set to <see cref="InvariantUtc"/>.
        /// </para>
        /// 
        /// <para>
        ///   The value of <see cref="Current"/> is maintained for the duration of the current 
        ///   asynchronous control flow. This means that you can set its value at the start of an 
        ///   asynchronous call stack and it will be available to all methods that run on the 
        ///   same call stack, even after awaiting asynchronous tasks. 
        /// </para>
        /// 
        /// <para>
        ///   Using <see cref="Current"/> is useful for scenarios where you want to use a specific 
        ///   parser for the duration of an operation, but you don't want to or cannot pass the 
        ///   parser as a parameter to every method in the call stack.
        /// </para>
        /// 
        /// <para>
        ///   Please refer to the documentation for <see cref="AsyncLocal{T}"/> for more information 
        ///   about how asynchronous control flow data is managed.
        /// </para>
        /// 
        /// </remarks>
        /// <seealso cref="AsyncLocal{T}"/>
        public static IRelativityParser Current {
            get {
                return s_current.Value ?? InvariantUtc;
            }
            set {
                s_current.Value = value ?? InvariantUtc;
            }
        }

    }
}
