using AutoMapper;
using MetricsAgent;
using MetricsAgent.Controllers;
using MetricsAgent.DAL;
using MetricsAgent.Models;
using MetricsAgent.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace MetricsAgentTest
{
    public class RamMetricsControllerUnitTests
    {
        private Mock<ILogger<RamMetricsController>> mockLogger;
        private Mapper mapper;
        private Mock<IRamMetricsRepository> mockRepository;
        private RamMetricsController controller;

        public RamMetricsControllerUnitTests()
        {
            mockRepository = new Mock<IRamMetricsRepository>();
            mockLogger = new Mock<ILogger<RamMetricsController>>();
            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MapperProfile())));
            controller = new RamMetricsController(mockLogger.Object, mockRepository.Object, mapper);
        }

        [Fact]
        public void GetAvailableRam_ReturnsOk()
        {
            //Act
            var result = controller.GetAvailableRam();

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
                .Returns(GetTestHddMetric());

            // Act
            var result = controller.GetMetrics(fromTime, toTime);

            // Assert
            var response = ((result as OkObjectResult).Value as AllRamMetricsResponse).Metrics;
            Assert.Equal(GetTestHddMetric().Count, response.Count);
        }

        private List<RamMetric> GetTestHddMetric()
        {
            var ramMetric = new List<RamMetric>
            {
                new RamMetric { Id=1, Value=4, Time=4},
                new RamMetric { Id=2, Value=3, Time=3},
                new RamMetric { Id=3, Value=2, Time=2},
                new RamMetric { Id=4, Value=1, Time=1}
            };
            return ramMetric;
        }
    }
}
