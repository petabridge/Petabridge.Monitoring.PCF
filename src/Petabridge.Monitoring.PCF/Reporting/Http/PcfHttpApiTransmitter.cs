// -----------------------------------------------------------------------
// <copyright file="PcfHttpApiTransmitter.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
        private readonly PcfMetricForwarderSettings _settings;

        public PcfHttpApiTransmitter(HttpClient client, PcfMetricForwarderSettings settings)
        {
            _settings = settings;
            _client = client;

            // NOTE: PCF requires a non-standard Authorization header - not allowed to prefix it with an authentication scheme
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", _settings.Credentials.AccessKey);
            _timeout = settings.PcfHttpTimeout;
        }

        public async Task<HttpResponseMessage> TransmitMetrics(IEnumerable<PcfMetricRecording> metrics)
        {
            using (var stream = StreamManager.GetStream("Petabridge.Monitoring.PCF.HttpTransmitter"))
            {
                _serializer.Serialize(stream, metrics, _settings);
                var cts = new CancellationTokenSource(_timeout);
                stream.Position = 0;
                var content = new StreamContent(stream);
                var request =
                    new HttpRequestMessage(HttpMethod.Post, _settings.Credentials.EndPoint) { Content = content };
                return await _client.SendAsync(request, cts.Token);
            }
        }
    }
}