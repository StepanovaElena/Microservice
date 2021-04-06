using AutoMapper;
using MetricsAgent;
using MetricsAgent.Controllers;
using MetricsAgent.DAL;
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
        private Mock<ILogger<HddMetricsController>> mockLogger;
        private Mapper mapper;
        private Mock<IHddMetricsRepository> mockRepository;
        private HddMetricsController controller;

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
    }
}
