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
            // message showing checker reset 
            if (Session["userId"] == null)
            {
                Session["redirect"] = Url.Action("SignIn");
                return RedirectToAction("Messaging", "Shared");
            }

            else
            {
                Session["redirect"] = Url.Action("Index");
                return View();
            }
        }

        public ActionResult SignIn()
        {
            return View();
        }

        public ActionResult WithDrawal()
        {
            return View();
        }

        public ActionResult SignUp()
        {
            Session["messageDisplay"] = null;
            Session["message"] = null;
            return View();
        }

        public ActionResult SignOut()
        {
            Session.Clear();

            Session["messageDisplay"] = "true";
            Session["message"] = "로그아웃 되었습니다.";
            Session["messageType"] = "info";

            return RedirectToAction("Index", "Home");
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

                if (Session["management"].Equals("true"))
                {
                    string checkQuery = string.Format("select * from userInfo where userAuth < {0} order by userAuth desc", userAuth).ToString();
                    DataTable dt = new DataTable();
                    List<AuthInfo> userTable = new List<AuthInfo>();

                    db.inquire(ref dt, checkQuery);

                    foreach (DataRow row in dt.Rows)
                    {
                        AuthInfo info = new AuthInfo();
                        info.userId = row["userId"] as string;
                        info.authGrade = row["userAuth"] as string;
                        info.userName = row["userName"] as string;

                        info.monitoring = row["monitoring"] as string;
                        info.recipe = row["recipe"] as string;
                        info.management = row["management"] as string;
                        
                        userTable.Add(info);
                    }
                    
                    db.close();

                    return View(userTable);
                }

                else
                {
                    db.close();

                    Session["messageDisplay"] = "true";
                    Session["message"] = "권한이 없습니다.";
                    Session["messageType"] = "warning";
                    Session["redirect"] = Url.Content("~/Home");
                    return RedirectToAction("Messaging", "Shared");
                }
            }

            catch
            {
                db.close();

                Session["messageDisplay"] = "true";
                Session["message"] = "에러가 발생하였습니다.";
                Session["messageType"] = "danger";
                Session["redirect"] = Url.Content("~/Home");
                return RedirectToAction("Messaging", "Shared");
            }
        }

        [HttpPost]
        public string userManagement(string deletedString, string updatedString)
        {
            DataBaseModuleFactory dbFactory = DataBaseModuleFactory.getInstance();
            IDataBase db = dbFactory.getDataBaseModule(dataBaseType.MySql);

            dbFactory = DataBaseModuleFactory.getInstance();
            db.connect();

            try
            {
                UserAuthList deletedList, updatedList;

                if (UserAuthList.TryParse(deletedString, out deletedList) && UserAuthList.TryParse(updatedString, out updatedList))
                {
                    if (deletedList.length == 0 && updatedList.length == 0)
                    {
                        Session["messageDisplay"] = "true";
                        Session["message"] = "변경된 유저 정보가 없습니다.";
                        Session["messageType"] = "warning";
                        Session["redirect"] = Url.Action("Management");
                    }

                    else
                    {
                        foreach (var deletedUser in deletedList)
                        {
                            string deleteQuery = "delete from userInfo where userId = @userId";

                            List<MySqlParameter> queryData = new List<MySqlParameter>();
                            queryData.Add(new MySqlParameter("userId", deletedUser.userId));

                            db.update(deleteQuery, queryData);
                        }

                        foreach (var updatedUser in updatedList)
                        {
                            string updateQuery = "update userInfo set monitoring = @mon, recipe = @rec, management = @man where userId = @userId";

                            List<MySqlParameter> queryData = new List<MySqlParameter>();
                            queryData.Add(new MySqlParameter("mon", updatedUser.monitoringAuth));
                            queryData.Add(new MySqlParameter("rec", updatedUser.recipeAuth));
                            queryData.Add(new MySqlParameter("man", updatedUser.managementAuth));
                            queryData.Add(new MySqlParameter("userId", updatedUser.userId));

                            db.update(updateQuery, queryData);
                        }

                        Session["messageDisplay"] = "true";
                        Session["message"] = "유저 정보를 수정하였습니다.";
                        Session["messageType"] = "success";
                        Session["redirect"] = Url.Action("Management");
                    }
                }

                else
                {
                    Session["messageDisplay"] = "true";
                    Session["message"] = "에러가 발생하였습니다.";
                    Session["messageType"] = "danger";
                    Session["redirect"] = Url.Action("Management");
                }
            }

            catch
            {
                Session["messageDisplay"] = "true";
                Session["message"] = "에러가 발생하였습니다.";
                Session["messageType"] = "danger";
                Session["redirect"] = Url.Action("Management");
            }

            finally
            {
                db.close();
            }

            return Url.Content("~/Shared/Messaging");
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
                Session["messageDisplay"] = "true";
                Session["message"] = "잘못된 아이디를 입력하였습니다.";
                Session["messageType"] = "danger";
                Session["redirect"] = Url.Content("~/Home/SignUp");

                return RedirectToAction("Messaging", "Shared");
            }

            else if (!psChecker)
            {
                Session["messageDisplay"] = "true";
                Session["message"] = "잘못된 비밀번호를 입력하였습니다.";
                Session["messageType"] = "danger";
                Session["redirect"] = Url.Content("~/Home/SignUp");

                return RedirectToAction("Messaging", "Shared");
            }

            else if (!nameChecker)
            {
                Session["messageDisplay"] = "true";
                Session["message"] = "올바르지 않은 이름을 입력하였습니다.";
                Session["messageType"] = "danger";
                Session["redirect"] = Url.Content("~/Home/SignUp");

                return RedirectToAction("Messaging", "Shared");
            }

            else if (!userPs.Equals(userPsAgain))
            {
                Session["messageDisplay"] = "true";
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
                    queryData.Add(new MySqlParameter("userMonitoring", "false"));
                    queryData.Add(new MySqlParameter("userRecipe", "false"));
                    queryData.Add(new MySqlParameter("userManagement", "false"));
                    queryData.Add(new MySqlParameter("userAuth", "0"));
                    db.update(registerQuery, queryData);

                    db.close();

                    Session["messageDisplay"] = "true";
                    Session["message"] = "회원가입 되었습니다.";
                    Session["messageType"] = "success";
                    Session["redirect"] = Url.Content("~/Home/");

                    return RedirectToAction("Messaging", "Shared");
                }
                
                catch
                {
                    Session["messageDisplay"] = "true";
                    Session["message"] = "회원 가입 중 에러가 발생하였습니다.";
                    Session["messageType"] = "danger";
                    Session["redirect"] = Url.Content("~/Home/SignUp");

                    return RedirectToAction("Messaging", "Shared");
                }
            }
        }



        [HttpPost]
        public ActionResult unRegisterUser(string userId, string userPs)
        {
            bool idChecker = idCheck(userId).Equals("Duplicate");
            bool psChecker = psCheck(userPs).Equals("Success");

            if (!idChecker)
            {
                Session["messageDisplay"] = "true";
                Session["message"] = "잘못된 접근입니다.";
                Session["messageType"] = "danger";
                Session["redirect"] = Url.Content("~/Home/WithDrawal");
            }

            else if (!psChecker)
            {
                Session["messageDisplay"] = "true";
                Session["message"] = "잘못된 비밀번호 입력입니다.";
                Session["messageType"] = "warning";
                Session["redirect"] = Url.Content("~/Home/WithDrawal");
            }

            else
            {
                DataBaseModuleFactory dbFactory = DataBaseModuleFactory.getInstance();
                IDataBase db = dbFactory.getDataBaseModule(dataBaseType.MySql);

                try
                {
                    db.connect();

                    DataTable result = new DataTable();

                    // 사용자 체크
                    string findUserQuery = "select * from userInfo where userId = @userId and userPs = @userPs";
                    List<MySqlParameter> queryData = new List<MySqlParameter>();
                    queryData.Add(new MySqlParameter("userId", userId));
                    queryData.Add(new MySqlParameter("userPs", userPs));

                    // 검색 결과 사용자가 없거나 2명 이상인 경우
                    if (db.inquire(findUserQuery, queryData) != 1)
                    {
                        Session["messageDisplay"] = "true";
                        Session["message"] = "비밀번호가 일치하지 않습니다.";
                        Session["messageType"] = "warning";
                        Session["redirect"] = Url.Content("~/Home/WithDrawal");
                    }

                    else
                    {
                        string withDrawalQuery = "delete from userInfo where userId = @userId";

                        queryData.Clear();
                        queryData.Add(new MySqlParameter("userId", userId));
                        db.update(withDrawalQuery, queryData);

                        Session.Clear();

                        Session["messageDisplay"] = "true";
                        Session["message"] = "탈퇴처리 되었습니다.";
                        Session["messageType"] = "success";

                        Session["redirect"] = Url.Content("~/Home/");
                    }
                }

                catch
                {
                    Session["messageDisplay"] = "true";
                    Session["message"] = "회원 탈퇴 중 에러가 발생하였습니다.";
                    Session["messageType"] = "danger";
                    Session["redirect"] = Url.Content("~/Home/WithDrawal");
                }

                finally
                {
                    db.close();
                }
            }
            return RedirectToAction("Messaging", "Shared");
        }

        [HttpPost]
        public ActionResult changePs(string userInputPs, string userInputNewPs, string userInputNewPsAgain)
        {
            string userId = Session["userId"] as string;

            bool idChecker = idCheck(userId).Equals("Duplicate");
            bool psChecker = psCheck(userInputPs).Equals("Success");

            if (!idChecker)
            {
                Session["messageDisplay"] = "true";
                Session["message"] = "잘못된 접근입니다.";
                Session["messageType"] = "danger";
                Session["redirect"] = Url.Content("~/Home");

                return RedirectToAction("Messaging", "Shared");
            }

            else if (!psChecker)
            {
                Session["messageDisplay"] = "true";
                Session["message"] = "잘못된 비밀번호를 입력하였습니다.";
                Session["messageType"] = "danger";
                Session["redirect"] = Url.Content("~/Home/Account");

                return RedirectToAction("Messaging", "Shared");
            }

            else if (!userInputNewPs.Equals(userInputNewPsAgain))
            {
                Session["messageDisplay"] = "true";
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
                        Session["messageDisplay"] = "true";
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

                        Session["messageDisplay"] = "true";
                        Session["message"] = "회원정보가 수정 되었습니다.";
                        Session["messageType"] = "success";
                        Session["redirect"] = Url.Content("~/Home/");

                        return RedirectToAction("Messaging", "Shared");
                    }
                }

                catch
                {
                    Session["messageDisplay"] = "true";
                    Session["message"] = "회원정보 수정 중 에러가 발생하였습니다.";
                    Session["messageType"] = "danger";
                    Session["redirect"] = Url.Content("~/Home/Account");

                    return RedirectToAction("Messaging", "Shared");
                }

                Session["messageDisplay"] = "true";
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

                    Session["messageDisplay"] = "true";
                    Session["message"] = string.Format("환영합니다! {0}님!", result.Rows[0]["userName"]);
                    Session["messageType"] = "success";
                    Session["redirect"] = Url.Content("~/Home");

                    return RedirectToAction("Messaging", "Shared");
                }

                else
                {   
                    Session["messageDisplay"] = "true";
                    Session["message"] = "없는 아이디이거나 비밀번호가 일치하지 않습니다.";
                    Session["messageType"] = "danger";
                    Session["redirect"] = Url.Content("~/Home/SignIn");

                    return RedirectToAction("Messaging", "Shared");
                }
            }

            catch
            {
                Session["messageDisplay"] = "true";
                Session["message"] = "에러가 발생하였습니다.";
                Session["messageType"] = "danger";
                Session["redirect"] = Url.Content("~/Home/SignIn");

                return RedirectToAction("Messaging", "Shared");
            }
        }
    }
}