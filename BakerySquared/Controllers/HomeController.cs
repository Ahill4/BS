using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        [HttpPost]
        public ActionResult refillDB(String locationsArr)
        {
            string[] words = locationsArr.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');

            return Json("Completed " + words[0], JsonRequestBehavior.AllowGet);
        }
    }
}