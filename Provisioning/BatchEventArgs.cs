using System;

namespace Provisioning
{
    public class BatchEventArgs : EventArgs
    {
        public string Message { get; set; }
        public string MachineName { get; set; }

        public object Data { get; set; }

        public BatchEventArgs(string machineName, string message = "")
        {
            MachineName = machineName;
            Message = message;
        }
    }
}
