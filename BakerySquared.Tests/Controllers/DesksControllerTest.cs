using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BakerySquared.Models;
using BakerySquared.Controllers;
using System.Web.Mvc;
using BSDB.Models;
using System.Linq;
using PagedList;

namespace BakerySquared.Tests.Controllers
{
    /// <summary>
    /// Test class for DeskController.cs and it's methods.
    /// </summary>
    [TestClass]
    public class DesksControllerTest
    {
        [TestMethod]
        public void Index_ViewContains_PagedListOfDesks()
        {
            // Arrange
            Mock<IDesksRepository> mock = new Mock<IDesksRepository>();

            mock.Setup(d => d.ToQuery()).Returns(new Desk[]
            {
                new Desk {  Desk_Id = "D1000", Occupant = "Occupant1"   },
                new Desk {  Desk_Id = "D1001", Occupant = "Occupant2"   },
                new Desk {  Desk_Id = "D1002", Occupant = "Occupant3"   }
            }.AsQueryable());

            DesksController controller = new DesksController(mock.Object);

            // Act
            var actual = (PagedList<Desk>)controller.Index("", "", "", 1).Model;
            
            // Assert
            Assert.IsInstanceOfType(actual, typeof(PagedList<Desk>));
        }
    }
}
