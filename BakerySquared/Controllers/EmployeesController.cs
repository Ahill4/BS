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
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using BakerySquared.Controllers;
//using BSDB.Models;
using BakerySquared.Models;
using Microsoft.Ajax.Utilities;
using PagedList;

namespace BakerySquared.Controllers
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
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.EmailSortParm = sortOrder == "email" ? "email_desc" : "email";
            ViewBag.PhoneSortParm = sortOrder == "phone" ? "phone_desc" : "phone";
            ViewBag.DeskSortParm = sortOrder == "desk" ? "desk_desc" : "desk";
            ViewBag.MngrSortParm = sortOrder == "mngr" ? "mngr_desc" : "mngr";
            ViewBag.TitleSortParm = sortOrder == "title" ? "title_desc" : "title";
            ViewBag.IdSortParm = sortOrder == "id" ? "id_desc" : "id";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var employees = from e in db.Employees select e;

            if (!String.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e => e.Name.Contains(searchString) || e.Email.Contains(searchString) || 
                e.Phone.Contains(searchString) || e.Desk.Contains(searchString) || e.Manager.Contains(searchString) ||
                e.Id.Contains(searchString) || e.Title.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    {
                        employees = employees.OrderByDescending(e => e.Name);
                        break;
                    }
                case "desk_desc":
                    {
                        employees = employees.OrderByDescending(e => e.Desk);
                        break;
                    }
                case "desk":
                    {
                        employees = employees.OrderBy(e => e.Desk);
                        break;
                    }
                case "mngr_desc":
                    {
                        employees = employees.OrderByDescending(e => e.Manager);
                        break;
                    }
                case "mngr":
                    {
                        employees = employees.OrderBy(e => e.Manager);
                        break;
                    }
                case "title_desc":
                    {
                        employees = employees.OrderByDescending(e => e.Title);
                        break;
                    }
                case "title":
                    {
                        employees = employees.OrderBy(e => e.Title);
                        break;
                    }
                case "id_desc":
                    {
                        employees = employees.OrderByDescending(e => e.Id);
                        break;
                    }
                case "id":
                    {
                        employees = employees.OrderBy(e => e.Id);
                        break;
                    }
                case "phone_desc":
                    {
                        employees = employees.OrderByDescending(e => e.Phone);
                        break;
                    }
                case "phone":
                    {
                        employees = employees.OrderBy(e => e.Phone);
                        break;
                    }
                case "email_desc":
                    {
                        employees = employees.OrderByDescending(e => e.Email);
                        break;
                    }
                case "email":
                    {
                        employees = employees.OrderBy(e => e.Email);
                        break;
                    }
                default:
                    {
                        employees = employees.OrderBy(e => e.Name);
                        break;
                    }
            }

            int pageSize = 25;
            int pageNumber = (page ?? 1);
            return View(employees.ToPagedList(pageNumber, pageSize));
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
            Boolean deskEmpty = true;

            if (employee.Desk.IsNullOrWhiteSpace())
            {
                result = RedirectToAction("Index");
                deskFound = true;
                db.Employees.Add(employee);
                db.SaveChanges();
            }
            else
            {
                Desk d = new Desk();

                d = db.Desks.Find(employee.Desk);

                if (d == null)
                {
                    ViewBag.message = "Desk not found.";
                }
                else
                {
                    deskFound = true;

                    if (!d.Occupant.IsNullOrWhiteSpace())
                    {
                        deskEmpty = false;
                        ViewBag.message = "This desk is already occupied by " + d.Occupant;
                    }
                }

                if (ModelState.IsValid && deskFound == true && deskEmpty == true)
                {

                    db.Employees.Add(employee);
                    db.Desks.Attach(d);
                    db.Desks.Remove(d);
                    db.SaveChanges();
                    d.Occupant = employee.Name;
                    db.Desks.Add(d);
                    db.SaveChanges();
                    result = RedirectToAction("Index");
                }
                else
                {
                    result = View(employee);
                }
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
        /// If the desk assigned to a user matches the appropriate format for a desk then it will route to the 
        /// proper floor and highlight the desk
        /// </summary>
        /// <param name="desk"> the desk assigned to the user that is being found</param>
        /// <returns>url that directs to the proper floor or current page if not right format</returns>
        public ActionResult Find(string desk)
        {
            Regex rx = new Regex("^(M|D|S)[0-9]{4}$",
                   RegexOptions.Compiled | RegexOptions.IgnoreCase);
            string url = this.Request.UrlReferrer.AbsolutePath; 
            if(desk != null)
            {
                Match match = rx.Match(desk);
                if (match.Success)
                {
                    char floor = desk[1];
                    url = "../Home/Floor" + floor + "?ID=" + desk;
                }
            }

            return Redirect(url);
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

            Boolean b = true;
            if (!employee.Desk.IsNullOrWhiteSpace())
            {
                Desk desk = db.Desks.Find(employee.Desk);
                if (desk == null)
                {
                    b = false;
                    ViewBag.message = "Desk not found.";
                }
                else
                {
                    if ((!desk.Occupant.IsNullOrWhiteSpace()) && (!desk.Occupant.Equals(employee.Name)))
                    {
                        b = false;
                        ViewBag.message = "This desk is already occupied by " + desk.Occupant;
                    }
                    else
                    {
                        b = true;
                        db.Desks.Remove(desk);
                        db.SaveChanges();
                        desk.Occupant = employee.Name;
                        db.Desks.Add(desk);
                        db.SaveChanges();
                    }
                }
            }
            else //--------------------TEST THIS--------------------
            {
                foreach (Desk d in db.Desks.ToArray())
                {
                    if (d.Occupant.Trim().Equals(employee.Name.Trim()))
                    {
                        d.Occupant = null;
                        db.Desks.Remove(d);
                        db.SaveChanges();
                        db.Desks.Add(d);
                    }
                }
            }

            if (ModelState.IsValid && b)
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
            if (!employee.Desk.IsNullOrWhiteSpace())
            {
                Desk d = db.Desks.Find(employee.Desk);
                d.Occupant = null;
                db.Desks.Remove(d);
                db.SaveChanges();
                db.Desks.Add(d);
                db.SaveChanges();
            }
            
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
