using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AboutManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
