using MetricsAgent.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MetricsAgentTest
{
    public class NetworkMetricsControllerUnitTest
    {
        private NetworkMetricsController controller;
        public NetworkMetricsControllerUnitTest()
        {
            controller = new NetworkMetricsController();
        }

        [Fact]
        public void GetMetrics_ReturnsOk()
        {
            //Arrange
            var fromTime = TimeSpan.FromSeconds(0);
            var toTime = TimeSpan.FromSeconds(100);

            //Act
            var result = controller.GetMetrics(fromTime, toTime);

            // Assert
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}
