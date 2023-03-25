using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class MeterDevice
    {
        public MeterDevice(string meterId, string meterState)
        {
            MeterId = meterId;
            MeterState = meterState;
        }
        public MeterDevice()
        { }

        [Key]
        [DataMember]
        public string MeterId { get; set; }
        [DataMember]
        public string MeterState { get; set; }


    }
}
