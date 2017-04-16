using System;

namespace MachineScanner
{
    public class ScannerEventArgs : EventArgs
    {
        public string Message { get; set; }
        public string MachineName { get; set; }

        public ScannerEventArgs(string machineName, string message = "")
        {
            MachineName = machineName;
            Message = message;
        }
    }
}
