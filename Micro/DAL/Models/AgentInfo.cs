using System;
using System.ComponentModel.DataAnnotations;

namespace MetricsManager.DAL.Models
{
    public class AgentInfo
    {
        [Key]
        public int? Id { get; set; }

        public int AgentId { get; set; }

        public string AgentUrl { get; set; }
    }
}
