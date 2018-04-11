namespace Petabridge.Monitoring.PCF.Impl
{
    /// <summary>
    /// The credentials used for emitting to the PCF metrics forwarder.
    /// </summary>
    public sealed class MetricsForwarderCredentials
    {
        public MetricsForwarderCredentials(string accessKey, string hostName)
        {
            AccessKey = accessKey;
            HostName = hostName;
        }

        /// <summary>
        /// The access key, needed for authenticating against the PCF service.
        /// </summary>
        public string AccessKey { get; }

        /// <summary>
        /// The HTTP endpoint of the PCF metrics forwarder.
        /// </summary>
        public string HostName { get; }
    }
}