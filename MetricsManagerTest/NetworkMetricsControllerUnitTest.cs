using System;
using Xunit;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MetricsManager.Enums;
using MetricsManager.Controllers;
using Moq;
using Microsoft.Extensions.Logging;
using MetricsManager.DAL.Interfaces;
using AutoMapper;
using MetricsManager;
using MetricsManager.DAL.Responses;
using MetricsManager.DAL.Models;

namespace MetricsManagerTest
{
    public class NetworkMetricsControllerUnitTest
    {
        private readonly NetworkMetricsController controller;
        private readonly Mock<ILogger<NetworkMetricsController>> mockLogger;
        private readonly Mock<INetworkMetricsRepository> mockRepository;
        private readonly Mock<IAgentsRepository> mockAgentsRepository;
        private readonly Mapper mapper;

        public NetworkMetricsControllerUnitTest()
        {
            mockLogger = new Mock<ILogger<NetworkMetricsController>>();
            mockRepository = new Mock<INetworkMetricsRepository>();
            mockAgentsRepository = new Mock<IAgentsRepository>();
            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MapperProfile())));
            controller = new NetworkMetricsController(mockLogger.Object, mockRepository.Object, mockAgentsRepository.Object, mapper);
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
            var response = ((result as OkObjectResult).Value as AllNetworkMetricsResponse).Metrics;
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
            var response = ((result as OkObjectResult).Value as NetworkMetricDto);
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
            var expected = mapper.Map<NetworkMetricDto>(GetTestMetric()[1]);
            var response = ((result as OkObjectResult).Value as AllNetworkMetricsResponse);
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
            var response = ((result as OkObjectResult).Value as AllNetworkMetricsResponse).Metrics;
            Assert.Equal(expectedResult.Count, response.Count);
        }

        private List<NetworkMetric> GetTestMetricFromAgent()
        {
            var metric = new List<NetworkMetric>
            {
                new NetworkMetric { Id=1, Value=30, Time=1, AgentId=2},
                new NetworkMetric { Id=2, Value=40, Time=2, AgentId=2}
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

        private List<NetworkMetric> GetTestMetric()
        {
            var metric = new List<NetworkMetric>
            {
                new NetworkMetric { Id=1, Value=40, Time= 129030884876, AgentId=2},
                new NetworkMetric { Id=2, Value=300, Time=129030884876, AgentId=2},
                new NetworkMetric { Id=3, Value=200, Time=129030884876, AgentId=3},
                new NetworkMetric { Id=4, Value=100, Time=129030884876, AgentId=3},
                new NetworkMetric { Id=5, Value=400, Time=129030884876, AgentId=1},
                new NetworkMetric { Id=6, Value=30, Time=129030884876, AgentId=1},
                new NetworkMetric { Id=7, Value=20, Time=129030884876, AgentId=4},
                new NetworkMetric { Id=8, Value=10, Time=129030884876, AgentId=4}
            };

            return metric;
        }
    }
}
