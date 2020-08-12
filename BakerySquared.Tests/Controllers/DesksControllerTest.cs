using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
using BakerySquared.Models;
using BakerySquared.Controllers;
using System.Web.Mvc;
using BakerySquared.Models;
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
        /// <summary>
        /// Tests that the Index() method produces a paged list of Desk objects in its view.
        /// </summary>
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

        /// <summary>
        /// Tests that the Details() method produces a Desk object in it's view.
        /// </summary>
        [TestMethod]
        public void Details_ViewContains_DeskObject()
        {
            // Arrange
            Mock<IDesksRepository> mock = new Mock<IDesksRepository>();

            mock.Setup(d => d.Find("")).Returns(new Desk { Desk_Id = "D1000", Occupant = "Occupant1" });

            DesksController controller = new DesksController(mock.Object);

            // Act
            var actual = (Desk)controller.Details("").Model;

            // Assert
            Assert.IsInstanceOfType(actual, typeof(Desk));
        }

        /// <summary>
        /// Tests that the Create() method, when called to create a desk that does not already
        /// exist in the database, redirects to a different action ("Index" action).
        /// </summary>
        [TestMethod]
        public void Create_DeskDoesNotExist_RedirectsToAction()
        {
            // Arrange
            Mock<IDesksRepository> mock = new Mock<IDesksRepository>();

            mock.Setup(d => d.AlreadyExists(new Desk())).Returns(false);

            DesksController controller = new DesksController(mock.Object);

            // Act
            var actual = controller.Create(new Desk());

            // Assert
            Assert.IsInstanceOfType(actual, typeof(RedirectToRouteResult));
        }

        /// <summary>
        /// Tests that the Create() method, when called to create a desk that already exists
        /// in the database, returns an Action Result (View Result with attempted Desk_Id and Occupant).
        /// </summary>
        [TestMethod]
        public void Create_DeskAlreadyExist_ViewContainsDeskObject()
        {
            // Arrange
            Mock<IDesksRepository> mock = new Mock<IDesksRepository>();

            mock.Setup(d => d.AlreadyExists(new Desk())).Returns(true);

            DesksController controller = new DesksController(mock.Object);

            // Act
            var actual = controller.Create(new Desk());

            // Assert
            Assert.IsInstanceOfType(actual, typeof(ActionResult));
        }
    }

}
