using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Messages {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RouteAttribute : Attribute {
        public string Path { get; }
        public RouteAttribute(string path) => Path = path;
    }
}
