using MetricsAgent.Controllers;
using MetricsAgent.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MetricsAgentTest
{
    public class RamMetricsControllerUnitTest
    {
        private Mock<ILogger<RamMetricsController>> mockLogger;
        private Mock<IRamMetricsRepository> mockRepository;
        private RamMetricsController controller;

        public RamMetricsControllerUnitTest()
        {
            mockRepository = new Mock<IRamMetricsRepository>();
            mockLogger = new Mock<ILogger<RamMetricsController>>();
            controller = new RamMetricsController(mockLogger.Object, mockRepository.Object);
        }

        [Fact]
        public void GetAvailableRam_ReturnsOk()
        {
            //Act
            var result = controller.GetAvailableRam();

            // Assert
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}
