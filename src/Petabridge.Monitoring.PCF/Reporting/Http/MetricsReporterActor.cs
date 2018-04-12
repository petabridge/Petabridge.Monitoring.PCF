// -----------------------------------------------------------------------
// <copyright file="MetricsReporterActor.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Akka.Actor;
using Akka.Event;
using Phobos.Actor.Common;

namespace Petabridge.Monitoring.PCF.Reporting.Http
{
    /// <summary>
    ///     INTERNAL API
    ///     Does the actual reporting and transmission to PCF Metrics Forwarder.
    /// </summary>
    internal sealed class MetricsReporterActor : ReceiveActor, INeverMonitor, INeverTrace
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();

        private readonly PcfMetricForwarderSettings _settings;

        private readonly PcfHttpApiTransmitter _transmitter;

        private ICancelable _batchTransimissionTimer;

        public MetricsReporterActor(PcfMetricForwarderSettings settings)
        {
            _settings = settings;
            PendingMessages = new List<PcfMetricRecording>(_settings.MaximumBatchSize);
            _transmitter = new PcfHttpApiTransmitter(new HttpClient(), _settings.PcfHttpTimeout);
            Batching();
        }

        public List<PcfMetricRecording> PendingMessages { get; }

        public bool BatchSizeReached => PendingMessages.Count >= _settings.MaximumBatchSize;

        private void Batching()
        {
            Receive<PcfMetricRecording>(s =>
            {
                PendingMessages.Add(s);
                if (BatchSizeReached)
                    ExecuteDelivery();
            });

            Receive<DeliverBatch>(d =>
            {
                if (PendingMessages.Any())
                    ExecuteDelivery();
                else
                    RescheduleBatchTransmission();
            });

            Receive<HttpResponseMessage>(rsp =>
            {
                if (_log.IsDebugEnabled)
                    _log.Debug(
                        "Received notification that Span batch was received by PCF Metrics Forwarder at [{0}] with success code [{1}]",
                        _settings.Credentials.HostName, rsp.StatusCode);
            });

            // Indicates that one of our HTTP requests timed out
            Receive<Status.Failure>(f =>
            {
                if (_log.IsErrorEnabled)
                    _log.Error(f.Cause, "Error occurred while uploading metrics to [{0}]",
                        _settings.Credentials.HostName);
            });
        }

        private void ExecuteDelivery()
        {
            _transmitter.TransmitMetrics(PendingMessages, _settings).PipeTo(Self);

            /*
                     * TransmitSpans will synchronously write out the JSON in a stream before this method
                     * returns, therefore it is safe for us to modify the PendingMessages collection directly.
                     */
            PendingMessages.Clear();
            RescheduleBatchTransmission();
        }

        private void RescheduleBatchTransmission()
        {
            _batchTransimissionTimer?.Cancel(false);
            _batchTransimissionTimer =
                Context.System.Scheduler.ScheduleTellOnceCancelable(_settings.MaxBatchInterval, Self,
                    DeliverBatch.Instance, Self);
        }

        protected override void PreStart()
        {
            RescheduleBatchTransmission();
        }

        protected override void PostStop()
        {
            _batchTransimissionTimer?.Cancel();
        }

        /// <summary>
        ///     INTERNAL API.
        ///     Signal
        /// </summary>
        private sealed class DeliverBatch
        {
            public static readonly DeliverBatch Instance = new DeliverBatch();

            private DeliverBatch()
            {
            }
        }
    }
}