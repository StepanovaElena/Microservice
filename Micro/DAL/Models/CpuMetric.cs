using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsManager.DAL.Models
{
    public class CpuMetric
    {
        [Key]
        public int Id { get; set; }

        public int AgentId { get; set; }

        public int Value { get; set; }

        public long Time { get; set; }
    }
}
