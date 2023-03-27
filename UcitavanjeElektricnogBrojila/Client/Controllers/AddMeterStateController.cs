using Common;
using Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Diagnostics.Metrics;
using System.Fabric;

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
        public async Task<IActionResult> AddMeterStateMethod(MeterState state)
        {
            if (state.StateId == 0 || state.MeterId == 0 || state.City == "" || state.OldState == "" || state.NewState == "")
            {
                ViewData["Error"] = "Fields can not be empty!.";
                return View("AddMeterStateView");
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

                    result = await proxy.AddMeterState(state);

                    index++;
                }

                if (result)
                {
                    ViewData["Error"] = "MeterState sucessfully added!";
                }
                else
                {
                    ViewData["Error"] = "MeterState not added! Try again";
                }
                ViewData["Error"] = "";
                return View("AddMeterStateView");
            }
            catch
            {
                ViewData["Error"] = "Korisnik NIJE Dodat!";
                return View("AddMeterStateView");
            }
        }
    }
}
