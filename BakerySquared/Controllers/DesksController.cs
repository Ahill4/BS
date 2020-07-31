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
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BSDB.Models;

namespace BakerySquared.Controllers
{
    public class DesksController : Controller
    {
        private BakerySquareDirectoryEntities db = new BakerySquareDirectoryEntities();

        // GET: Desks
        public ActionResult Index()
        {
            return View(db.Desks.ToList());
        }

        // GET: Desks/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Desk desk = db.Desks.Find(id);
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
                db.Desks.Add(desk);
                db.SaveChanges();
                return RedirectToAction("Index");
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
            Desk desk = db.Desks.Find(id);
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
                db.Entry(desk).State = EntityState.Modified;
                db.SaveChanges();
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
            Desk desk = db.Desks.Find(id);
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
