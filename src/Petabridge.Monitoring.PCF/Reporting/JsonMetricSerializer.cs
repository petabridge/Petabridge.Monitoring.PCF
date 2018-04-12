// -----------------------------------------------------------------------
// <copyright file="JsonMetricSerializer.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Petabridge.Monitoring.PCF.Reporting
{
    /// <summary>
    ///     Serializes <see cref="PcfMetricRecording" /> objects into JSON format for delivery
    ///     over HTTP.
    /// </summary>
    /// <remarks>
    ///     Based on the JSON scheme described here: http://docs.pivotal.io/metrics-forwarder/api/#metrics
    /// </remarks>
    public sealed class JsonMetricSerializer : IMetricSerializer
    {
        internal const string Applications = "applications";
        internal const string ApplicationId = "id";
        internal const string Instances = "instances";
        internal const string InstanceId = "id";
        internal const string InstanceIndex = "index";
        internal const string Metrics = "metrics";
        internal const string MetricName = "name";
        internal const string MetricType = "type";
        internal const string MetricValue = "value";
        internal const string MetricUnit = "unit";
        internal const string Timestamp = "timestamp";
        internal const string Tags = "tags";

        public void Serialize(Stream stream, PcfMetricRecording metric, PcfMetricForwarderSettings settings)
        {
            using (var writer = new JsonTextWriter(new StreamWriter(stream, Encoding.Default, 2048, true)))
            {
                WriteHeaders(writer, settings);
                MetricToJson(writer, ref metric);
                CloseHeaders(writer);
            }
        }

        public void Serialize(Stream stream, IEnumerable<PcfMetricRecording> metrics,
            PcfMetricForwarderSettings settings)
        {
            using (var writer = new JsonTextWriter(new StreamWriter(stream, Encoding.Default, 2048, true)))
            {
                WriteHeaders(writer, settings);
                foreach (var m in metrics)
                {
                    var metric = m; // compiler ceremony
                    MetricToJson(writer, ref metric);
                }
                CloseHeaders(writer);
            }
        }

        private static void WriteHeaders(JsonTextWriter writer, PcfMetricForwarderSettings settings)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(Applications);
            writer.WriteStartArray();
            writer.WriteStartObject();
            writer.WritePropertyName(ApplicationId);
            writer.WriteValue(settings.Identity.AppId);
            writer.WritePropertyName(Instances);
            writer.WriteStartArray();
            writer.WriteStartObject();
            writer.WritePropertyName(InstanceId);
            writer.WriteValue(settings.Identity.InstanceId);
            writer.WritePropertyName(InstanceIndex);
            writer.WriteValue(settings.Identity.InstanceIndex);
            writer.WritePropertyName(Metrics);
            writer.WriteStartArray();
        }

        private static void CloseHeaders(JsonTextWriter writer)
        {
            writer.WriteEndArray();
            writer.WriteEndObject();
            writer.WriteEndArray();
            writer.WriteEndObject();
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        /// <summary>
        ///     Serializes the metric to JSON.
        /// </summary>
        /// <param name="writer">The JSON Writer.</param>
        /// <param name="recording">The metric recording.</param>
        /// <remarks>
        ///     Implementation note, for performance and correctness:
        ///     1. Don't emit null fields at all if the value isn't populated.
        ///     No need for that space in the content on the wire.
        ///     2. Design this method to be repeatable for multiple metrics in case
        ///     we want to batch.
        /// </remarks>
        private static void MetricToJson(JsonTextWriter writer, ref PcfMetricRecording recording)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(MetricName);
            writer.WriteValue(recording.Name);
            writer.WritePropertyName(MetricType);
            writer.WriteValue(recording.Type);
            writer.WritePropertyName(MetricValue);
            writer.WriteValue(recording.Value);
            writer.WritePropertyName(Timestamp);
            writer.WriteValue(recording.Timestamp);

            if (!string.IsNullOrEmpty(recording.Unit))
            {
                writer.WritePropertyName(MetricUnit);
                writer.WriteValue(recording.Unit);
            }

            if (recording.Tags != null)
            {
                writer.WritePropertyName(Tags);
                writer.WriteStartArray();
                foreach (var t in recording.Tags)
                {
                    writer.WritePropertyName(t.Key);
                    writer.WriteValue(t.Value);
                }
                writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }
    }
}