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
    public class HddMetricsControllerUnitTest
    {
        private Mock<ILogger<HddMetricsController>> mockLogger;
        private Mock<IHddMetricsRepository> mockRepository;
        private HddMetricsController controller;

        public HddMetricsControllerUnitTest()
        {
            mockRepository = new Mock<IHddMetricsRepository>();
            mockLogger = new Mock<ILogger<HddMetricsController>>();
            controller = new HddMetricsController(mockLogger.Object, mockRepository.Object);
        }

        [Fact]
        public void GetSpaceLeft_ReturnsOk()
        {            
            //Act
            var result = controller.GetSpaceLeft();

            // Assert
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}
