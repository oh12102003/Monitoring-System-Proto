using DataStreamType;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class MonitoringController : Controller
    {
        // GET: Monitoring
        public ActionResult Index()
        {
            string userId = Session["userId"] as string;
            string userAuth = Session["userAuth"] as string;

            if (userId == null)
            {
                Session["message"] = "로그인 이후에 사용할 수 있습니다.";
                Session["redirect"] = Url.Content("~/Home");
                return RedirectToAction("Messaging", "Shared");
            }

            DataBaseModuleFactory dbFactory = DataBaseModuleFactory.getInstance();
            IDataBase db = dbFactory.getDataBaseModule(dataBaseType.MySql);

            dbFactory = DataBaseModuleFactory.getInstance();

            try
            {
                db.connect();

                DataTable dt = new DataTable();
                string checkQuery = "select * from userInfo where userId=@userId and userAuth=@userAuth";

                List<MySqlParameter> queryData = new List<MySqlParameter>();
                queryData.Add(new MySqlParameter("userId", userId));
                queryData.Add(new MySqlParameter("userAuth", userAuth));

                int count = db.inquire(ref dt, checkQuery, queryData);

                if (count == 1)
                {
                    if (int.Parse(userAuth) >= 1)
                    {
                        return View();
                    }

                    else
                    {
                        Session["message"] = "권한이 없습니다.";
                        Session["redirect"] = Url.Content("~/Home");
                        return RedirectToAction("Messaging", "Shared");
                    }
                }

                else
                {
                    Session["message"] = "잘못된 권한 요청입니다.";
                    Session["redirect"] = Url.Content("~/Home");
                    return RedirectToAction("Messaging", "Shared");
                }
            }

            catch
            {
                Session["message"] = "에러가 발생하였습니다.";
                Session["redirect"] = Url.Content("~/Home");
                return RedirectToAction("Messaging", "Shared");
            }
        }
    }
}