using MetricsAgent.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MetricsAgentTest
{
    public class RamMetricsControllerUnitTest
    {
        private RamMetricsController controller;
        public RamMetricsControllerUnitTest()
        {
            controller = new RamMetricsController();
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
