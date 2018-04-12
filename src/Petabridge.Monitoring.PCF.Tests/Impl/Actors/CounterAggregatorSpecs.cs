// -----------------------------------------------------------------------
// <copyright file="CounterAggregatorSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using FluentAssertions;
using Petabridge.Monitoring.PCF.Impl.Actors;
using Petabridge.Monitoring.PCF.Util;
using Xunit;

namespace Petabridge.Monitoring.PCF.Tests.Impl.Actors
{
    public class CounterAggregatorSpecs : TestKit
    {
        [Fact(DisplayName = "CounterAggregator should accumulate increments and decrements for correct metrics")]
        public void CounterAggregatorShouldAggregateIncrements()
        {
            var counter =
                Sys.ActorOf(
                    Props.Create(() =>
                        new CounterAggregator(TestActor, new DateTimeOffsetTimeProvider(), TimeSpan.FromHours(1))),
                    "counter");

            counter.Tell(new CounterAggregator.CounterIncrement("foo", 1));
            counter.Tell(new CounterAggregator.CounterIncrement("foo", 1));
            counter.Tell(new CounterAggregator.CounterIncrement("foo", 1));
            counter.Tell(new CounterAggregator.CounterIncrement("bar", 1));
            counter.Tell(new CounterAggregator.CounterIncrement("bar", -1)); //decrement
            counter.Tell(CounterAggregator.Flush.Instance);

            var metrics = ReceiveN(2).Cast<PcfMetricRecording>().ToDictionary(x => x.Name, x => x);
            metrics["foo"].Value.Should().Be(3.0D);
            metrics["bar"].Value.Should().Be(0.0D);
        }

        [Fact(DisplayName = "CounterAggregator should reset aggregates upon flush")]
        public void CounterAggregatorShouldResetAggregates()
        {
            var counter =
                Sys.ActorOf(
                    Props.Create(() =>
                        new CounterAggregator(TestActor, new DateTimeOffsetTimeProvider(), TimeSpan.FromHours(1))),
                    "counter");

            counter.Tell(new CounterAggregator.CounterIncrement("foo", 1));
            counter.Tell(new CounterAggregator.CounterIncrement("foo", 1));
            counter.Tell(new CounterAggregator.CounterIncrement("foo", 1));
            counter.Tell(new CounterAggregator.CounterIncrement("bar", 1));
            counter.Tell(new CounterAggregator.CounterIncrement("bar", -1)); //decrement
            counter.Tell(CounterAggregator.Flush.Instance);

            var metrics = ReceiveN(2).Cast<PcfMetricRecording>().ToDictionary(x => x.Name, x => x);

            counter.Tell(new CounterAggregator.CounterIncrement("baz", 1));
            counter.Tell(CounterAggregator.Flush.Instance);

            ExpectMsg<PcfMetricRecording>().Name.Should().Be("baz");
        }
    }
}