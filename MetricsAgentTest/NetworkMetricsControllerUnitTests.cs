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
    public class NetworkMetricsControllerUnitTests
    {
        private Mock<ILogger<NetworkMetricsController>> mockLogger;
        private Mapper mapper;
        private Mock<INetworkMetricsRepository> mockRepository;
        private NetworkMetricsController controller;

        public NetworkMetricsControllerUnitTests()
        {
            mockRepository = new Mock<INetworkMetricsRepository>();
            mockLogger = new Mock<ILogger<NetworkMetricsController>>();
            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MapperProfile())));
            controller = new NetworkMetricsController(mockLogger.Object, mockRepository.Object, mapper);
        }

        [Fact]
        public void GetMetrics_ReturnsOk()
        {
            //Arrange
            var fromTime = DateTimeOffset.MinValue;
            var toTime = DateTimeOffset.Now;

            //Act
            var result = controller.GetMetrics(fromTime, toTime);

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
                .Returns(GetTestNetworkMetric());

            // Act
            var result = controller.GetMetrics(fromTime, toTime);

            // Assert
            var response = ((result as OkObjectResult).Value as AllNetworkMetricsResponse).Metrics;
            Assert.Equal(GetTestNetworkMetric().Count, response.Count);
        }

        private List<NetworkMetric> GetTestNetworkMetric()
        {
            var networkMetric = new List<NetworkMetric>
            {
                new NetworkMetric { Id=1, Value=4, Time=4},
                new NetworkMetric { Id=2, Value=3, Time=3},
                new NetworkMetric { Id=3, Value=2, Time=2},
                new NetworkMetric { Id=4, Value=1, Time=1}
            };      
            return networkMetric;
        }
    }
}
