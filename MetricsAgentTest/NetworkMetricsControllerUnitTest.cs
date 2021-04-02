using MetricsAgent.Controllers;
using MetricsAgent.DAL;
using MetricsAgent.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MetricsAgentTest
{
    public class NetworkMetricsControllerUnitTest
    {
        private Mock<ILogger<NetworkMetricsController>> mockLogger;
        private Mock<INetworkMetricsRepository> mockRepository;
        private NetworkMetricsController controller;

        public NetworkMetricsControllerUnitTest()
        {
            mockRepository = new Mock<INetworkMetricsRepository>();
            mockLogger = new Mock<ILogger<NetworkMetricsController>>();
            controller = new NetworkMetricsController(mockLogger.Object, mockRepository.Object);
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

        [Fact]
        public void GetMetrics_ShouldCall_GetInTimePeriod_From_Repository()
        {
            var fromTime = TimeSpan.FromSeconds(0);
            var toTime = TimeSpan.FromSeconds(100);

            // Arrange
            mockRepository.Setup(repository => repository.GetInTimePeriod(fromTime, toTime)).Returns(new List<NetworkMetric>());

            // Act
            var result = controller.GetMetrics(fromTime, toTime);

            // Assert
            Assert.NotNull(result);
        }
    }
}
