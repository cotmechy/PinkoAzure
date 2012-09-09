using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PinkoWebService.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Constructor - HomeController 
        /// </summary>
        public HomeController()
        {
            
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}
