using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class SharedController : Controller
    {
        public ActionResult LogOut()
        {
            return View();
        }

        public ActionResult Messaging()
        {
            return View();
        }
    }
}