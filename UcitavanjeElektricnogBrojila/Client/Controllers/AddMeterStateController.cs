using Common;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class AddMeterStateController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddMeterStateView()
        {
            return View();
        }
        [HttpPost]
        [Route("/AddMeterState/AddMeterStateMethod")]
        public Task<IActionResult> AddMeterStateMethod(MeterState state)
        {
            return Task.FromResult<IActionResult>(View("Index"));
        }
    }
}
