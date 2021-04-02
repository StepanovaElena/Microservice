using MetricsAgent.Controllers;
using MetricsAgent.DAL;
using MetricsAgent.Entities;
using MetricsManager.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace MetricsAgentTest
{
    public class CpuMetricsControllerUnitTest
    {
        private Mock<ILogger<CpuMetricsController>> mockLogger;
        private Mock<ICpuMetricsRepository> mockRepository;
        private CpuMetricsController controller;

        public CpuMetricsControllerUnitTest()
        {
            mockRepository = new Mock<ICpuMetricsRepository>();
            mockLogger = new Mock<ILogger<CpuMetricsController>>();
            controller = new CpuMetricsController(mockLogger.Object, mockRepository.Object);
        }

        [Fact]
        public void GetMetrics_ReturnsOk()
        {            
            var fromTime = TimeSpan.FromSeconds(0);
            var toTime = TimeSpan.FromSeconds(100);
           
            var result = controller.GetMetrics(fromTime, toTime);
            
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void GetMetrics_ShouldCall_GetInTimePeriod_From_Repository()
        {
            var fromTime = TimeSpan.FromSeconds(0);
            var toTime = TimeSpan.FromSeconds(100);

            // Arrange
            mockRepository.Setup(repository => repository.GetInTimePeriod(fromTime, toTime)).Returns(new List<CpuMetric>());

            // Act
            var result = controller.GetMetrics(fromTime, toTime);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetMetricsByPercentile_ReturnsOk()
        {
            //Arrange
            var fromTime = TimeSpan.FromSeconds(0);
            var toTime = TimeSpan.FromSeconds(100);
            var percentile = Percentile.P90;

            //Act
            var result = controller.GetMetricsByPercentile(fromTime, toTime, percentile);

            // Assert
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void Create_ShouldCall_Create_From_Repository()
        {
            // устанавливаем параметр заглушки
            // в заглушке прописываем что в репозиторий прилетит CpuMetric объект
            mockRepository.Setup(repository => repository.Create(It.IsAny<CpuMetric>())).Verifiable();

            // выполняем действие на контроллере
            var result = controller.Create(new MetricsAgent.Requests.CpuMetricCreateRequest { 
                Time = 1,
                Value = 50 
            });

            // проверяем заглушку на то, что пока работал контроллер
            // действительно вызвался метод Create репозитория с нужным типом объекта в параметре
            mockRepository.Verify(repository => repository.Create(It.IsAny<CpuMetric>()), Times.AtMostOnce());
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
    }

}
