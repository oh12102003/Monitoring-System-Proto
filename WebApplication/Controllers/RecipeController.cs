using DataStreamType;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class RecipeController : Controller
    {
        // GET: Recipe
        public ActionResult Index()
        {
            return View();
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