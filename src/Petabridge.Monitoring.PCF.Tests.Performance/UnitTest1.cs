// -----------------------------------------------------------------------
// <copyright file="UnitTest1.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using NBench;

namespace Petabridge.Monitoring.PCF.Tests.Performance
{
    public class UnitTest1
    {
        public const string CounterName = "Operations";
        private Counter _opsCounter;

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            _opsCounter = context.GetCounter(CounterName);
        }

        [PerfBenchmark(NumberOfIterations = 5, RunMode = RunMode.Throughput, RunTimeMilliseconds = 1000)]
        [CounterMeasurement(CounterName)]
        [GcMeasurement(GcMetric.TotalCollections, GcGeneration.AllGc)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        public void TestMethod1()
        {
            _opsCounter.Increment();
        }
    }
}