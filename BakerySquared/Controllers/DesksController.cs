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

namespace BakerySquared.Controllers
{
    public class DesksController : Controller
    {
        private BakerySquareDirectoryEntities db = new BakerySquareDirectoryEntities();
        private IDesksRepository _repository;

        public DesksController()
        {
            this._repository = new EFDesksRepository();
        }


        // GET: Desks
        public ActionResult Index()
        {
            //IEnumerable<Desk> deskList = db.Desks.ToList();
            IEnumerable<Desk> deskList = _repository.ToList();

            return View(deskList);
        }

        // GET: Desks/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Desk desk = db.Desks.Find(id);
            Desk desk = _repository.Find(id);

            if (desk == null)
            {
                return HttpNotFound();
            }
            return View(desk);
        }

        // GET: Desks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Desks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Desk_Id,Occupant")] Desk desk)
        {
            if (ModelState.IsValid)
            {
                //db.Desks.Add(desk);
                //db.SaveChanges();

                if(!_repository.AlreadyExists(desk))
                {
                    _repository.Create(desk);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Desk already exists.");
                    return View(desk);
                }

            }

            return View(desk);
        }

        // GET: Desks/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Desk desk = db.Desks.Find(id);
            Desk desk = _repository.Find(id);

            if (desk == null)
            {
                return HttpNotFound();
            }
            return View(desk);
        }

        // POST: Desks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Desk_Id,Occupant")] Desk desk)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(desk).State = EntityState.Modified;
                //db.SaveChanges();
                _repository.Edit(desk);

                return RedirectToAction("Index");
            }
            return View(desk);
        }

        // GET: Desks/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Desk desk = db.Desks.Find(id);
            Desk desk = _repository.Find(id);

            if (desk == null)
            {
                return HttpNotFound();
            }
            return View(desk);
        }

        // POST: Desks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            //Desk desk = db.Desks.Find(id);
            //db.Desks.Remove(desk);
            //db.SaveChanges();
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
