// -----------------------------------------------------------------------
// <copyright file="PcfMetricRecorder.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Bootstrap.PCF;
using Akka.Configuration;
using Akka.Util.Internal;
using Petabridge.Monitoring.PCF.Impl.Actors;
using Petabridge.Monitoring.PCF.Reporting.Http;
using Petabridge.Monitoring.PCF.Util;

namespace Petabridge.Monitoring.PCF
{
    /// <inheritdoc cref="IPcfMetricRecorder"/>
    /// <summary>
    ///     A <see cref="T:Petabridge.Monitoring.PCF.IPcfMetricRecorder" /> implementation that uses Akka.NET actors to
    ///     aggregate
    ///     counters and other metrics
    /// </summary>
    public sealed class PcfMetricRecorder : IPcfMetricRecorder, IDisposable
    {
        public const string CounterPostfix = "count";
        public const string TimingPostfix = "duration";

        private static readonly AtomicCounter NameCounter = new AtomicCounter(0);

        private static readonly Config NormalHocon = ConfigurationFactory.Empty;
        private static readonly Config DebugHocon = ConfigurationFactory.ParseString(@"akka.loglevel = DEBUG");
        private readonly IActorRef _counterAggregatorRef;
        private readonly ActorSystem _ownedActorSystem;
        private readonly IActorRef _reporterActorRef;

        /// <summary>
        ///     Creates a new metric recorder using the values provided in <see cref="PcfEnvironment" />
        /// </summary>
        internal PcfMetricRecorder(IActorRef reporterActorRef, IActorRef counterAggregatorRef,
            ActorSystem ownedActorSystem) : this(PcfMetricForwarderSettings.FromEnvironment(), reporterActorRef,
            counterAggregatorRef, ownedActorSystem)
        {
        }

        internal PcfMetricRecorder(PcfMetricForwarderSettings settings, IActorRef reporterActorRef,
            IActorRef counterAggregatorRef, ActorSystem ownedActorSystem = null)
        {
            Settings = settings;
            _reporterActorRef = reporterActorRef;
            _counterAggregatorRef = counterAggregatorRef;
            _ownedActorSystem = ownedActorSystem;
            TimeProvider = Settings.TimeProvider ?? new DateTimeOffsetTimeProvider();
        }

        public ITimeProvider TimeProvider { get; }

        public void Dispose()
        {
            // give it a chance to cleanup
            var stop1 = _counterAggregatorRef.GracefulStop(TimeSpan.FromSeconds(5));
            var stop2 = _reporterActorRef.GracefulStop(TimeSpan.FromSeconds(5));
            Task.WaitAll(stop1, stop2);

            // then optionally terminate ActorSystem
            _ownedActorSystem?.Dispose();
        }

        public PcfMetricForwarderSettings Settings { get; }

        public void IncrementCounter(string name, int value = 1)
        {
            _counterAggregatorRef.Tell(
                new CounterAggregator.CounterIncrement(Settings.ApplyMetricsSuffixes ? ApplyPostfix(name, CounterPostfix) : name, value));
        }

        public void DecrementCounter(string name, int value = -1)
        {
            _counterAggregatorRef.Tell(
                new CounterAggregator.CounterIncrement(Settings.ApplyMetricsSuffixes ? ApplyPostfix(name, CounterPostfix) : name, value));
        }

        public void RecordGauge(string name, double value)
        {
            _reporterActorRef.Tell(new PcfMetricRecording(name, value, null, TimeProvider.NowUnixEpoch, null));
        }

        public void RecordTiming(string name, long value)
        {
            _reporterActorRef.Tell(new PcfMetricRecording(Settings.ApplyMetricsSuffixes ? ApplyPostfix(name, TimingPostfix) : name, value, "milliseconds",
                TimeProvider.NowUnixEpoch, null));
        }

        /// <summary>
        ///     Performs all of the initialization to get the PCF metrics forwarding engine up and running.
        /// </summary>
        /// <param name="system">Optional. An <see cref="ActorSystem" /> used to spawn the underlying actors needed to do the job.</param>
        /// <returns>A new <see cref="PcfMetricRecorder" /> instance.</returns>
        public static PcfMetricRecorder Create(ActorSystem system = null)
        {
            return Create(PcfMetricForwarderSettings.FromEnvironment(), system);
        }

        /// <summary>
        ///     Performs all of the initialization to get the PCF metrics forwarding engine up and running.
        /// </summary>
        /// <param name="settings">The options and settings needed to communicate with PCF.</param>
        /// <param name="system">Optional. An <see cref="ActorSystem" /> used to spawn the underlying actors needed to do the job.</param>
        /// <returns>A new <see cref="PcfMetricRecorder" /> instance.</returns>
        public static PcfMetricRecorder Create(PcfMetricForwarderSettings settings, ActorSystem system = null)
        {
            var weOwnActorSystem = false;

            if (system == null) // create our own ActorSystem if it doesn't already exist.
            {
                weOwnActorSystem = true;
                system = ActorSystem.Create("pbpcfmetrics",
                    settings.DebugLogging ? DebugHocon : NormalHocon);
            }

            // spawn as a System actor, so in the event of being in a non-owned system our traces get shut down
            // only after all of the user-defined actors have terminated.
            var reporterActor = system.AsInstanceOf<ExtendedActorSystem>().SystemActorOf(
                Props.Create(() => new MetricsReporterActor(settings)),
                $"pcf-reporter-{NameCounter.GetAndIncrement()}");


            var counterActor = system.AsInstanceOf<ExtendedActorSystem>().SystemActorOf(
                Props.Create(() =>
                    new CounterAggregator(reporterActor, settings.TimeProvider ?? new DateTimeOffsetTimeProvider())),
                $"pcf-reporter-{NameCounter.GetAndIncrement()}");

            return new PcfMetricRecorder(reporterActor, counterActor, weOwnActorSystem ? system : null);
        }

        public static string ApplyPostfix(string metricName, string postFix)
        {
            return metricName.TrimEnd('.') + '.' + postFix;
        }

        internal void RecordMetric(PcfMetricRecording recording)
        {
            _reporterActorRef.Tell(recording);
        }
    }
}