// -----------------------------------------------------------------------
// <copyright file="IPcfMetricRecorder.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Petabridge.Monitoring.PCF
{
    /// <summary>
    ///     Handle on the underlying engine that will carry out the recording of metrics.
    /// </summary>
    public interface IPcfMetricRecorder
    {
        void IncrementCounter(string name, int value = 1, double sampleRate = PcfMetricRecording.DefaultSampleRate);
        void DecrementCounter(string name, int value = -1, double sampleRate = PcfMetricRecording.DefaultSampleRate);

        void IncrementCounter(string name, long timestamp, int value = 1,
            double sampleRate = PcfMetricRecording.DefaultSampleRate);

        void DecrementCounter(string name, long timestamp, int value = -1,
            double sampleRate = PcfMetricRecording.DefaultSampleRate);

        void RecordGauge(string name, double value, double sampleRate = PcfMetricRecording.DefaultSampleRate);

        void RecordGauge(string name, long timestamp, double value,
            double sampleRate = PcfMetricRecording.DefaultSampleRate);

        void RecordTiming(string name, long value, double sampleRate = PcfMetricRecording.DefaultSampleRate);

        void RecordTiming(string name, long timestamp, long value,
            double sampleRate = PcfMetricRecording.DefaultSampleRate);
    }

    /// <summary>
    ///     A <see cref="IPcfMetricRecorder" /> implementation that uses Akka.NET actors to aggregate
    ///     counters and other metrics
    /// </summary>
    public sealed class PcfMetricRecorder : IPcfMetricRecorder
    {
        public const string CounterPostfix = "";
        public const string TimingPostfix = "";

        public void IncrementCounter(string name, int value = 1,
            double sampleRate = PcfMetricRecording.DefaultSampleRate)
        {
            throw new NotImplementedException();
        }

        public void DecrementCounter(string name, int value = -1,
            double sampleRate = PcfMetricRecording.DefaultSampleRate)
        {
            throw new NotImplementedException();
        }

        public void IncrementCounter(string name, long timestamp, int value = 1,
            double sampleRate = PcfMetricRecording.DefaultSampleRate)
        {
            throw new NotImplementedException();
        }

        public void DecrementCounter(string name, long timestamp, int value = -1,
            double sampleRate = PcfMetricRecording.DefaultSampleRate)
        {
            throw new NotImplementedException();
        }

        public void RecordGauge(string name, double value, double sampleRate = PcfMetricRecording.DefaultSampleRate)
        {
            throw new NotImplementedException();
        }

        public void RecordGauge(string name, long timestamp, double value,
            double sampleRate = PcfMetricRecording.DefaultSampleRate)
        {
            throw new NotImplementedException();
        }

        public void RecordTiming(string name, long value, double sampleRate = PcfMetricRecording.DefaultSampleRate)
        {
            throw new NotImplementedException();
        }

        public void RecordTiming(string name, long timestamp, long value,
            double sampleRate = PcfMetricRecording.DefaultSampleRate)
        {
            throw new NotImplementedException();
        }
    }
}