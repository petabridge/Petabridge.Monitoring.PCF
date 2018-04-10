using System;
using FluentAssertions;
using Petabridge.Monitoring.PCF.Impl;
using Xunit;

namespace Petabridge.Monitoring.PCF.Tests
{
    public class MetricsCredentialParserSpecs
    {
        public static string VcapCredentialsJson = @"
            {
              ""metrics-forwarder"": [
               {
                ""credentials"": {
                 ""access_key"": ""abcd123"",
                 ""hostname"": ""https://api.pcf.io/metrics""
                },
                ""label"": ""metrics-forwarder"",
                ""name"": ""[service_instance_name]"",
                ""plan"": ""[plan]"",
                ""provider"": null,
                ""syslog_drain_url"": null,
                ""tags"": [],
                ""volume_mounts"": []
               }
              ]
             }";

        [Fact(DisplayName = "Should be able to parse Metrics Forwarder credentials from VCAP_SERVICES")]
        public void ShouldParseVcapCredentials()
        {
            var credentials = MetricsCredentialParser.ParseVcapServices(VcapCredentialsJson);
            credentials.AccessKey.Should().Be("abcd123");
            credentials.HostName.Should().Be("https://api.pcf.io/metrics");
        }
    }
}
