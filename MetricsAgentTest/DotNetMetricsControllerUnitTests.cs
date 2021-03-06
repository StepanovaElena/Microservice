using System;
using Xunit;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MetricsAgent.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using MetricsAgent.DAL;
using AutoMapper;
using MetricsAgent;
using MetricsAgent.Models;
using MetricsAgent.Responses;

namespace MetricsAgentTest
{
    public class DotNetMetricsControllerUnitTests
    {
        private readonly Mock<ILogger<DotNetMetricsController>> mockLogger;
        private readonly Mapper mapper;
        private readonly Mock<IDotNetMetricsRepository> mockRepository;
        private readonly DotNetMetricsController controller;

        public DotNetMetricsControllerUnitTests()
        {
            mockRepository = new Mock<IDotNetMetricsRepository>();
            mockLogger = new Mock<ILogger<DotNetMetricsController>>();
            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MapperProfile())));
            controller = new DotNetMetricsController(mockLogger.Object, mockRepository.Object, mapper);
        }       

        [Fact]
        public void GetErrorsCount_ShouldCall_GetInTimePeriod_From_Repository()
        {
            var fromTime = DateTimeOffset.MinValue;
            var toTime = DateTimeOffset.Now;

            // Arrange
            mockRepository
                .Setup(repository => repository.GetInTimePeriod(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
                .Returns(GetTestDotNetMetric());

            // Act
            var result = controller.GetErrorsCount(fromTime, toTime);

            // Assert
            var response = (result as OkObjectResult).Value;
            Assert.Equal(GetTestDotNetMetric().Count, response);
        }

        [Fact]
        public void GetMetrics_ShouldCall_GetInTimePeriod_From_Repository()
        {
            var fromTime = DateTimeOffset.MinValue;
            var toTime = DateTimeOffset.Now;

            // Arrange
            mockRepository
                .Setup(repository => repository.GetInTimePeriod(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
                .Returns(GetTestDotNetMetric());

            // Act
            var result = controller.GetMetrics(fromTime, toTime);

            // Assert
            var response = ((result as OkObjectResult).Value as AllDotNetMetricsResponse).Metrics;
            Assert.Equal(GetTestDotNetMetric().Count, response.Count);
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
