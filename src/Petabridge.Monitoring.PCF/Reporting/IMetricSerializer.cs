using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Petabridge.Monitoring.PCF.Reporting
{
    public interface IMetricSerializer
    {
        /// <summary>
        ///     Serializes the provided metric into the <see cref="Stream" />.
        /// </summary>
        /// <param name="stream">A writable stream that we'll be adding the contents of the metric to.</param>
        /// <param name="metric">The metric to be serialized.</param>
        /// <param name="settings">The settings for this application instance.</param>
        void Serialize(Stream stream, PcfMetricRecording metric, PcfMetricForwarderSettings settings);

        /// <summary>
        ///     Serializes a collection of metrics into the <see cref="Stream" />.
        /// </summary>
        /// <param name="stream">A writable stream that we'll be adding the contents of the metric to.</param>
        /// <param name="metrics">The metrics to be serialized.</param>
        /// <param name="settings">The settings for this application instance.</param>
        /// <remarks>
        ///     Designed to offer support for batching et al.
        /// </remarks>
        void Serialize(Stream stream, IEnumerable<PcfMetricRecording> metrics, PcfMetricForwarderSettings settings);
    }
}
