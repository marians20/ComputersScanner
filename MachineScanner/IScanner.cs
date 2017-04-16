using Configuration;
using Persistence;
using System;
using System.Collections.Generic;

namespace MachineScanner
{
    public interface IScanner: IPrivateKeyAuth, IWindowsAuth
    {
        IPersistence Persistence { get; set; }
        void SetMachine(string machine);
        void SetUserName(string user);
        void SetPassword(string password);
        void SetKpis(IEnumerable<Kpi> kpis);
        bool Open();
        int Scan();
        bool Close();
        string GetResult();

        event EventHandler<ScannerEventArgs> OnStart;
        event EventHandler<ScannerEventArgs> OnDone;
        event EventHandler<ScannerEventArgs> OnFail;
    }
}
