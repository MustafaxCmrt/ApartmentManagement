using Microsoft.AspNetCore.Mvc;

namespace ApartmentManagement.API.Controllers.Common;

public class BaseController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}