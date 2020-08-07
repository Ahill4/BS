/*******************************************************************************
 * @file
 * @brief Controlls all the Desk views and interacts with the database
 *
 * *****************************************************************************
 *   Copyright (c) 2020 Koninklijke Philips N.V.
 *   All rights are reserved. Reproduction in whole or in part is
 *   prohibited without the prior written consent of the copyright holder.
 *******************************************************************************/

using System;
using BakerySquared.Models;
using BSDB.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Linq;
using PagedList;

namespace BakerySquared.Controllers
{
    /// <summary>
    /// Controller for the Desk table of the database
    /// </summary>
    public class DesksController : Controller
    {
        private BakerySquareDirectoryEntities db = new BakerySquareDirectoryEntities();
        private IDesksRepository _repository;

        public DesksController()
        {
            this._repository = new EFDesksRepository();
        }

        public DesksController(IDesksRepository repository)
        {
            _repository = repository;
        }


        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.OccuSortParm = sortOrder == "occu" ? "occu_desc" : "occu";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var desks = _repository.ToQuery();

            if (!String.IsNullOrEmpty(searchString))
            {
                desks = _repository.Contains(desks, searchString);
            }

            switch (sortOrder)
            {
                case "occupant_desc":
                    {
                        desks = _repository.OrderByDescendingId(desks);
                        break;
                    }
                case "occu":
                    {
                        desks = _repository.OrderByAscendingOccupant(desks);
                        break;
                    }
                case "occu_desc":
                    {
                        desks = _repository.OrderByDescendingOccupant(desks);
                        break;
                    }
                default: //desc Desk_Id
                    {
                        desks = _repository.OrderByAscendingId(desks);
                        break;
                    }
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(desks.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// shows details about an entry is the desks table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(string id)
        {
            ActionResult result;

            if (id == null)
            {
                result = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                Desk desk = _repository.Find(id);

                if (desk == null)
                {
                    result = HttpNotFound();
                }
                else
                {
                    result = View(desk);
                }
            }

            return result;
        }

        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// creates an entry in the desks table
        /// </summary>
        /// <param name="desk"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Desk_Id,Occupant")] Desk desk)
        {
            ActionResult result;

            if (ModelState.IsValid)
            {
                // This if/else checks if the desk exists in the db. If it does, it doesn't create and tells the user.
                if (!_repository.AlreadyExists(desk))
                {
                    _repository.Create(desk);
                    result = RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Desk already exists.");
                    result = View(desk);
                }
            }
            else
            {
                result = View(desk);
            }

            return result;
        }

        /// <summary>
        /// edits an entry in the desks table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(string id)
        {
            ActionResult result;

            if (id == null)
            {
                result = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                Desk desk = _repository.Find(id);

                if (desk == null)
                {
                    result = HttpNotFound();
                }
                else
                {
                    result = View(desk);
                }
            }
            return result;
        }

        /// <summary>
        /// edits an entry in the desks table
        /// </summary>
        /// <param name="desk"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Desk_Id,Occupant")] Desk desk)
        {
            ActionResult result;

            if (ModelState.IsValid)
            {
                _repository.Edit(desk);

                result = RedirectToAction("Index");
            }
            else
            {
                result = View(desk);
            }

            return result;
        }

        /// <summary>
        /// deletes an entry in the desks table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(string id)
        {
            ActionResult result;
            if (id == null)
            {
                result = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                Desk desk = _repository.Find(id);

                if (desk == null)
                {
                    result = HttpNotFound();
                }
                else
                {
                    result = View(desk);
                }
            }

            return result;
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            _repository.Delete(id);

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
