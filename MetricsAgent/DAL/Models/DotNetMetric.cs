using System;
using System.ComponentModel.DataAnnotations;

namespace MetricsAgent.Models
{
    public class DotNetMetric
    {
        [Key]
        public int Id { get; set; }

        public int Value { get; set; }

        public long Time { get; set; }
    }
}