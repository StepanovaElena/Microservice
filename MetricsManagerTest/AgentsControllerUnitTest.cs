using AutoMapper;
using MetricsManager;
using MetricsManager.Controllers;
using MetricsManager.DAL.Interfaces;
using MetricsManager.DAL.Models;
using MetricsManager.DAL.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace MetricsManagerTest
{
    public class AgentsControllerUnitTests
    {
        private readonly AgentsController controller;
        private readonly Mock<ILogger<AgentsController>> mockLogger;        
        private readonly Mock<IAgentsRepository> mockRepository;
        private readonly Mapper mapper;

        public AgentsControllerUnitTests()
        {
            mockLogger = new Mock<ILogger<AgentsController>>();
            mockRepository = new Mock<IAgentsRepository>();
            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MapperProfile())));            
            controller = new AgentsController(mockLogger.Object, mockRepository.Object, mapper);
        }

        [Fact]
        public void ReadRegisteredAgents_ShouldCall_From_Repository()
        {
            // Arrange
            mockRepository.Setup(repository => repository.GetAllAgentsInfo()).Returns(GetTestAgentsInfo());

            //Act
            var result = controller.ReadRegisteredAgents();

            // Assert
            var response = ((result as OkObjectResult).Value as AllAgentsInfoResponse).Agents;
            Assert.Equal(GetTestAgentsInfo().Count, response.Count);
        }        

        [Fact]
        public void RegistrAgent_ReturnsOk()
        {
            //Arrange
            var agentInfo = new AgentInfo()
            {
                AgentId = 1,
                AgentUrl = "http://www.example.com"
            };

            //Act
            var result = controller.RegisterAgent(agentInfo);

            // Assert
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void EnableAgentById_ReturnsOk()
        {
            //Arrange
            var agentId = 1;

            //Act
            var result = controller.EnableAgentById(agentId);

            // Assert
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void DisableAgentById_ReturnsOk()
        {
            //Arrange
            var agentId = 1;

            //Act
            var result = controller.DisableAgentById(agentId);

            // Assert
            _ = Assert.IsAssignableFrom<IActionResult>(result);
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
