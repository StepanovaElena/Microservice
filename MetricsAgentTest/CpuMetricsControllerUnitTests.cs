using AutoMapper;
using MetricsAgent;
using MetricsAgent.Controllers;
using MetricsAgent.DAL;
using MetricsAgent.Models;
using MetricsAgent.Responses;
using MetricsManager.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace MetricsAgentTest
{
    public class CpuMetricsControllerUnitTests
    {
        private readonly Mock<ILogger<CpuMetricsController>> mockLogger;
        private readonly Mock<ICpuMetricsRepository> mockRepository;
        private readonly CpuMetricsController controller;
        private readonly Mapper mapper;

        public CpuMetricsControllerUnitTests()
        {
            mockRepository = new Mock<ICpuMetricsRepository>();
            mockLogger = new Mock<ILogger<CpuMetricsController>>();
            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MapperProfile())));
            controller = new CpuMetricsController(mockLogger.Object, mockRepository.Object, mapper);
        }
        
        [Fact]
        public void GetMetrics_ShouldCall_GetInTimePeriod_From_Repository()
        {
            var fromTime = DateTimeOffset.MinValue;
            var toTime = DateTimeOffset.Now;

            // Arrange
            mockRepository
                .Setup(repository => repository.GetInTimePeriod(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
                .Returns(GetTestCpuMetric());

            // Act
            var result = controller.GetMetrics(fromTime, toTime);

            // Assert
            var response = ((result as OkObjectResult).Value as AllCpuMetricsResponse).Metrics;            
            Assert.Equal(GetTestCpuMetric().Count, response.Count);
        } 

        [Fact]
        public void GetAll_ShouldCall_GetAll_From_Repository()
        {
            // Arrange
            mockRepository.Setup(repository => repository.GetAll()).Returns(new List<CpuMetric>());

            // Act
            var result = controller.GetAll();

            // Assert
            Assert.NotNull(result);
        }

        private List<CpuMetric> GetTestCpuMetric()
        {
            var cpuMetric = new List<CpuMetric>
            {
                new CpuMetric { Id=1, Value=4, Time=4},
                new CpuMetric { Id=2, Value=3, Time=3},
                new CpuMetric { Id=3, Value=2, Time=2},
                new CpuMetric { Id=4, Value=1, Time=1}
            };

            return cpuMetric;
        }
    }

}
