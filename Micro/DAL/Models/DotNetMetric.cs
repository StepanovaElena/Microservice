using System;
using System.ComponentModel.DataAnnotations;

namespace MetricsManager.DAL.Models
{
    public class DotNetMetric
    {
        [Key]
        public int Id { get; set; }
        public int AgentId { get; set; }

        public int Value { get; set; }

        public long Time { get; set; }
    }
}