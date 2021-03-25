using System;
using Xunit;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MetricsAgent.Controllers;

namespace MetricsAgentTest
{
    public class DotNetMetricsControllerUnitTest
    {
        private DotNetMetricsController controller;

        public DotNetMetricsControllerUnitTest()
        {
            controller = new DotNetMetricsController();
        }

        [Fact]
        public void GetErrorsCount_ReturnsOk()
        {
            //Arrange
            var fromTime = TimeSpan.FromSeconds(0);
            var toTime = TimeSpan.FromSeconds(100);

            //Act
            var result = controller.GetErrorsCount(fromTime, toTime);

            // Assert
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}
