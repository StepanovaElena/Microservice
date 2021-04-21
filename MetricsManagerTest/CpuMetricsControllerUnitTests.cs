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

            mockRepository
                .Setup(repository => repository
                .GetInTimePeriod(It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
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
                .Returns(GetTestAgentsInfo());

            var firstAgentMetrics = GetTestCpuMetric().FindAll(m => m.AgentId == 1);
            var secondAgentMetrics = GetTestCpuMetric().FindAll(m => m.AgentId == 2);
            
            mockRepository.
                Setup(repository => repository.GetInTimePeriod(
                    It.Is<int>(agentId => agentId == 1),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>())).
                Returns(firstAgentMetrics);

            mockRepository.
                Setup(repository => repository.GetInTimePeriod(
                    It.Is<int>(agentId => agentId == 2),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>())).
                Returns(secondAgentMetrics);

            //Act
            var result = controller.GetMetricsFromAllCluster(fromTime, toTime);

            // Assert
            var response = ((result as OkObjectResult).Value as AllCpuMetricsResponse).Metrics;
            Assert.Equal(
                GetTestCpuMetric().FindAll(m => m.AgentId == 1 && m.AgentId == 2).Count, 
                response.Count);
        }

        [Fact]
        public void GetMetricsByPercentileFromAllCluster_ShouldCall_From_Repository()
        {
            //Arrange
            var fromTime = DateTimeOffset.MinValue;
            var toTime = DateTimeOffset.Now;
            var percentile = Percentile.P95;

            mockAgentsRepository
                .Setup(repository => repository.GetAllAgentsInfo())
                .Returns(GetTestAgentsInfo());

            var firstAgentMetrics = GetTestCpuMetric().FindAll(m => m.AgentId == 1)[0];
            var thirdAgentMetrics = GetTestCpuMetric().FindAll(m => m.AgentId == 3)[0];

            mockRepository.
                Setup(repository => repository.GetInTimePeriodPercentile(
                    It.Is<int>(agentId => agentId == 1),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<Percentile>())).
                Returns(firstAgentMetrics);

            mockRepository.
                Setup(repository => repository.GetInTimePeriodPercentile(
                    It.Is<int>(agentId => agentId == 3),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<Percentile>())).
                Returns(thirdAgentMetrics);

            //Act
            var result = controller.GetMetricsByPercentileFromAllCluster(fromTime, toTime, percentile);

            // Assert
            var response = ((result as OkObjectResult).Value as AllCpuMetricsResponse).Metrics;
            var expectedList = new List<CpuMetricDto>
            {
                mapper.Map<CpuMetricDto>(firstAgentMetrics),
                mapper.Map<CpuMetricDto>(thirdAgentMetrics)
            };

            Assert.Equal(expectedList.ToArray(), response.ToArray());
        }

        [Fact]
        public void GetMetricsFromAgent_ReturnsOk()
        {
            //Arrange
            var agentId = 1;
            var fromTime = DateTimeOffset.MinValue;
            var toTime = DateTimeOffset.Now;

            //Act
            var result = controller.GetMetricsFromAgent(agentId, fromTime, toTime);

            // Assert
            _ = Assert.IsAssignableFrom<IActionResult>(result);
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
                new CpuMetric { Id=1, Value=40, Time=129030884876, AgentId=2},
                new CpuMetric { Id=2, Value=300, Time=129030884876, AgentId=2}
            };
            return cpuMetric;
        }

        private List<AgentInfo> GetTestAgentsInfo()
        {
            var agentMetric = new List<AgentInfo>
            {
                new AgentInfo { Id=1, AgentId=3, AgentUrl="http://www.example.com"},
                new AgentInfo { Id=2, AgentId=5, AgentUrl="http://www.example.com"},
                new AgentInfo { Id=3, AgentId=4, AgentUrl="http://www.example.com"},
                new AgentInfo { Id=4, AgentId=2, AgentUrl="http://www.example.com"}
            };
            return agentMetric;
        }


    }

}
