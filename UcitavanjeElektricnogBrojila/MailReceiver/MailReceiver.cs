using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Interfaces;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Runtime;
using Spire.Email;
using Spire.Email.Pop3;

namespace MailReceiver
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class MailReceiver : StatefulService
    {
        public MailReceiver(StatefulServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }

                await CheckMailsRecived();

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
        public async Task CheckMailsRecived()
        {
            try
            {
                Pop3Client pop = new Pop3Client();
                pop.Host = "pop.gmail.com";
                pop.Username = "cloudmasterelektricnobrojilo@gmail.com";
                pop.Password = "likduhrkvvrvevbl";
                pop.Port = 995;
                pop.EnableSsl = true;
                pop.Connect();
                int numberofMails = pop.GetMessageCount();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    for (int i = 1; i <= numberofMails; i++)
                    {
                        MailMessage message = pop.GetMessage(i);
                        string[] mail = message.BodyText.Split(';');
                        try
                        {
                            MeterState meterState = new MeterState();
                            int stateId = Convert.ToInt32(mail[0]);
                            int meterId = Convert.ToInt32(mail[1]);
                            string city = mail[2];
                            string oldState = mail[3];
                            string newState = mail[4];


                            FabricClient fabricClient = new System.Fabric.FabricClient();
                            int partitionsNumber = (await fabricClient.QueryManager.GetPartitionListAsync(new Uri("fabric:/UcitavanjeElektricnogBrojila/Saver"))).Count;
                            int index = 0;

                            for (int j = 0; j < partitionsNumber; j++)
                            {
                                var proxy = ServiceProxy.Create<ISaver>(
                                new Uri("fabric:/UcitavanjeElektricnogBrojila/Saver"),
                                new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(index % partitionsNumber)
                                );

                                await proxy.AddMeterState(new MeterState(stateId, meterId,city, oldState, newState));

                                index++;
                            }

                        }
                        catch
                        {
                            ServiceEventSource.Current.Message("Wrong format of email!");
                        }
                    }
                    await tx.CommitAsync();
                }
                pop.DeleteAllMessages();
                pop.Disconnect();

            }
            catch
            {
                ServiceEventSource.Current.Message("The email service is currently down.");
                
            }

}
    }
}
