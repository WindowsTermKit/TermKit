using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Node.net;
using System.IO;

namespace TermKitLocalServer
{
    class Program
    {
        static void Main(string[] args)
        {
            HostEngine engine = new HostEngine();
            Stream stream = new FileStream(args[0], FileMode.Open);
            object o = engine.Execute(stream);
            Console.WriteLine("Result: ", o);
        }
    }
}
