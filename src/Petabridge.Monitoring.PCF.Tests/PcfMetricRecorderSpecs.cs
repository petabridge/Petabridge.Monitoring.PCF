// -----------------------------------------------------------------------
// <copyright file="PcfMetricRecorderSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using Akka.Actor;
using Akka.TestKit.Xunit2;
using FluentAssertions;
using Petabridge.Monitoring.PCF.Impl.Actors;
using Xunit;

namespace Petabridge.Monitoring.PCF.Tests
{
    public class PcfMetricRecorderSpecs : TestKit
    {
        [Fact(DisplayName = "PcfMetricsRecorder should not capture metrics with suffixes when configured not to")]
        public void ShouldNotRecordMetricsWithSuffixesWhenSpecified()
        {
            var settings = PcfMetricForwarderSettings.FromEnvironment(applySuffixes: false);
            var counterActor = Sys.ActorOf(Props.Create(() => new CounterAggregator(TestActor, settings.TimeProvider)),
                "counter");
            var pcfRecorder = new PcfMetricRecorder(settings, TestActor, counterActor);
            pcfRecorder.RecordGauge("foo", 1.0d);

            // gauges never have suffixes
            ExpectMsg<PcfMetricRecording>().Name.Should().Be("foo");

            // timings do
            pcfRecorder.RecordTiming("foo", 1000);
            ExpectMsg<PcfMetricRecording>().Name.Should().Be("foo");
        }

        [Fact(DisplayName = "PcfMetricsRecorder should capture metrics with suffixes by default")]
        public void ShouldRecordMetricsWithSuffixesByDefault()
        {
            var settings = PcfMetricForwarderSettings.FromEnvironment();
            var counterActor = Sys.ActorOf(Props.Create(() => new CounterAggregator(TestActor, settings.TimeProvider)),
                "counter");
            var pcfRecorder = new PcfMetricRecorder(settings, TestActor, counterActor);
            pcfRecorder.RecordGauge("foo", 1.0d);

            // gauges never have suffixes
            ExpectMsg<PcfMetricRecording>().Name.Should().Be("foo");

            // timings do
            pcfRecorder.RecordTiming("foo", 1000);
            ExpectMsg<PcfMetricRecording>().Name.Should().Be("foo.duration");
        }
    }
}