using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers
{
    public class UrgentOperationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
