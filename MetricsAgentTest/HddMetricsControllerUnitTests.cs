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
using System.Text;
using Xunit;

namespace MetricsAgentTest
{
    public class HddMetricsControllerUnitTests
    {
        private readonly Mock<ILogger<HddMetricsController>> mockLogger;
        private readonly Mapper mapper;
        private readonly Mock<IHddMetricsRepository> mockRepository;
        private readonly HddMetricsController controller;

        public HddMetricsControllerUnitTests()
        {
            mockRepository = new Mock<IHddMetricsRepository>();
            mockLogger = new Mock<ILogger<HddMetricsController>>();
            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MapperProfile())));
            controller = new HddMetricsController(mockLogger.Object, mockRepository.Object, mapper);
        }

        [Fact]
        public void GetSpaceLeft_ReturnsOk()
        {            
            //Act
            var result = controller.GetSpaceLeft();

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
            var response = ((result as OkObjectResult).Value as AllHddMetricsResponse).Metrics;
            Assert.Equal(GetTestHddMetric().Count, response.Count);
        }

        private List<HddMetric> GetTestHddMetric()
        {
            var hddMetric = new List<HddMetric>
            {
                new HddMetric { Id=1, Value=4, Time=4},
                new HddMetric { Id=2, Value=3, Time=3},
                new HddMetric { Id=3, Value=2, Time=2},
                new HddMetric { Id=4, Value=1, Time=1}
            };
            return hddMetric;
        }
    }
}
