using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Phobos.Actor.Common;

namespace Petabridge.Monitoring.PCF.Impl.Actors
{
    /// <summary>
    /// INTERNAL API
    /// 
    /// Does the actual reporting and transmission to PCF Metrics Forwarder.
    /// </summary>
    internal sealed class MetricsReporterActor : ReceiveActor, INeverMonitor, INeverTrace
    {
    }
}
