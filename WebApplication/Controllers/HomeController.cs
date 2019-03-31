using MySql.Data.MySqlClient;
using System.Web.Mvc;
using DataStreamType;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        static DataBaseModuleFactory dbFactory;

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
                    return "사용할 수 있는 문자는 한글, 숫자, 영문자입니다.";
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
        public string authorNumberCheck(string inputNumber)
        {
            dbFactory = DataBaseModuleFactory.getInstance();
            IDataBase db = dbFactory.getDataBaseModule(dataBaseType.MySql);

            dbFactory = DataBaseModuleFactory.getInstance();

            try
            {
                db.connect();

                DataTable result = new DataTable();
                string checkQuery = "select * from authorInfo where authorNumber = @inputNumber";

                List<MySqlParameter> queryData = new List<MySqlParameter>();
                queryData.Add(new MySqlParameter("inputNumber", inputNumber));

                int count = db.inquire(ref result, checkQuery, queryData);
                db.close();

                if (count == 1)
                {
                    DataRow row = result.Rows[0];
                    string str = row["userId"] as string;
                    return (str == null) ? row["authorGrade"] as string : "Denied";
                }

                else
                {
                    return "Denied";
                }
            }

            catch
            {
                return "Error";
            }
        }

        [HttpPost]
        public string register(string inputId, string inputPs, string inputAuthNum)
        {
            bool idChecker = idCheck(inputId).Equals("Success");
            bool psChecker = psCheck(inputPs).Equals("Success");
            bool authNumChecker = (!authorNumberCheck(inputAuthNum).Equals("Denied")) && (!authorNumberCheck(inputAuthNum).Equals("Error"));

            if (!idChecker)
            {
                return "잘못된 아이디입니다.";
            }

            else if (!psChecker)
            {
                return "잘못된 비밀번호 입력입니다.";
            }

            else if (!authNumChecker)
            {
                return "잘못된 인증번호 입력입니다.";
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
                    string registerQuery = "insert into userInfo values (@userId, @userPs, @authNum)";
                    List<MySqlParameter> queryData = new List<MySqlParameter>();
                    queryData.Add(new MySqlParameter("userId", inputId));
                    queryData.Add(new MySqlParameter("userPs", inputPs));
                    queryData.Add(new MySqlParameter("authNum", inputAuthNum));
                    db.update(registerQuery, queryData);

                    // 인증번호 업데이트
                    string updateAuthNumQuery = "update authorInfo set userId = @userId where authorNumber = @authNum";
                    queryData.Clear();
                    queryData.Add(new MySqlParameter("userId", inputId));
                    queryData.Add(new MySqlParameter("authNum", inputAuthNum));
                    db.update(updateAuthNumQuery, queryData);

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
                    string userNumber = result.Rows[0]["userNumber"] as string;

                    Session["userId"] = inputId;
                    Session["authNum"] = userNumber;

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