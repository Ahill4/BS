using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BakerySquared.Models;
using BakerySquared.Controllers;
using System.Web.Mvc;
using BSDB.Models;

namespace BakerySquared.Tests.Controllers
{
    /// <summary>
    /// Summary description for DeskControllerTest
    /// </summary>
    [TestClass]
    public class DeskControllerTest
    { 
        [TestMethod]
        public void Index_ReturnsViewResult_WithListOfDesks()
        {
            // Arrange
            var mock = new Mock<IDesksRepository>();
            mock.Setup(repo => repo.ToList());
            var controller = new DesksController(mock.Object);

            // Act
            var result = controller.Index();

            // Assert

        }
    }
}
