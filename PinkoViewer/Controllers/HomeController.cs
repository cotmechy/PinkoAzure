using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PinkoViewer.Controllers
{           
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Pinko Expression Samples";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult SusbcribeSample()
        {
            return View();
        }

        public ActionResult PinkoChartView()
        {
            return View();
        }
        
    }
}
