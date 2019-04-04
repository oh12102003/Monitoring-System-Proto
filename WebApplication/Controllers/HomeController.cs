using MySql.Data.MySqlClient;
using System.Web.Mvc;
using DataStreamType;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WebApplication.Models;
using System;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        static DataBaseModuleFactory dbFactory;

        public ActionResult Index()
        {
            if (Session["userId"] == null)
            {
                if (Session["register"] != null)
                {
                    Session["register"] = null;
                }

                else
                {
                    Session["message"] = null;
                }

                return View("SignIn");
            }

            else
            {
                if (Session["register"] != null)
                {
                    Session["register"] = null;
                }

                else
                {
                    Session["message"] = null;
                }

                Session["redirect"] = Url.Content("~/Messenger");

                return RedirectToAction("Messaging", "Shared");
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

        public ActionResult SignOut()
        {
            Session.Clear();

            Session["register"] = "logout";
            Session["message"] = "로그아웃 되었습니다.";
            Session["messageType"] = "info";
            Session["redirect"] = Url.Content("~/Home");

            return RedirectToAction("Messaging", "Shared");
        }


        public ActionResult Messenger()
        {
            Session["message"] = null;
            return View();
        }

        public ActionResult Management()
        {
            string userId = Session["userId"] as string;
            string userAuth = Session["userAuth"] as string;

            DataBaseModuleFactory dbFactory = DataBaseModuleFactory.getInstance();
            IDataBase db = dbFactory.getDataBaseModule(dataBaseType.MySql);

            dbFactory = DataBaseModuleFactory.getInstance();

            try
            {
                db.connect();

                if (Session["management"].ToString().Equals("True"))
                {
                    string checkQuery = string.Format("select * from userInfo where userAuth < {0} order by userAuth desc", userAuth);
                    DataTable dt = new DataTable();
                    List<AuthInfo> userTable = new List<AuthInfo>();

                    db.inquire(ref dt, checkQuery);

                    foreach (DataRow row in dt.Rows)
                    {
                        AuthInfo info = new AuthInfo();
                        info.userId = row["userId"] as string;
                        info.authGrade = row["userAuth"] as string;
                        info.userName = row["userName"] as string;

                        userTable.Add(info);
                    }

                    Session["message"] = null;
                    db.close();

                    return View(userTable);
                }

                else
                {
                    db.close();
                    Session["message"] = "권한이 없습니다.";
                    Session["messageType"] = "warning";
                    Session["redirect"] = Url.Content("~/Home");
                    return RedirectToAction("Messaging", "Shared");
                }
            }

            catch
            {
                db.close();
                Session["message"] = "에러가 발생하였습니다.";
                Session["messageType"] = "danger";
                Session["redirect"] = Url.Content("~/Home");
                return RedirectToAction("Messaging", "Shared");
            }
        }

        public ActionResult Account()
        {
            return View();
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

                else if (count == 1)
                {
                    return "Duplicate";
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
        public string nameCheck(string inputName)
        {
            Regex regex = new Regex(@"[a-zA-Z가-힣]");

            try
            {
                if (inputName.Length <= 0)
                {
                    return "이름을 입력하시지 않았습니다.";
                }

                else if (!regex.IsMatch(inputName))
                {
                    return "올바른 이름이 아닙니다.";
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
        public ActionResult register(string userId, string userName, string userPs, string userPsAgain)
        {
            bool idChecker = idCheck(userId).Equals("Success");
            bool nameChecker = nameCheck(userName).Equals("Success");
            bool psChecker = psCheck(userPs).Equals("Success");

            if (!idChecker)
            {
                Session["message"] = "잘못된 아이디를 입력하였습니다.";
                Session["messageType"] = "danger";
                Session["redirect"] = Url.Content("~/Home/SignUp");

                return RedirectToAction("Messaging", "Shared");
            }

            else if (!psChecker)
            {
                Session["message"] = "잘못된 비밀번호를 입력하였습니다.";
                Session["messageType"] = "danger";
                Session["redirect"] = Url.Content("~/Home/SignUp");

                return RedirectToAction("Messaging", "Shared");
            }

            else if (!nameChecker)
            {
                Session["message"] = "올바르지 않은 이름을 입력하였습니다.";
                Session["messageType"] = "danger";
                Session["redirect"] = Url.Content("~/Home/SignUp");

                return RedirectToAction("Messaging", "Shared");
            }

            else if (!userPs.Equals(userPsAgain))
            {
                Session["message"] = "비밀번호 입력 값과 비밀번호 확인 입력 값이 일치하지 않습니다.";
                Session["messageType"] = "danger";
                Session["redirect"] = Url.Content("~/Home/SignUp");

                return RedirectToAction("Messaging", "Shared");
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
                    string registerQuery = "insert into userInfo values (@userId, @userPs, @userName, @userMonitoring, @userRecipe, @userManagement, @userAuth)";
                    List<MySqlParameter> queryData = new List<MySqlParameter>();
                    queryData.Add(new MySqlParameter("userId", userId));
                    queryData.Add(new MySqlParameter("userPs", userPs));
                    queryData.Add(new MySqlParameter("userName", userName));
                    queryData.Add(new MySqlParameter("userMonitoring", "0"));
                    queryData.Add(new MySqlParameter("userRecipe", "0"));
                    queryData.Add(new MySqlParameter("userManagement", "0"));
                    queryData.Add(new MySqlParameter("userAuth", "0"));
                    db.update(registerQuery, queryData);

                    db.close();

                    Session["message"] = "회원가입 되었습니다.";
                    Session["messageType"] = "success";
                    Session["redirect"] = Url.Content("~/Home/");
                    Session["register"] = "register";

                    return RedirectToAction("Messaging", "Shared");
                }
                
                catch
                {
                    Session["message"] = "회원 가입 중 에러가 발생하였습니다.";
                    Session["messageType"] = "danger";
                    Session["redirect"] = Url.Content("~/Home/SignUp");

                    return RedirectToAction("Messaging", "Shared");
                }
            }
        }

        [HttpPost]
        public ActionResult changePs(string userInputPs, string userInputNewPs, string userInputNewPsAgain)
        {
            string userId = Session["userId"] as string;

            bool idChecker = idCheck(userId).Equals("Duplicate");
            bool psChecker = psCheck(userInputPs).Equals("Success");

            if (!idChecker)
            {
                Session["message"] = "잘못된 접근입니다.";
                Session["messageType"] = "danger";
                Session["redirect"] = Url.Content("~/Home");

                return RedirectToAction("Messaging", "Shared");
            }

            else if (!psChecker)
            {
                Session["message"] = "잘못된 비밀번호를 입력하였습니다.";
                Session["messageType"] = "danger";
                Session["redirect"] = Url.Content("~/Home/Account");

                return RedirectToAction("Messaging", "Shared");
            }

            else if (!userInputNewPs.Equals(userInputNewPsAgain))
            {
                Session["message"] = "비밀번호 입력 값과 비밀번호 확인 입력 값이 일치하지 않습니다.";
                Session["messageType"] = "danger";
                Session["redirect"] = Url.Content("~/Home/Account");

                return RedirectToAction("Messaging", "Shared");
            }

            else
            {
                DataBaseModuleFactory dbFactory = DataBaseModuleFactory.getInstance();
                IDataBase db = dbFactory.getDataBaseModule(dataBaseType.MySql);

                try
                {
                    db.connect();

                    string query = "select * from userInfo where userId = @userId and userPs = @userPs";
                    List<MySqlParameter> queryData = new List<MySqlParameter>();
                    queryData.Add(new MySqlParameter("userId", userId));
                    queryData.Add(new MySqlParameter("userPs", userInputPs));

                    if (db.inquire(query, queryData) != 1)
                    {
                        Session["message"] = "입력한 이전 비밀번호가 일치하지 않습니다.";
                        Session["messageType"] = "warning";
                        Session["redirect"] = Url.Content("~/Home/Account");
                    }

                    else
                    {
                        query = "update userInfo set userPs = @userPs where userId = @userId";
                        queryData.Clear();

                        queryData.Add(new MySqlParameter("userId", userId));
                        queryData.Add(new MySqlParameter("userPs", userInputNewPs));
                        db.update(query, queryData);
                        
                        db.close();

                        Session["message"] = "회원정보가 수정 되었습니다.";
                        Session["messageType"] = "success";
                        Session["redirect"] = Url.Content("~/Home/");
                        Session["register"] = "updated";

                        return RedirectToAction("Messaging", "Shared");
                    }
                }

                catch
                {
                    Session["message"] = "회원정보 수정 중 에러가 발생하였습니다.";
                    Session["messageType"] = "danger";
                    Session["redirect"] = Url.Content("~/Home/Account");

                    return RedirectToAction("Messaging", "Shared");
                }

                Session["message"] = "회원정보 수정 중 에러가 발생하였습니다.";
                Session["messageType"] = "danger";
                Session["redirect"] = Url.Content("~/Home/Account");

                return RedirectToAction("Messaging", "Shared");
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
                    Session["userId"] = inputId;

                    Session["userAuth"] = result.Rows[0]["userAuth"];
                    Session["monitoring"] = result.Rows[0]["monitoring"];
                    Session["recipe"] = result.Rows[0]["recipe"];
                    Session["management"] = result.Rows[0]["management"];

                    Session["messageType"] = "success";
                    Session["message"] = string.Format("환영합니다! {0}님!", inputId).ToString();
                    Session["register"] = "newLogin";
                    Session["redirect"] = Url.Content("~/Home");

                    return RedirectToAction("Messaging", "Shared");
                }

                else
                {
                    Session["message"] = "없는 아이디이거나 비밀번호가 일치하지 않습니다.";
                    Session["messageType"] = "danger";
                    Session["redirect"] = Url.Content("~/Home/SignIn");

                    return RedirectToAction("Messaging", "Shared");
                }
            }

            catch
            {
                Session["message"] = "에러가 발생하였습니다.";
                Session["messageType"] = "danger";
                Session["redirect"] = Url.Content("~/Home/SignIn");

                return RedirectToAction("Messaging", "Shared");
            }
        }
    }
}