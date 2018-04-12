// -----------------------------------------------------------------------
// <copyright file="PcfIdentity.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

namespace Petabridge.Monitoring.PCF
{
    /// <summary>
    ///     Defines the identity data that need to be passed along to the PCF Metrics Forwarder.
    /// </summary>
    public sealed class PcfIdentity
    {
        public PcfIdentity(string appId, string instanceId, int instanceIndex)
        {
            AppId = appId;
            InstanceId = instanceId;
            InstanceIndex = instanceIndex;
        }

        /// <summary>
        ///     The PCF application ID.
        /// </summary>
        public string AppId { get; }

        /// <summary>
        ///     The ID for this instance of the PCF application.
        /// </summary>
        public string InstanceId { get; }

        /// <summary>
        ///     The [0-N] index number of this instance.
        /// </summary>
        public int InstanceIndex { get; }
    }
}