// -----------------------------------------------------------------------
// <copyright file="PcfHttpApiTransmitter.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IO;

namespace Petabridge.Monitoring.PCF.Reporting.Http
{
    /// <summary>
    ///     INTERNAL API.
    ///     Used for transmitting data to the PCF Metrics Forwarder HTTP API.
    /// </summary>
    public sealed class PcfHttpApiTransmitter
    {
        /// <summary>
        ///     Using JSON.
        /// </summary>
        public const string MediaType = "application/json";

        /// <summary>
        ///     Only need one of these globally, and it's thread-safe. The streams it accesses internally are inherently not safe.
        /// </summary>
        private static readonly RecyclableMemoryStreamManager StreamManager = new RecyclableMemoryStreamManager();

        private readonly HttpClient _client;
        private readonly IMetricSerializer _serializer = new JsonMetricSerializer();
        private readonly TimeSpan _timeout;

        public PcfHttpApiTransmitter(HttpClient client, TimeSpan timeout)
        {
            _client = client;
            _timeout = timeout;
        }

        public async Task<HttpResponseMessage> TransmitMetrics(IEnumerable<PcfMetricRecording> metrics,
            PcfMetricForwarderSettings settings)
        {
            using (var stream = StreamManager.GetStream("Petabridge.Monitoring.PCF.HttpTransmitter"))
            {
                _serializer.Serialize(stream, metrics, settings);
                var cts = new CancellationTokenSource(_timeout);
                stream.Position = 0;
                var content = new StreamContent(stream);
                content.Headers.TryAddWithoutValidation("Content-Type", MediaType);
                content.Headers.TryAddWithoutValidation("Content-Length", stream.Length.ToString());
                content.Headers.TryAddWithoutValidation("Authorization", settings.Credentials.AccessKey);
                return await _client.PostAsync(settings.Credentials.EndPoint, content, cts.Token);
            }
        }
    }
}