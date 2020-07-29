using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using BSDB.Models;

namespace cshtmlMix.Controllers
{

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Floor1()
        {
            ViewBag.Message = "Your floor 1";

            return View();
        }
        public ActionResult Floor4()
        {
            ViewBag.Message = "Your floor 4";

            return View();
        }
        public ActionResult Floor5()
        {
            ViewBag.Message = "Your floor 5";

            return View();
        }
        public ActionResult Floor6()
        {
            ViewBag.Message = "Your floor 6";

            return View();
        }
        public ActionResult Floor7()
        {
            ViewBag.Message = "Your floor 7";

            return View();
        }

        [HttpGet]
        public ActionResult GetController(String id)
        {
            String userId = id + " hello";
            return Json(userId, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult refillDB(String floor)
        {

            string ids = FileRegex(floor);
            string[] locations = ids.Split(' ');
            
            return Json("Completed "+locations, JsonRequestBehavior.AllowGet);
        }

        private string FileRegex(string floor)
        {
            Regex rx = new Regex("id=\"(M|D|S)[0-9]{4}\"",
                   RegexOptions.Compiled | RegexOptions.IgnoreCase);

            int counter = 0;
            string ids = "";
            string line;
            string path = HttpContext.Server.MapPath("~/Views/Floors/Floor" + floor + ".svg");

            System.Console.WriteLine(path);
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