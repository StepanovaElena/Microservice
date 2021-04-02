using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsAgent.Requests
{
	public class NetworkMetricCreateRequest
	{
		public int Time { get; set; }
		public int Value { get; set; }
	}
}
