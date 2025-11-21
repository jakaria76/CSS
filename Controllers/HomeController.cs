using Microsoft.AspNetCore.Mvc;
using CSS.Models;
using System.Diagnostics;

namespace CSS.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult Privacy() => View();

        public IActionResult Error()
        {
            return View(new ErrorViewModel());
        }
    }
}
