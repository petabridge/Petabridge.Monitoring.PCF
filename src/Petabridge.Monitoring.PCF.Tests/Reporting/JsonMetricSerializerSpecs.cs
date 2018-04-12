// -----------------------------------------------------------------------
// <copyright file="JsonMetricSerializerSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using System.Text;
using Petabridge.Monitoring.PCF.Reporting;
using Xunit;

namespace Petabridge.Monitoring.PCF.Tests.Reporting
{
    public class JsonMetricSerializerSpecs
    {
        public static readonly PcfMetricForwarderSettings Settings =
            new PcfMetricForwarderSettings(new PcfIdentity("a1", "a2", 0),
                new MetricsForwarderCredentials("foo", "http://localhost/api/v1"));

        [Fact]
        public void ShouldSerializeMetricIntoJson()
        {
            var serializer = new JsonMetricSerializer();
            var recording = new PcfMetricRecording("hi", 0, "foos", Settings.TimeProvider.NowUnixEpoch, null);
            var stream = new MemoryStream();
            serializer.Serialize(stream, recording, Settings);
            var actualOutput = stream.ToArray();
            var jsonStr = Encoding.UTF8.GetString(actualOutput);
            stream.Flush();
            stream.Dispose();
        }
    }
}