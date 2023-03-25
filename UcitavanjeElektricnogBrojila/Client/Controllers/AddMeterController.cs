using Common;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;

namespace Client.Controllers
{
    public class AddMeterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddMeterView()
        {
            return View();
        }

        [HttpPost]
        [Route("/AddMeter/AddMeterMethod")]
        public Task<IActionResult>AddMeter(MeterDevice meter) 
        {
            return Task.FromResult<IActionResult>(View("Index"));
        }
    }
}
