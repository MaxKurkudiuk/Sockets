using Shared.Messages;
using Shared.Xml;
using System;
using System.Threading.Tasks;

namespace ScratchPad {
    class Program {
        static async Task Main(string[] args) {
            var messageHandler = new XDocumentMessageDispatcher();

            //register handler
            messageHandler.Register<HeartBeatRequestMessage, HeartBeatResponseMessage>(HeartBeatRequestHandler);
            messageHandler.Register<HeartBeatResponseMessage>(HeartBeatResponseHandler);

            //create hb message and serialize to XDocument
            var hbRequest = new HeartBeatRequestMessage {
                Id = "HB_0001",
                POSData = new POSData { Id = "POS_001" }
            };
            var hbRequestXDoc = XmlSerialization.Serialize(hbRequest);

            //dispatch the message
            var responseXDoc = await messageHandler.DispatchAsync(hbRequestXDoc);
            if (responseXDoc != null)
                await messageHandler.DispatchAsync(responseXDoc);
        }

        //Handler on the 'Server' side of the system
        [Route("/Message[@type='Request' and @action='HeartBeat']")]
        public static Task<HeartBeatResponseMessage> HeartBeatRequestHandler(HeartBeatRequestMessage request) {
            var response = new HeartBeatResponseMessage {
                Id = request.Id,
                POSData = request.POSData,
                Result = new Result { Status = Status.Success }
            };

            return Task.FromResult(response);
        }

        //Handler on the 'Client' side of the system
        [Route("/Message[@type='Response' and @action='HeartBeat']")]
        public static Task HeartBeatResponseHandler(HeartBeatResponseMessage request) {
            Console.WriteLine($"Received Response: {request?.Result?.Status}");
            return Task.CompletedTask;
        }
    }
}
