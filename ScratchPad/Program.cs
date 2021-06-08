using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ScratchPad {

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RouteAttribute : Attribute {
        public string Path { get; }
        public RouteAttribute(string path) => Path = path;
    }

    public abstract class MessageDispather<TMessageType> where TMessageType : class, new() {
        public abstract void Register<TParam, TResult>(Func<TParam, Task<TResult>> target);
        public abstract void Register<TParam>(Func<TParam, Task> target);
        public abstract Task<TMessageType> DispatchAsync(TMessageType message);
    }

    public class XDocumentMessageDispatcher : MessageDispather<XDocument> {
        readonly List<(string, Func<XDocument, Task<XDocument?>>)> _headers = new List<(string, Func<XDocument, Task<XDocument?>>)>();

        public override async Task<XDocument?> DispatchAsync(XDocument message) {
            foreach (var (route, target) in _headers) {
                if ((message.XPathEvaluate(route) as bool?) == true) {
                    return await target(message);
                }
            }
            // No header registered
            return null;
        }

        public override void Register<TParam, TResult>(Func<TParam, Task<TResult>> target) {
            throw new NotImplementedException();
        }

        public override void Register<TParam>(Func<TParam, Task> target) {
            throw new NotImplementedException();
        }
    }

    class Program {
        static async Task Main(string[] args) {
            var messageHandler = new XDocumentMessageDispatcher();
        }
    }
}
