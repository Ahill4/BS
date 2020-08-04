﻿/*******************************************************************************
 * @file
 * @brief Controlls all the Floor views as well as ther necessary server side functions
 *
 * *****************************************************************************
 *   Copyright (c) 2020 Koninklijke Philips N.V.
 *   All rights are reserved. Reproduction in whole or in part is
 *   prohibited without the prior written consent of the copyright holder.
 *******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using BSDB.Models;

namespace BakerySquared.Controllers
{
    /// <summary>
    /// Class containing the views for the Floor plans as well as controller methods to interact with database
    /// </summary>
    public class HomeController : Controller
    {
        private BakerySquareDirectoryEntities db = new BakerySquareDirectoryEntities();

        /// <summary>
        /// Displays Index page
        /// </summary>
        /// <returns> view containing index data</returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Displays about page
        /// </summary>
        /// <returns> view containing about data</returns>
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        /// <summary>
        /// Displays contact page
        /// </summary>
        /// <returns> view containing contact data</returns>
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        /// <summary>
        /// Displays Floor page
        /// </summary>
        /// <returns> view containing floor data</returns>
        public ActionResult Floor1()
        {
            return View();
        }

        /// <summary>
        /// Displays Floor page
        /// </summary>
        /// <returns> view containing floor data</returns>
        public ActionResult Floor4()
        {
            return View();
        }

        /// <summary>
        /// Displays Floor page
        /// </summary>
        /// <returns> view containing floor data</returns>
        public ActionResult Floor5()
        {
            return View();
        }

        /// <summary>
        /// Displays Floor page
        /// </summary>
        /// <returns> view containing floor data</returns>
        public ActionResult Floor6()
        {
            return View();
        }

        /// <summary>
        /// Displays Floor page
        /// </summary>
        /// <returns> view containing floor data</returns>
        public ActionResult Floor7()
        {
            return View();
        }

        /// <summary>
        /// takes the id sent from the client after user click and uses it to search the db
        /// </summary>
        /// <param name="id">id of the element that was clicked by user</param>
        /// <returns>returns Json string containing information for space</returns>
        [HttpGet]
        public ActionResult GetController(String id)
        {
            string returnString = null;
            var employees = from e in db.Employees select e;

            employees = employees.Where(e => e.Desk.Contains(id));

            employees.ToList();

            foreach (Employee e in employees)
            {
               string userId = e.Id + "\n";
                string userTitle = e.Title + "\n";
                string userPhone = e.Phone + "\n";
                string userDesk = e.Desk + "\n";
                string userEmail = e.Email + "\n";
                string userManager = e.Manager + "\n";
                returnString = "ID: " + userId + "Title: " + userTitle + "Phone: " + userPhone + "Desk: " + userDesk + "Email: "
                    + userEmail + "Manager: " + userManager;
            }

        if(returnString==null)
            {
                returnString = "Not Occupied";
            }
            return Json(returnString, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Will be used to remove all elements from the database from the current floor and refill it with 
        /// the elements from the file. To be used to update DB after floor plan change.
        /// </summary>
        /// <param name="floor">a string that is passed from the client to get the specific floor user is on 
        /// to update db</param>
        /// <returns>returns json string to client containing all floor location ids</returns>
        [HttpGet]
        public ActionResult refillDB(String floor)
        {
            string ids = FileRegex(floor);
            string[] locations = ids.Split(' ');
            
            return Json("Completed "+locations, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// opens the file of the floor map made in editor and searches for all ids that match the regular expression
        /// </summary>
        /// <param name="floor"> a string that is passed from the client to get the specific floor user is on 
        /// to update db</param>
        /// <returns> returns a string containing all the ids in the file that match the regular expression</returns>
        private string FileRegex(string floor)
        {
            Regex rx = new Regex("id=\"(M|D|S)[0-9]{4}\"",
                   RegexOptions.Compiled | RegexOptions.IgnoreCase);

            int counter = 0;
            string ids = "";
            string line;
            string path = HttpContext.Server.MapPath("~/Views/Floors/Floor" + floor + ".svg");

            //Read the file and display it line by line.
            System.IO.StreamReader file =
                new System.IO.StreamReader(path);
            while ((line = file.ReadLine()) != null)
            {
                Match match = rx.Match(line);
                if (match.Success)
                {
                    string clean = line.Replace("id=\"", "").Replace("\"", "").Trim();
                    ids = ids + " " + clean;
                    counter++;
                }
            }

            file.Close();
            return ids;
        }
    }
}