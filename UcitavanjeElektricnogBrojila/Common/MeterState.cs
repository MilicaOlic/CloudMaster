using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class MeterState
    {
        public MeterState() { }

        public MeterState(int stateId, int meterId, string city, string oldState, string newState)
        {
            StateId = stateId;
            MeterId = meterId;
            City = city;
            OldState = oldState;
            NewState = newState;
        }

        [Key]
        [DataMember]
        public int StateId { get; set; }

        [DataMember]
        public int MeterId { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string OldState { get; set; }

        [DataMember]
        public string NewState { get; set; }
    }
}
