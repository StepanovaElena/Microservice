using System;
using Xunit;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MetricsManager.Controllers;
using MetricsManager.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace MetricsAgentTest
{
    public class HddMetricsControllerUnitTest
    {
        private HddMetricsController controller;
        private Mock<ILogger<HddMetricsController>> mockLogger;

        public HddMetricsControllerUnitTest()
        {
            mockLogger = new Mock<ILogger<HddMetricsController>>();
            controller = new HddMetricsController(mockLogger.Object);
        }

        [Fact]
        public void GetMetricsFromAgent_ReturnsOk()
        {
            //Arrange
            var agentId = 1;
            var fromTime = DateTimeOffset.MinValue;
            var toTime = DateTimeOffset.Now;

            //Act
            var result = controller.GetMetricsFromAgent(agentId, fromTime, toTime);

            // Assert
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void GetMetricsByPercentileFromAgent_ReturnsOk()
        {
            //Arrange
            var agentId = 1;
            var fromTime = DateTimeOffset.MinValue;
            var toTime = DateTimeOffset.Now;
            var percentile = Percentile.P90;

            //Act
            var result = controller.GetMetricsByPercentileFromAgent(agentId, fromTime, toTime, percentile);

            // Assert
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void GetMetricsFromAllCluster_ReturnsOk()
        {
            //Arrange
            var fromTime = DateTimeOffset.MinValue;
            var toTime = DateTimeOffset.Now;

            //Act
            var result = controller.GetMetricsFromAllCluster(fromTime, toTime);

            // Assert
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void GetMetricsByPercentileFromAllCluster_ReturnsOk()
        {
            //Arrange
            var fromTime = DateTimeOffset.MinValue;
            var toTime = DateTimeOffset.Now;
            var percentile = Percentile.P90;

            //Act
            var result = controller.GetMetricsByPercentileFromAllCluster(fromTime, toTime, percentile);

            // Assert
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}
