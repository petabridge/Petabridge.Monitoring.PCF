// -----------------------------------------------------------------------
// <copyright file="PcfMetricForwarderSettings.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using Akka.Bootstrap.PCF;
using Petabridge.Monitoring.PCF.Impl;
using Petabridge.Monitoring.PCF.Util;

namespace Petabridge.Monitoring.PCF
{
    /// <summary>
    ///     The full range of settings needed to publish to the PCF Metrics forwarder.
    /// </summary>
    /// <remarks>
    ///     Populated by <see cref="PcfEnvironment" />.
    /// </remarks>
    public sealed class PcfMetricForwarderSettings
    {
        public const int DefaultBatchSize = 30;
        public static readonly TimeSpan DefaultReportingInterval = TimeSpan.FromMilliseconds(100);
        public static readonly TimeSpan DefaultHttpTimeoutInterval = TimeSpan.FromSeconds(5);

        public PcfMetricForwarderSettings(PcfIdentity identity, MetricsForwarderCredentials credentials,
            int maximumBatchSize = DefaultBatchSize,
            TimeSpan? maxBatchInterval = null, TimeSpan? pcfHttpTimeout = null, bool debugLogging = false,
            bool errorLogging = true, ITimeProvider timeProvider = null, bool applySuffixes = true)
        {
            Identity = identity;
            Credentials = credentials;
            MaximumBatchSize = maximumBatchSize;
            MaxBatchInterval = maxBatchInterval ?? DefaultReportingInterval;
            PcfHttpTimeout = pcfHttpTimeout ?? DefaultHttpTimeoutInterval;
            DebugLogging = debugLogging;
            ErrorLogging = errorLogging;
            TimeProvider = timeProvider ?? new DateTimeOffsetTimeProvider();
            ApplyMetricsSuffixes = applySuffixes;
        }

        public MetricsForwarderCredentials Credentials { get; }

        /// <summary>
        ///     The identity of this specific application and instance.
        /// </summary>
        public PcfIdentity Identity { get; }

        /// <summary>
        ///     The maximum number of metric instances allowed in a single batch transmission.
        /// </summary>
        public int MaximumBatchSize { get; }

        /// <summary>
        ///     The maxium allowed time interval between batches.
        /// </summary>
        public TimeSpan MaxBatchInterval { get; }

        /// <summary>
        ///     Timeout duration for pushing data to the PCF Metrics Forwarder HTTP API.
        /// </summary>
        public TimeSpan PcfHttpTimeout { get; }

        /// <summary>
        ///     Enables debug logging via the Akka.NET logging channels
        /// </summary>
        public bool DebugLogging { get; }

        /// <summary>
        ///     Enables error logging via the Akka.NET logging channels. Defaults to <c>true</c>.
        /// </summary>
        public bool ErrorLogging { get; }

        /// <summary>
        ///     The <see cref="ITimeProvider" /> used by the metrics reporting system.
        /// </summary>
        public ITimeProvider TimeProvider { get; }

        /// <summary>
        /// When set to <c>true</c>, automatically supplies suffixes to the names of the
        /// counters and timings that are recorded by the <see cref="IPcfMetricRecorder"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        public bool ApplyMetricsSuffixes { get; }

        /// <summary>
        ///     Creates a new set of PCF Metrics Forwarder settings from built-in environment variables.
        /// </summary>
        /// <returns>A new settings instance.</returns>
        public static PcfMetricForwarderSettings FromEnvironment(int maximumBatchSize = DefaultBatchSize,
            TimeSpan? maxBatchInterval = null, TimeSpan? pcfHttpTimeout = null, bool debugLogging = false,
            bool errorLogging = true, ITimeProvider timeProvider = null, bool applySuffixes = true)
        {
            if (PcfEnvironment.IsRunningPcf)
            {
                return new PcfMetricForwarderSettings(new PcfIdentity(
                        PcfEnvironment.Instance.Value.VCAP_APPLICATION.ApplicationId,
                        PcfEnvironment.Instance.Value.CF_INSTANCE_GUID,
                        PcfEnvironment.Instance.Value.CF_INSTANCE_INDEX ?? 0),
                    MetricsCredentialParser.ParseVcapServices(), maximumBatchSize, maxBatchInterval, pcfHttpTimeout,
                    debugLogging, errorLogging, timeProvider, applySuffixes);
            }
            else
            {
                return new PcfMetricForwarderSettings(new PcfIdentity(
                        null,
                        null,
                        0),
                    new MetricsForwarderCredentials(null, null), maximumBatchSize, maxBatchInterval, pcfHttpTimeout,
                    debugLogging, errorLogging, timeProvider, applySuffixes);
            }
        }
    }
}