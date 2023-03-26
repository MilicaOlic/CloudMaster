using Common;
using Common.Interfaces;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Diagnostics.Metrics;
using System.Fabric;

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
        public async Task<IActionResult> AddMeter(MeterDevice meter)
        {
            if (meter.MeterId == 0 || meter.MeterState == "")
            {
                ViewData["Error"] = "Fields can not be empty!.";
                return View("AddMeterView");
            }
            try
            {
                bool result = true;
                FabricClient fabricClient = new System.Fabric.FabricClient();
                int partitionsNumber = (await fabricClient.QueryManager.GetPartitionListAsync(new Uri("fabric:/UcitavanjeElektricnogBrojila/Saver"))).Count;
                int index = 0;

                for (int i = 0; i < partitionsNumber; i++)
                {
                    var proxy = ServiceProxy.Create<ISaver>(
                    new Uri("fabric:/UcitavanjeElektricnogBrojila/Saver"),
                    new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(index % partitionsNumber)
                    );

                    result = await proxy.AddMeter(meter);

                    index++;
                }

                if (result)
                {
                    ViewData["Error"] = "Meter sucessfully added!";
                }
                else
                {
                    ViewData["Error"] = "Meter not added! Try again";
                }

                return View("AddMeterView");
            }
            catch
            {
                ViewData["Error"] = "Korisnik NIJE Dodat!";
                return View("AddMeterView");
            }
        }
    }
}
