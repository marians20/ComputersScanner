using Configuration;
using MachineScanner;
using Persistence;
using Provisioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Scanner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var s = GenericScanner.GetMachineOs("localhost", "", "", "");
            Console.WriteLine("localhost " + s);

            Console.Write("Done.\nPress any key to exit...");
            Console.ReadKey();
            Console.WriteLine();
        }
    }
}
