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
        public void Create_DeskAlreadyExist_ReturnsActionResult()
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

        /// <summary>
        /// Tests that the Create() method, when called with an argument that is invalid such 
        /// as null object properties, returns an Action Result (View Result with desk object properties).
        /// </summary>
        [TestMethod]
        public void Create_InvalidModelState_ReturnsActionResult()
        {
            // Arrange
            Mock<IDesksRepository> mock = new Mock<IDesksRepository>();

            DesksController controller = new DesksController(mock.Object);

            // Act
            var actual = controller.Create(new Desk { Desk_Id = null, Occupant = null });

            // Assert
            Assert.IsInstanceOfType(actual, typeof(ActionResult));
        }

        /// <summary>
        /// Tests that the Edit() method (with string parameter), when called with a
        /// null argument, returns an HTTP Status Code Result (Bad Request).
        /// </summary>
        [TestMethod]
        public void Edit_IdArgumentIsNull_ReturnsHTTPStatusCodeResult()
        {
            // Arrange
            Mock<IDesksRepository> mock = new Mock<IDesksRepository>();

            DesksController controller = new DesksController(mock.Object);

            // Act
            string test = null;
            var actual = controller.Edit(test);

            // Assert
            Assert.IsInstanceOfType(actual, typeof(HttpStatusCodeResult));
        }

        /// <summary>
        /// Tests that the Edit() method (with string parameter), when desk id to edit is 
        /// found to be null, returns an Action Result (View("Error")).
        /// </summary>
        [TestMethod]
        public void Edit_DeskIsNull_ReturnsHTTPNotFound()
        {
            // Arrange
            Mock<IDesksRepository> mock = new Mock<IDesksRepository>();

            mock.Setup(d => d.Find("")).Returns(new Desk { Desk_Id = "", Occupant = "" });

            DesksController controller = new DesksController(mock.Object);

            // Act
            var actual = controller.Edit("");

            // Assert
            Assert.IsInstanceOfType(actual, typeof(ActionResult));
        }

        /// <summary>
        /// Tests that the Edit() method (with string parameter), when desk used to call
        /// is successfully found, returns an Action Result (View with Desk properties to edit).
        /// </summary>
        [TestMethod]
        public void Edit_DeskToEditIsFound_ReturnsViewResult()
        {
            // Arrange
            Mock<IDesksRepository> mock = new Mock<IDesksRepository>();

            mock.Setup(d => d.Find("")).Returns(new Desk { Desk_Id = "D1000", Occupant = "Occupant1" });

            DesksController controller = new DesksController(mock.Object);

            // Act
            var actual = controller.Edit("");

            // Assert
            Assert.IsInstanceOfType(actual, typeof(ActionResult));
        }

        /// <summary>
        /// Tests that the Edit() method (with Desk object parameter), when model state is 
        /// valid, the Desk in db is edited, and returns Redirect to Route Result (Redirect to "Index" action).
        /// </summary>
        [TestMethod]
        public void Edit_ModelStateIsValid_RedirectsToAction()
        {
            // Arrange
            Mock<IDesksRepository> mock = new Mock<IDesksRepository>();

            mock.Setup(d => d.Edit(new Desk())).Verifiable();

            DesksController controller = new DesksController(mock.Object);

            // Act
            var actual = controller.Edit(new Desk { Desk_Id = "D1000", Occupant = "Occupant1" });

            // Assert
            Assert.IsInstanceOfType(actual, typeof(RedirectToRouteResult));
        }

        /// <summary>
        /// Tests that the Edit() method (with Desk object parameter), when model state is 
        /// invalid, and returns an Action Result (View with Desk object).
        /// </summary>
        [TestMethod]
        public void Edit_ModelStateIsInvalid_RedirectsToAction()
        {
            // Arrange
            Mock<IDesksRepository> mock = new Mock<IDesksRepository>();

            DesksController controller = new DesksController(mock.Object);

            // Act
            var actual = controller.Edit(new Desk { Desk_Id = null, Occupant = null });

            // Assert
            Assert.IsInstanceOfType(actual, typeof(ActionResult));
        }
    }

}
