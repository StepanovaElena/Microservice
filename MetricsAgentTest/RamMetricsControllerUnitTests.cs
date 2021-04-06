using AutoMapper;
using MetricsAgent;
using MetricsAgent.Controllers;
using MetricsAgent.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MetricsAgentTest
{
    public class RamMetricsControllerUnitTests
    {
        private Mock<ILogger<RamMetricsController>> mockLogger;
        private Mapper mapper;
        private Mock<IRamMetricsRepository> mockRepository;
        private RamMetricsController controller;

        public RamMetricsControllerUnitTests()
        {
            mockRepository = new Mock<IRamMetricsRepository>();
            mockLogger = new Mock<ILogger<RamMetricsController>>();
            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MapperProfile())));
            controller = new RamMetricsController(mockLogger.Object, mockRepository.Object, mapper);
        }

        [Fact]
        public void GetAvailableRam_ReturnsOk()
        {
            //Act
            var result = controller.GetAvailableRam();

            // Assert
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}
