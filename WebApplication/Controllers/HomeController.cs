using MySql.Data.MySqlClient;
using System.Web.Mvc;
using DataStreamType;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        static DataBaseModuleFactory dbFactory;

        public ActionResult Index()
        {
            if (Session["userId"] == null)
            {
                return View("SignIn");
            }

            else
            {
                return View();
            }
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

        public ActionResult showUserList()
        {
            string userId = Session["userId"] as string;
            string userAuth = Session["userAuth"] as string;

            DataBaseModuleFactory dbFactory = DataBaseModuleFactory.getInstance();
            IDataBase db = dbFactory.getDataBaseModule(dataBaseType.MySql);

            dbFactory = DataBaseModuleFactory.getInstance();

            try
            {
                db.connect();

                if (int.Parse(userAuth) >= 4)
                {
                    string checkQuery = "select * from userInfo";
                    DataTable dt = new DataTable();
                    List<AuthInfo> userTable = new List<AuthInfo>();

                    db.inquire(ref dt, checkQuery);
                    db.close();

                    foreach (DataRow row in dt.Rows)
                    {
                        AuthInfo info = new AuthInfo();
                        info.authNumber = row["authorNumber"] as string;
                        info.userId = row["userId"] as string;
                        info.authGrade = row["authorGrade"] as string;

                        userTable.Add(info);
                    }

                    return View(userTable);
                }

                else
                {
                    db.close();
                    Session["message"] = "권한이 없습니다.";
                    Session["redirect"] = Url.Content("~/Home");
                    return RedirectToAction("Messaging", "Shared");
                }
            }

            catch
            {
                db.close();
                Session["message"] = "에러가 발생하였습니다.";
                Session["redirect"] = Url.Content("~/Home");
                return RedirectToAction("Messaging", "Shared");
            }
        }

        [HttpPost]
        public string idCheck(string inputId)
        {
            dbFactory = DataBaseModuleFactory.getInstance();
            IDataBase db = dbFactory.getDataBaseModule(dataBaseType.MySql);

            dbFactory = DataBaseModuleFactory.getInstance();

            try
            {
                db.connect();

                DataTable result = new DataTable();
                string checkQuery = "select * from userInfo where userId=@userId";

                List<MySqlParameter> queryData = new List<MySqlParameter>();
                queryData.Add(new MySqlParameter("userId", inputId));

                int count = db.inquire(ref result, checkQuery, queryData);
                db.close();

                if (count == 0)
                {
                    return "Success";
                }

                else
                {
                    return "Failed";
                }
            }

            catch
            {
                return "Error";
            }
        }


        [HttpPost]
        public string psCheck(string inputPs)
        {
            Regex regex = new Regex(@"[a-zA-Z0-9]");

            try
            {
                if (inputPs.Length < 5)
                {
                    return "비밀번호는 5자리 이상이어야 합니다.";
                }

                else if (!regex.IsMatch(inputPs))
                {
                    return "한글, 숫자, 영문자만 사용 가능합니다.";
                }

                else
                {
                    return "Success";
                }
            }

            catch
            {
                return "Error";
            }
        }

        [HttpPost]
        public string register(string inputId, string inputPs)
        {
            bool idChecker = idCheck(inputId).Equals("Success");
            bool psChecker = psCheck(inputPs).Equals("Success");

            if (!idChecker)
            {
                return "잘못된 아이디입니다.";
            }

            else if (!psChecker)
            {
                return "잘못된 비밀번호 입력입니다.";
            }

            else
            {
                DataBaseModuleFactory dbFactory = DataBaseModuleFactory.getInstance();
                IDataBase db = dbFactory.getDataBaseModule(dataBaseType.MySql);

                try
                {
                    db.connect();

                    DataTable result = new DataTable();

                    // 사용자 등록
                    string registerQuery = "insert into userInfo values (@userId, @userPs, @userAuth)";
                    List<MySqlParameter> queryData = new List<MySqlParameter>();
                    queryData.Add(new MySqlParameter("userId", inputId));
                    queryData.Add(new MySqlParameter("userPs", inputPs));
                    queryData.Add(new MySqlParameter("userAuth", "0"));
                    db.update(registerQuery, queryData);

                    db.close();

                    return "Success";
                }
                
                catch
                {
                    return "회원가입 중 오류가 발생하였습니다.";
                }
            }
        }



        [HttpPost]
        public ActionResult checkLogin(string inputId, string inputPs)
        {
            DataBaseModuleFactory dbFactory = DataBaseModuleFactory.getInstance();
            IDataBase db = dbFactory.getDataBaseModule(dataBaseType.MySql);

            try
            {
                db.connect();

                DataTable result = new DataTable();

                string checkQuery = "select * from userInfo where userId = @userId and userPs = @userPs";
                List<MySqlParameter> queryData = new List<MySqlParameter>();
                queryData.Add(new MySqlParameter("userId", inputId));
                queryData.Add(new MySqlParameter("userPs", inputPs));

                int count = db.inquire(ref result, checkQuery, queryData);

                if (count == 1)
                {
                    string userNumber = result.Rows[0]["userAuth"] as string;

                    Session["userId"] = inputId;
                    Session["userAuth"] = userNumber;

                    Session["message"] = "로그인 되었습니다.";
                    Session["redirect"] = Url.Content("~/Home");

                    return RedirectToAction("Messaging", "Shared");
                }

                else
                {
                    Session["message"] = "없는 아이디이거나 비밀번호가 일치하지 않습니다.";
                    Session["redirect"] = Url.Content("~/Home/SignIn");

                    return RedirectToAction("Messaging", "Shared");
                }
            }

            catch
            {
                Session["message"] = "에러가 발생하였습니다.";
                Session["redirect"] = Url.Content("~/Home/SignIn");

                return RedirectToAction("Messaging", "Shared");
            }
        }

        public ActionResult SignOut()
        {
            Session.Clear();
            return RedirectToAction("LogOut", "Shared");
        }
    }
}