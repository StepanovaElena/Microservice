using MetricsManager.Controllers;
using MetricsManager.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MetricsManagerTest
{
    public class AgentsControllerUnitTests
    {
        private AgentsController controller;
        private Mock<ILogger<AgentsController>> mockLogger;

        public AgentsControllerUnitTests()
        {
            mockLogger = new Mock<ILogger<AgentsController>>();
            controller = new AgentsController(mockLogger.Object);
        }

        [Fact]
        public void ReadRegisteredAgents_ReturnsOk()
        {
            //Arrange

            //Act
            var result = controller.ReadRegisteredAgents();

            // Assert
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void RegistrAgent_ReturnsOk()
        {
            //Arrange
            var agentInfo = new AgentInfo()
            {
                AgentId = 1,
                AgentAddress = new Uri("http://www.example.com")
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
    }
}
