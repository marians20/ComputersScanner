using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MachineScanner;

namespace Scanner.Samples
{
    internal class GetOperatingSystem
    {
        static void Main_OS(string[] args)
        {
            var s = GenericScanner.GetMachineOs("localhost", "", "", "");
            Console.WriteLine("localhost " + s);

            Console.Write("Done.\nPress any key to exit...");
            Console.ReadKey();
            Console.WriteLine();
        }
    }
}
