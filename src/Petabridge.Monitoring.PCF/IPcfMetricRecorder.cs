// -----------------------------------------------------------------------
// <copyright file="IPcfMetricRecorder.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

namespace Petabridge.Monitoring.PCF
{
    /// <summary>
    ///     Handle on the underlying engine that will carry out the recording of metrics.
    /// </summary>
    public interface IPcfMetricRecorder
    {
        /// <summary>
        ///     The settings for connecting to the PCF metrics forwarder.
        /// </summary>
        PcfMetricForwarderSettings Settings { get; }

        void IncrementCounter(string name, int value = 1);
        void DecrementCounter(string name, int value = -1);

        void RecordGauge(string name, double value);

        void RecordTiming(string name, long value);
    }
}