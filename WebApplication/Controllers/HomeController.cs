using MySql.Data.MySqlClient;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult SignIn()
        {
            return View();
        }

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult checkLogin(UserChecking inputChecker)
        {
            try
            {
                MySqlConnection connection = new MySqlConnection("Server=localhost;Database=monitoring;Uid=root;Pwd=1234;");
                connection.Open();

                string checkQuery = string.Format("select * from userInfo where userId = {0} and userPs = {1}", inputChecker.userId, inputChecker.userPs);

                MySqlCommand command = new MySqlCommand(checkQuery, connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.FieldCount > 1)
                {
                    return Json("success");
                }
            }

            catch
            {
                return Json("failed");
            }

            return Json("failed");
        }

        [HttpPost]
        public ActionResult checkSingIn()
        {
            try
            {
                return Json("success");
            }

            catch
            {
                return Json("failed");
            }
        }
    }
}