using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class MonitoringController : Controller
    {
        // GET: Monitoring
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Recipe()
        {/*
            if (ViewBag.user == null)
            {
                return View("NoRight", null);
            }

            else
            {*/
                return View();
            //}
        }
    }
}