// -----------------------------------------------------------------------
// <copyright file="ITimeProvider.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Petabridge.Monitoring.PCF
{
    /// <summary>
    ///     Abstraction for providing time, so we can use virtual time and other fun stuff
    ///     when unit testing.
    /// </summary>
    public interface ITimeProvider
    {
        DateTimeOffset Now { get; }

        /// <summary>
        ///     The current UNIX epoch timestamp in milliseconds.
        /// </summary>
        long NowUnixEpoch { get; }
    }
}