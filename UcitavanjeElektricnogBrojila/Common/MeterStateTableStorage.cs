using Microsoft.WindowsAzure.Storage.Table;
using System.ComponentModel.DataAnnotations;

namespace Common
{
    public class MeterStateTableStorage : TableEntity
    {

        public MeterStateTableStorage() { }

        public MeterStateTableStorage(int stateId, int meterId, string city, string oldState, string newState)
        {
            PartitionKey = stateId.ToString();
            RowKey = meterId.ToString();
            StateId = stateId;
            MeterId = meterId;
            City = city;
            OldState = oldState;
            NewState = newState;
        }

        [Key]
        public int StateId { get; set; }

        public int MeterId { get; set; }

        public string City { get; set; }

        public string OldState { get; set; }

        public string NewState { get; set; }
    }
}
