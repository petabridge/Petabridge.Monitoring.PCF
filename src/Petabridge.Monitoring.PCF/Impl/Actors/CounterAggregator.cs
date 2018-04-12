// -----------------------------------------------------------------------
// <copyright file="CounterAggregator.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Akka.Actor;
using Phobos.Actor.Common;

namespace Petabridge.Monitoring.PCF.Impl.Actors
{
    /// <summary>
    ///     INTERNAL API.
    ///     Gathers counter metric events and aggregates them before flushing downstream.
    /// </summary>
    internal sealed class CounterAggregator : ReceiveActor, INeverMonitor, INeverTrace
    {
        public static readonly TimeSpan DefaultFlushInterval = TimeSpan.FromSeconds(1);

        private readonly Dictionary<string, int> _counters = new Dictionary<string, int>();
        private readonly TimeSpan _flushInterval;
        private readonly IActorRef _reportingActor;
        private readonly ITimeProvider _timeProvider;

        private ICancelable _flushTimer;

        public CounterAggregator(IActorRef reportingActor, ITimeProvider timeProvider) : this(reportingActor,
            timeProvider, DefaultFlushInterval)
        {
        }

        public CounterAggregator(IActorRef reportingActor, ITimeProvider timeProvider, TimeSpan flushInterval)
        {
            _reportingActor = reportingActor;
            _flushInterval = flushInterval;
            _timeProvider = timeProvider;

            Receive<CounterIncrement>(c =>
            {
                // create if not exist...
                if (!_counters.ContainsKey(c.Name))
                    _counters[c.Name] = 0;

                _counters[c.Name] += c.IncrementValue;
            });

            Receive<Flush>(f =>
            {
                var timestamp = _timeProvider.NowUnixEpoch;
                foreach (var counter in _counters)
                    _reportingActor.Tell(new PcfMetricRecording(counter.Key, counter.Value, string.Empty, timestamp,
                        null));

                _counters.Clear();
            });
        }

        protected override void PreStart()
        {
            _flushTimer = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(_flushInterval, _flushInterval,
                Self, Flush.Instance, ActorRefs.NoSender);
        }

        protected override void PostStop()
        {
            _flushTimer?.Cancel();
        }

        /// <summary>
        ///     INTERNAL API.
        ///     Flush signal.
        /// </summary>
        public sealed class Flush
        {
            public static readonly Flush Instance = new Flush();

            private Flush()
            {
            }
        }

        /// <summary>
        ///     Used to increment the underlying <see cref="CounterAggregator" /> counter collection
        /// </summary>
        public struct CounterIncrement
        {
            public CounterIncrement(string name, int incrementValue)
            {
                Name = name;
                IncrementValue = incrementValue;
            }

            public string Name { get; }

            public int IncrementValue { get; }
        }
    }
}