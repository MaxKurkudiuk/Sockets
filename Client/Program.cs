using Newtonsoft.Json.Linq;
using Shared;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Client
{
    public class MyMessage {
        public string StringProperty { get; set; }
        public int IntProperty { get; set; }
    }
    class Program {
        private const int PORT = 9000;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Press Enter to Connect");
            Console.ReadLine();

            var endPoint = new IPEndPoint(IPAddress.Loopback, PORT);

            var channel = new ClientChannel<JsonMessageProtocol, JObject>();
            //var channel = new ClientChannel<XmlMessageProtocol, XDocument>();

            channel.OnMessage(OnMessage);

            await channel.ConnectAsync(endPoint).ConfigureAwait(false);

            var myMessage = new MyMessage {
                IntProperty = 404,
                StringProperty = "Hello World!"
            };

            Console.WriteLine("Sending");
            Print(myMessage);

            await channel.SendAsync(myMessage).ConfigureAwait(false);

            Console.ReadLine();
        }

        static Task OnMessage(JObject jObject) {
            Console.WriteLine("Received JObject Message");
            Print(Convert(jObject));
            return Task.CompletedTask;
        }
        static Task OnMessage(XDocument xDoc) {
            Console.WriteLine("Received XDocument Message");
            Print(Convert(xDoc));
            return Task.CompletedTask;
        }

        static MyMessage Convert(JObject jObj)
            => jObj.ToObject(typeof(MyMessage)) as MyMessage;
        static MyMessage Convert(XDocument xDoc)
            => new XmlSerializer(typeof(MyMessage)).Deserialize(new StringReader(xDoc.ToString())) as MyMessage;

        static void Print(MyMessage m) => Console.WriteLine($"MyMessage.IntProperty = {m.IntProperty}, MyMessage.StringProperty = {m.StringProperty}");
    }
}
