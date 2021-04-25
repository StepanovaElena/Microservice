using System;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using MetricsManager.Controllers;
using MetricsManager.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using MetricsManager.DAL.Interfaces;
using AutoMapper;
using MetricsManager;
using MetricsManager.DAL.Models;
using System.Collections.Generic;
using MetricsManager.DAL.Responses;

namespace MetricsManagerTest
{
    public class RamMetricsControllerUnitTest
    {
        private readonly RamMetricsController controller;
        private readonly Mock<ILogger<RamMetricsController>> mockLogger;
        private readonly Mock<IRamMetricsRepository> mockRepository;
        private readonly Mock<IAgentsRepository> mockAgentsRepository;
        private readonly Mapper mapper;

        public RamMetricsControllerUnitTest()
        {
            mockLogger = new Mock<ILogger<RamMetricsController>>();
            mockRepository = new Mock<IRamMetricsRepository>();
            mockAgentsRepository = new Mock<IAgentsRepository>();
            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MapperProfile())));
            controller = new RamMetricsController(mockLogger.Object, mockRepository.Object, mockAgentsRepository.Object, mapper);
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
                .Returns(GetTestMetricFromAgent());

            //Act
            var result = controller.GetMetricsFromAgent(agentId, fromTime, toTime);

            // Assert
            var response = ((result as OkObjectResult).Value as AllRamMetricsResponse).Metrics;
            Assert.Equal(GetTestMetricFromAgent().Count, response.Count);
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
                Returns(GetTestMetricFromAgent()[1]);

            //Act
            var result = controller.GetMetricsByPercentileFromAgent(agentId, fromTime, toTime, percentile);

            // Assert
            var response = ((result as OkObjectResult).Value as RamMetricDto);
            Assert.Equal(GetTestMetricFromAgent()[1].Value, response.Value);
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
                Returns(GetTestMetric().FindAll(m => m.AgentId == 2)[1]);

            mockRepository.
                Setup(repository => repository.GetInTimePeriodPercentile(
                    It.Is<int>(agentId => agentId == 1),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<Percentile>())).
                Returns(GetTestMetric().FindAll(m => m.AgentId == 1)[0]);

            //Act
            var result = controller.GetMetricsByPercentileFromAllCluster(fromTime, toTime, percentile);

            // Assert
            var expected = mapper.Map<RamMetricDto>(GetTestMetric()[1]);
            var response = ((result as OkObjectResult).Value as AllRamMetricsResponse);
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
                Returns(GetTestMetric().FindAll(m => m.AgentId == 2));

            mockRepository.
                Setup(repository => repository.GetInTimePeriod(
                    It.Is<int>(agentId => agentId == 1),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>())).
                Returns(GetTestMetric().FindAll(m => m.AgentId == 1));

            //Act
            var result = controller.GetMetricsFromAllCluster(fromTime, toTime);
            var expectedResult = GetTestMetric().FindAll(m => (m.AgentId <= 2));
            // Assert
            var response = ((result as OkObjectResult).Value as AllRamMetricsResponse).Metrics;
            Assert.Equal(expectedResult.Count, response.Count);
        }

        private List<RamMetric> GetTestMetricFromAgent()
        {
            var metric = new List<RamMetric>
            {
                new RamMetric { Id=1, Value=30, Time=1, AgentId=2},
                new RamMetric { Id=2, Value=40, Time=2, AgentId=2}
            };

            return metric;
        }

        private List<AgentInfo> GetTestAgentsInfo()
        {
            var agentMetric = new List<AgentInfo>
            {
                new AgentInfo { Id=1, AgentId=1, AgentUrl="http://www.example.com"},
                new AgentInfo { Id=2, AgentId=5, AgentUrl="http://www.example.com"},
                new AgentInfo { Id=3, AgentId=4, AgentUrl="http://www.example.com"},
                new AgentInfo { Id=4, AgentId=2, AgentUrl="http://www.example.com"}
            };

            return agentMetric;
        }

        private List<RamMetric> GetTestMetric()
        {
            var metric = new List<RamMetric>
            {
                new RamMetric { Id=1, Value=40, Time= 129030884876, AgentId=2},
                new RamMetric { Id=2, Value=300, Time=129030884876, AgentId=2},
                new RamMetric { Id=3, Value=200, Time=129030884876, AgentId=3},
                new RamMetric { Id=4, Value=100, Time=129030884876, AgentId=3},
                new RamMetric { Id=5, Value=400, Time=129030884876, AgentId=1},
                new RamMetric { Id=6, Value=30, Time=129030884876, AgentId=1},
                new RamMetric { Id=7, Value=20, Time=129030884876, AgentId=4},
                new RamMetric { Id=8, Value=10, Time=129030884876, AgentId=4}
            };

            return metric;
        }
    }
}
