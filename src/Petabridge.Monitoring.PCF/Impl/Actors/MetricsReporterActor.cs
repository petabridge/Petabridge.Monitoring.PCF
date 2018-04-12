// -----------------------------------------------------------------------
// <copyright file="MetricsReporterActor.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using Akka.Actor;
using Phobos.Actor.Common;

namespace Petabridge.Monitoring.PCF.Impl.Actors
{
    /// <summary>
    ///     INTERNAL API
    ///     Does the actual reporting and transmission to PCF Metrics Forwarder.
    /// </summary>
    internal sealed class MetricsReporterActor : ReceiveActor, INeverMonitor, INeverTrace
    {
    }
}