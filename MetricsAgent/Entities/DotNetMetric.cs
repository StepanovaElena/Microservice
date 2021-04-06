using System;

namespace MetricsAgent.Entities
{
    public class DotNetMetric
    {
        public int Id { get; set; }

        public int Value { get; set; }

        public long Time { get; set; }
    }
}