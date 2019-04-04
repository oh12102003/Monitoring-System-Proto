using DataStreamType;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class RecipeController : Controller
    {
        // GET: Recipe
        public ActionResult Index()
        {
            string userId = Session["userId"] as string;

            if (userId == null)
            {
                Session["message"] = "로그인 이후에 사용할 수 있습니다.";
                Session["messageType"] = "info";
                Session["redirect"] = Url.Content("~/Home");
                return RedirectToAction("Messaging", "Shared");
            }

            DataBaseModuleFactory dbFactory = DataBaseModuleFactory.getInstance();
            IDataBase db = dbFactory.getDataBaseModule(dataBaseType.MySql);

            dbFactory = DataBaseModuleFactory.getInstance();

            try
            {
                db.connect();

                string checkQuery = "select * from userInfo where userId=@userId";

                List<MySqlParameter> queryData = new List<MySqlParameter>();
                queryData.Add(new MySqlParameter("userId", userId));

                int count = db.inquire(checkQuery, queryData);

                if (count == 1)
                {
                    queryData.Clear();

                    if (Session["recipe"].Equals("true"))
                    {
                        Session["message"] = null;
                        return View();
                    }

                    else
                    {
                        Session["message"] = "권한이 없습니다.";
                        Session["messageType"] = "warning";
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

        [HttpPost]
        public ActionResult Save(string jsonString)
        {
            try
            {
                JsonStream js = new JsonStream();
                DrinkList drinkList = DrinkList.Parse(jsonString);
                js.hardApplyInputData(drinkList);

                return Json(jsonString);
            }

            catch
            {
                return null;
            }
        }
    }
}