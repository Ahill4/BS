/*******************************************************************************
 * @file
 * @brief Controlls all the Desk views and interacts with the database
 *
 * *****************************************************************************
 *   Copyright (c) 2020 Koninklijke Philips N.V.
 *   All rights are reserved. Reproduction in whole or in part is
 *   prohibited without the prior written consent of the copyright holder.
 *******************************************************************************/

using BakerySquared.Models;
using BSDB.Models;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
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


        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            //IEnumerable<Desk> deskList = db.Desks.ToList();
            //IEnumerable<Desk> deskList = _repository.ToList();

            //return View(deskList);
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

            var desks = from d in db.Desks select d;

            if (!String.IsNullOrEmpty(searchString))
            {
                desks = desks.Where(d => d.Desk_Id.Contains(searchString) || d.Occupant.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "occupant_desc":
                    {
                        desks = desks.OrderByDescending(d => d.Desk_Id);
                        break;
                    }
                case "occu":
                    {
                        desks = desks.OrderBy(d => d.Occupant);
                        break;
                    }
                case "occu_desc":
                    {
                        desks = desks.OrderByDescending(d => d.Occupant);
                        break;
                    }
                default: //desc Desk_Id
                    {
                        desks = desks.OrderBy(d => d.Desk_Id);
                        break;
                    }
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(desks.ToPagedList(pageNumber, pageSize));
            //return View(employees.ToList());
        }

        /// <summary>
        /// shows details about an entry is the desks table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(string id)
        {
            ActionResult result = null;
            if (id == null)
            {
                result = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Desk desk = db.Desks.Find(id);
            //Desk desk = _repository.Find(id);

            //if (desk == null)
            else
            {
                Desk desk = db.Desks.Find(id);
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
            ActionResult result = null;

            if (ModelState.IsValid)
            {
                //db.Desks.Add(desk);
                //db.SaveChanges();

                //if(!_repository.AlreadyExists(desk))
                //{
                    //_repository.Create(desk);
                    //return RedirectToAction("Index");
                //}
                //else
                //{
                    //ModelState.AddModelError("", "Desk already exists.");
                    //return View(desk);
                //}

                db.Desks.Add(desk);
                db.SaveChanges();
                result = RedirectToAction("Index");
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
            ActionResult result = null;
            if (id == null)
            {
                result = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Desk desk = db.Desks.Find(id);
            //Desk desk = _repository.Find(id);

            //if (desk == null)
            else
            {
                Desk desk = db.Desks.Find(id);
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
            ActionResult result = null;

            if (ModelState.IsValid)
            {
                //db.Entry(desk).State = EntityState.Modified;
                //db.SaveChanges();
                //_repository.Edit(desk);

                //return RedirectToAction("Index");
                db.Entry(desk).State = EntityState.Modified;
                db.SaveChanges();
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
            ActionResult result = null;
            if (id == null)
            {
                result = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Desk desk = db.Desks.Find(id);
            //Desk desk = _repository.Find(id);

            //if (desk == null)
            else
            {
                Desk desk = db.Desks.Find(id);
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
            //Desk desk = db.Desks.Find(id);
            //db.Desks.Remove(desk);
            //db.SaveChanges();
            //_repository.Delete(id);

            Desk d = new Desk();
            d.Desk_Id = " <Your string> ";
            db.Desks.Add(d);

            Desk desk = db.Desks.Find(id);
            db.Desks.Remove(desk);
            db.SaveChanges();
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
