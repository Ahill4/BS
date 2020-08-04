/*******************************************************************************
 * @file
 * @brief Controller for the Employee model.
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
using Microsoft.Ajax.Utilities;

namespace BSDB.Controllers
{
    /// <summary>
    /// Controller for the Employee table of the database
    /// </summary>
    public class EmployeesController : Controller
    {
        private BakerySquareDirectoryEntities db = new BakerySquareDirectoryEntities();

        /// <summary>
        /// handles the search and sort functionality
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public ActionResult Index(string sortOrder, string searchString)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var employees = from e in db.Employees select e;

            if (!String.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e => e.Name.Contains(searchString) || e.Email.Contains(searchString) || 
                e.Phone.Contains(searchString) || e.Email.Contains(searchString) || e.Desk.Contains(searchString) || 
                e.Manager.Contains(searchString) || e.Id.Contains(searchString) || e.Title.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    {
                        employees = employees.OrderByDescending(e => e.Name);
                        break;
                    }
                default:
                    {
                        employees = employees.OrderBy(e => e.Name);
                        break;
                    }
            }
            return View(employees.ToList());
        }

        /// <summary>
        /// shows details about the selected entry from the employee table
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
            else
            {
                Employee employee = db.Employees.Find(id);
                if (employee == null)
                {
                    result = HttpNotFound();
                }
                else
                {
                    result = View(employee);
                }
            }

            return result;
        }

        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// adds an entry to the employee table
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Title,Desk,Email,Phone,Id,Manager")] Employee employee)
        {
            ActionResult result = null;
            Boolean deskFound = false;
            Boolean deskEmpty = false;
            
            Desk d = db.Desks.Find(employee.Desk);
            if (d == null)
            {
                ViewBag.message = "Desk not found.";
            }
            else
            {
                deskFound = true;

                if (d.Occupant.IsNullOrWhiteSpace())
                {
                    deskEmpty = true;
                }
                else
                {
                    ViewBag.message = "This desk is already occupied by " + d.Occupant;
                }
            }

            if (ModelState.IsValid && deskFound == true && deskEmpty == true)
            {
                db.Employees.Add(employee);
                db.SaveChanges();
                result = RedirectToAction("Index");
            }
            else
            {
                result = View(employee);
            }

            return result;
        }

        /// <summary>
        /// edits an entry from the employee table
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
            else
            {
                Employee employee = db.Employees.Find(id);
                if (employee == null)
                {
                    result = HttpNotFound();
                }
                else
                {
                    result = View(employee);
                }
            }
            
            return result;
        }

        /// <summary>
        /// Saves changes to the database
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Name,Title,Desk,Email,Phone,Id,Manager")] Employee employee)
        {
            ActionResult result = null;

            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                result = RedirectToAction("Index");
            }
            else
            {
                result = View(employee);
            }

            return result;
        }

        /// <summary>
        /// deletes an entry from the employee table
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
            else
            {
                Employee employee = db.Employees.Find(id);
                if (employee == null)
                {
                    result = HttpNotFound();
                }
                else
                {
                    result = View(employee);
                }               
            }

            return result;
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
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
