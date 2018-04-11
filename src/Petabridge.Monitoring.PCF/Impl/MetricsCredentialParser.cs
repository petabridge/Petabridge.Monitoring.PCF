// -----------------------------------------------------------------------
// <copyright file="MetricsCredentialParser.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Petabridge.Monitoring.PCF.Impl
{
    /// <summary>
    ///     INTERNAL API.
    ///     Used to parse the Metrics Forwarder credentials from the VCAP_SERVICES environment variable.
    /// </summary>
    /// <remarks>
    ///     See http://docs.pivotal.io/metrics-forwarder/api/#authentication for an explanation of the API
    /// </remarks>
    internal static class MetricsCredentialParser
    {
        /// <summary>
        ///     Parses the VCAP_SERVICES environmental variable and extracts
        ///     the <see cref="MetricsForwarderCredentials" /> accordingly.
        /// </summary>
        /// <returns>
        ///     A parsed set of credentials.
        /// </returns>
        public static MetricsForwarderCredentials ParseVcapServices()
        {
            return ParseVcapServices(Environment.GetEnvironmentVariable("VCAP_SERVICES"));
        }

        /// <summary>
        ///     Parses the VCAP_SERVICES environmental variable and extracts
        ///     the <see cref="MetricsForwarderCredentials" /> accordingly.
        /// </summary>
        /// <param name="vcapServices">Should be the JSON object contained inside the VCAP_SERVICES environment variable.</param>
        /// <returns>
        ///     A parsed set of credentials.
        /// </returns>
        public static MetricsForwarderCredentials ParseVcapServices(string vcapServices)
        {
            var jsonObj = JToken.Parse(vcapServices)["metrics-forwarder"].First()["credentials"];
            var accessKey = jsonObj["access_key"].Value<string>();
            var hostName = jsonObj["hostname"].Value<string>();

            return new MetricsForwarderCredentials(accessKey, hostName);
        }
    }
}