using System;
using Xunit;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MetricsAgent.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using MetricsAgent.DAL;
using MetricsAgent.Entities;
using MetricsAgent.Responses;

namespace MetricsAgentTest
{
    public class DotNetMetricsControllerUnitTests
    {
        private Mock<ILogger<DotNetMetricsController>> mockLogger;
        private Mock<IDotNetMetricsRepository> mockRepository;
        private DotNetMetricsController controller;

        public DotNetMetricsControllerUnitTests()
        {
            mockRepository = new Mock<IDotNetMetricsRepository>();
            mockLogger = new Mock<ILogger<DotNetMetricsController>>();
            controller = new DotNetMetricsController(mockLogger.Object, mockRepository.Object);
        }

        [Fact]
        public void GetErrorsCount_ReturnsOk()
        {
            //Arrange
            var fromTime = DateTimeOffset.MinValue;
            var toTime = DateTimeOffset.Now;

            //Act
            var result = controller.GetErrorsCount(fromTime, toTime);

            // Assert
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void GetMetrics_ShouldCall_GetInTimePeriod_From_Repository()
        {
            var fromTime = DateTimeOffset.MinValue;
            var toTime = DateTimeOffset.Now;

            // Arrange
            mockRepository.
                Setup(repository => repository.GetInTimePeriod(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
                .Returns(GetTestDotNetMetric());

            // Act
            var result = controller.GetErrorsCount(fromTime, toTime);

            // Assert
            var response = ((result as OkObjectResult).Value as AllDotNetMetricsResponse).Metrics.Count;
            Assert.Equal(GetTestDotNetMetric().Count, response);
        }

        private List<DotNetMetric> GetTestDotNetMetric()
        {
            var dotNetMetric = new List<DotNetMetric>
            {
                new DotNetMetric { Id=1, Value=4, Time=4},
                new DotNetMetric { Id=2, Value=3, Time=3},
                new DotNetMetric { Id=3, Value=2, Time=2},
                new DotNetMetric { Id=4, Value=1, Time=1}
            };
            return dotNetMetric;
        }
    }
}
