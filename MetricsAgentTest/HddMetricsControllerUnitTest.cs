using MetricsAgent.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MetricsAgentTest
{
    public class HddMetricsControllerUnitTest
    {
        private HddMetricsController controller;

        public HddMetricsControllerUnitTest()
        {
            controller = new HddMetricsController();
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
