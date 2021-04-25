using AutoMapper;
using MetricsManager;
using MetricsManager.Controllers;
using MetricsManager.DAL.Interfaces;
using MetricsManager.DAL.Models;
using MetricsManager.DAL.Responses;
using MetricsManager.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace MetricsManagerTest
{
    public class CpuMetricsControllerUnitTests
    {
        private readonly CpuMetricsController controller;
        private readonly Mock<ILogger<CpuMetricsController>> mockLogger;
        private readonly Mock<ICpuMetricsRepository> mockRepository;
        private readonly Mock<IAgentsRepository> mockAgentsRepository;
        private readonly Mapper mapper;

        public CpuMetricsControllerUnitTests()
        {
            mockLogger = new Mock<ILogger<CpuMetricsController>>();
            mockRepository = new Mock<ICpuMetricsRepository>();
            mockAgentsRepository = new Mock<IAgentsRepository>();
            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MapperProfile())));
            controller = new CpuMetricsController(mockLogger.Object, mockRepository.Object, mockAgentsRepository.Object, mapper);
        }

        [Fact]
        public void GetMetricsFromAgent_ShouldCall_From_Repository()
        {
            //Arrange
            var agentId = 2;
            var fromTime = DateTimeOffset.MinValue;
            var toTime = DateTimeOffset.Now;

            mockAgentsRepository
                .Setup(repository => repository.GetAllAgentsInfo())
                .Returns(GetTestAgentsInfo());

            mockRepository
                .Setup(repository => repository.GetInTimePeriod(
                    It.IsAny<int>(), 
                    It.IsAny<DateTimeOffset>(), 
                    It.IsAny<DateTimeOffset>()))
                .Returns(GetTestCpuMetricFromAgent());

            //Act
            var result = controller.GetMetricsFromAgent(agentId, fromTime, toTime);

            // Assert
            var response = ((result as OkObjectResult).Value as AllCpuMetricsResponse).Metrics;
            Assert.Equal(GetTestCpuMetricFromAgent().Count, response.Count);
        }

        [Fact]
        public void GetMetricsByPercentileFromAgent__ShouldCall_From_Repository()
        {
            //Arrange
            var agentId = 2;
            var fromTime = DateTimeOffset.MinValue;
            var toTime = DateTimeOffset.Now;
            var percentile = Percentile.P90;

            mockRepository
                .Setup(repository => repository.GetInTimePeriodPercentile(
                    It.IsAny<int>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<Percentile>())).
                Returns(GetTestCpuMetricFromAgent()[1]);

            //Act
            var result = controller.GetMetricsByPercentileFromAgent(agentId, fromTime, toTime, percentile);

            // Assert
            var response = ((result as OkObjectResult).Value as CpuMetricDto);
            Assert.Equal(GetTestCpuMetricFromAgent()[1].Value, response.Value);
        }

        [Fact]
        public void GetMetricsFromAllCluster_ShouldCall_From_Repository()
        {
            //Arrange
            var fromTime = DateTimeOffset.MinValue;
            var toTime = DateTimeOffset.Now;
            
            mockAgentsRepository
                .Setup(repository => repository.GetAllAgentsInfo())
                .Returns(GetTestAgentsInfo().FindAll(m => (m.AgentId <= 2)));
            
            mockRepository.
                Setup(repository => repository.GetInTimePeriod(
                    It.Is<int>(agentId => agentId == 2),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>())).
                Returns(GetTestCpuMetric().FindAll(m => m.AgentId == 2));

            mockRepository.
                Setup(repository => repository.GetInTimePeriod(
                    It.Is<int>(agentId => agentId == 1),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>())).
                Returns(GetTestCpuMetric().FindAll(m => m.AgentId == 1));

            //Act
            var result = controller.GetMetricsFromAllCluster(fromTime, toTime);
            var expectedResult = GetTestCpuMetric().FindAll(m => (m.AgentId <= 2));
            // Assert
            var response = ((result as OkObjectResult).Value as AllCpuMetricsResponse).Metrics;
            Assert.Equal(expectedResult.Count, response.Count);
        }

        [Fact]
        public void GetMetricsByPercentileFromClusters__ShouldCall_From_Repository()
        {
            //Arrange
            var fromTime = DateTimeOffset.MinValue;
            var toTime = DateTimeOffset.Now;
            var percentile = Percentile.P90;

            mockAgentsRepository
                .Setup(repository => repository.GetAllAgentsInfo())
                .Returns(GetTestAgentsInfo().FindAll(m => (m.AgentId <= 2)));

            mockRepository.
                Setup(repository => repository.GetInTimePeriodPercentile(
                    It.Is<int>(agentId => agentId == 2),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<Percentile>())).
                Returns(GetTestCpuMetric().FindAll(m => m.AgentId == 2)[1]);

            mockRepository.
                Setup(repository => repository.GetInTimePeriodPercentile(
                    It.Is<int>(agentId => agentId == 1),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<Percentile>())).
                Returns(GetTestCpuMetric().FindAll(m => m.AgentId == 1)[0]);

            //Act
            var result = controller.GetMetricsByPercentileFromAllCluster(fromTime, toTime, percentile);

            // Assert
            var expected = mapper.Map<CpuMetricDto>(GetTestCpuMetric()[1]);
            var response = ((result as OkObjectResult).Value as AllCpuMetricsResponse);
            var checkResult = false;

            for (int i = 0; i < response.Metrics.Count; i++)
            {
                if ((response.Metrics[i].Value != expected.Value) ||
                    (response.Metrics[i].Time != expected.Time) ||
                    (response.Metrics[i].AgentId != expected.AgentId))
                {
                    checkResult = true;
                    break;
                }
            }

            Assert.True(checkResult);
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

        private List<CpuMetric> GetTestCpuMetric()
        {
            var cpuMetric = new List<CpuMetric>
            {
                new CpuMetric { Id=1, Value=40, Time= 129030884876, AgentId=2},
                new CpuMetric { Id=2, Value=300, Time=129030884876, AgentId=2},
                new CpuMetric { Id=3, Value=200, Time=129030884876, AgentId=3},
                new CpuMetric { Id=4, Value=100, Time=129030884876, AgentId=3},
                new CpuMetric { Id=5, Value=400, Time=129030884876, AgentId=1},
                new CpuMetric { Id=6, Value=30, Time=129030884876, AgentId=1},
                new CpuMetric { Id=7, Value=20, Time=129030884876, AgentId=4},
                new CpuMetric { Id=8, Value=10, Time=129030884876, AgentId=4}
            };

            return cpuMetric;
        }

        private List<CpuMetric> GetTestCpuMetricFromAgent()
        {
            var cpuMetric = new List<CpuMetric>
            {
                new CpuMetric { Id=1, Value=30, Time=1, AgentId=2},
                new CpuMetric { Id=2, Value=40, Time=2, AgentId=2}
            };

            return cpuMetric;
        }

        private List<AgentInfo> GetTestAgentsInfo()
        {
            var agentMetric = new List<AgentInfo>
            {
                new AgentInfo { Id=1, AgentId=1, AgentUrl="http://www.example.com"},
                new AgentInfo { Id=4, AgentId=2, AgentUrl="http://www.example.com"}
            };

            return agentMetric;
        }


    }

}
