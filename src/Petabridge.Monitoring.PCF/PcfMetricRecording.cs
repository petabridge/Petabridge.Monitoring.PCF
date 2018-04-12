// -----------------------------------------------------------------------
// <copyright file="PcfMetricRecording.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Petabridge.Monitoring.PCF
{
    /// <summary>
    ///     A single recording a piece of metric data that will be sent downstream to the PCF metrics forwarder.
    /// </summary>
    public struct PcfMetricRecording
    {
        /// <summary>
        ///     The global default sample rate.
        /// </summary>
        public const double DefaultSampleRate = 1.0d;

        public PcfMetricRecording(string name, double value, string unit, long timestamp,
            IReadOnlyDictionary<string, string> tags)
        {
            Name = name;
            Value = value;
            Unit = unit;
            Timestamp = timestamp;
            Tags = tags;
        }

        /// <summary>
        ///     The name of this metric.
        /// </summary>
        /// <remarks>
        ///     Recommended to use the StatsD format: lowercase, separated by .
        /// </remarks>
        /// <example>
        ///     akka.actor.MyActor.recv
        /// </example>
        public string Name { get; }

        /// <summary>
        ///     The only type of metric supported by the PCF metrics forwarder at this time.
        /// </summary>
        public const string GaugeMetric = "gauge";

        public string Type => GaugeMetric;

        /// <summary>
        ///     The 64-bit value of this metric.
        /// </summary>
        public double Value { get; }

        /// <summary>
        ///     The unit of measure for this metric.
        /// </summary>
        public string Unit { get; }

        /// <summary>
        ///     The unix timestamp in milliseconds.
        /// </summary>
        public long Timestamp { get; }

        /// <summary>
        ///     Meta-data used to help codify this metric. Can be empty.
        /// </summary>
        public IReadOnlyDictionary<string, string> Tags { get; }
    }
}