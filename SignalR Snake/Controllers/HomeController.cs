using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SignalR_Snake.Models;

namespace SignalR_Snake.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Snake snake)
        {
            return RedirectToAction("Snek", "Home", snake);
            //return View("Snek",snake);
        }

        public ActionResult Snek(Snake snake)
        {
            //ViewData["Snake"] = snake;
            //ViewBag.Name = snake.Name;
            return View(snake);
        }
        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}