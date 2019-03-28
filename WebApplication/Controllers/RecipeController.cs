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

        public ActionResult Save(string jsonString)
        {
            JsonStream js = new JsonStream();
            DrinkList drinkList = DrinkList.Parse(jsonString);

            js.applyInputData(drinkList);

            return View("Index", null);
        }
    }
}