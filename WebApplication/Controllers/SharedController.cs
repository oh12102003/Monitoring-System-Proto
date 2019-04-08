using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class SharedController : Controller
    {
        public ActionResult Loading()
        {
            return View();
        }

        public ActionResult Messaging()
        {
            // set alert message displayed
            if (Session["messageDisplay"] != null)
            {
                Session["messageDisplay"] = null;
            }

            else
            {
                Session["message"] = null;
            }

            return View("Loading");
        }

        public ActionResult SignOut()
        {
            return RedirectToAction("SignOut", "Home");
        }

        public ActionResult Recipe()
        {
            Session["message"] = null;
            Session["messageDisplay"] = null;
            Session["redirect"] = Url.Content("~/Recipe/Index");
            return View("Loading");
        }

        public ActionResult Monitoring()
        {
            Session["message"] = null;
            Session["messageDisplay"] = null;
            Session["redirect"] = Url.Content("~/Monitoring/Index");
            return View("Loading");
        }

        public ActionResult Management()
        {
            Session["message"] = null;
            Session["messageDisplay"] = null;
            Session["redirect"] = Url.Content("~/Home/Management");
            return View("Loading");
        }

        public ActionResult Account()
        {
            Session["message"] = null;
            Session["messageDisplay"] = null;
            Session["redirect"] = Url.Content("~/Home/Account");
            return View("Loading");
        }

        public ActionResult Home()
        {
            Session["message"] = null;
            Session["messageDisplay"] = null;
            Session["redirect"] = Url.Content("~/Home");
            return View("Loading");
        }

        public ActionResult WithDrawal()
        {
            Session["message"] = null;
            Session["messageDisplay"] = null;
            Session["redirect"] = Url.Content("~/Home/WithDrawal");
            return View("Loading");
        }
    }
}