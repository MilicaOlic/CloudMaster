using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface ISaver:IService
    {
        Task<bool> AddMeter(MeterDevice device);
        Task<bool> AddMeterState(MeterState state);
        Task<List<MeterDevice>> MeterDeviceGetAllData();
        Task<List<MeterState>> GetMeterStates();
    }
}
