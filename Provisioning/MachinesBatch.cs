using MachineScanner;
using System;
using System.Collections.Generic;
using System.Threading;
using Logger;
using Persistence;
using Configuration;

namespace Provisioning
{
    public class MachinesBatch
    {
        private readonly object _threadsCountLock = new object();

        /// <summary>
        /// Number of active threads
        /// </summary>
        public int ThreadsCount
        {
            get
            {
                return _threadsCount;
            }
            private set
            {
                if (_threadsCount == value) return;
                lock(_threadsCountLock)
                {
                    _threadsCount = value;
                    RaiseOnThreadsCountChanged(new ThdEventArgs(_threadsCount));
                }
            }
        }

        private int _threadsCount;

        /// <summary>
        /// Maximum number of threads
        /// </summary>
        public int MaxThreadsCount { get; set; }

        public string Name { get; set; }
        public Dictionary<Machine, MachineScanner.IScanner> Machines { get; set; }

        public OperatingSystems OperatingSystem { get; set; }

        public string User { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public string SshKeyFileName { get; set; }
        public IEnumerable<Kpi> Kpis { get; set; }

        /// <summary>
        /// Machines provider (AD, network range, etc)
        /// </summary>
        public IScanner MachinesSource { get; set; }

        /// <summary>
        /// Used for saving data
        /// </summary>
        public IPersistence Persistence { get; set; }

        #region ctor
        public MachinesBatch()
        {
            ThreadsCount = 0;
            MaxThreadsCount = 10;
            Machines = new Dictionary<Machine, MachineScanner.IScanner>();
        }
        #endregion


        public void GetMachinesFromSource()
        {
            if (MachinesSource == null) return;
            var computers = MachinesSource.GetComputers();
            computers.ForEach(i => AddMachine(i, OperatingSystem, Kpis));
        }

        public void AddMachine(string machineName, OperatingSystems operatingSystem, IEnumerable<Kpi> kpis)
        {
            var machine = new Machine(machineName);
            if (operatingSystem == OperatingSystems.Unknown)
            {
                operatingSystem = GenericScanner.GetMachineOs(machineName, User, Password, SshKeyFileName);
            }
            var scanner = ScannerFactory.GetScanner(operatingSystem);
            scanner.Persistence = Persistence;
            scanner.SetKpis(kpis);
            Machines.Add(machine, scanner);
            Log.Debug("Machine {0} added to batch {1}", machineName, Name);
        }

        public void Scan()
        {
            RaiseOnStart(new BatchEventArgs(string.Empty));
            foreach (var machine in Machines.Keys)
            {
                while (ThreadsCount >= MaxThreadsCount)
                {
                    Thread.Sleep(0);
                }
                var t = new Thread(() =>
                {
                    var scanner = Machines[machine];
                    scanner.SetMachine(machine.Name);
                    scanner.SetUserName(User);
                    scanner.SetPassword(Password);
                    scanner.SetDomain(Domain);
                    scanner.SetPrivateKeyFileName(string.Empty);

                    scanner.OnStart += Scanner_OnStart;
                    scanner.OnDone += Scanner_OnDone;
                    scanner.OnFail += Scanner_OnFail;
                    scanner.Scan();
                    ThreadsCount--;
                });
                ThreadsCount++;
                t.Start();
            }

            //Wait for at least one thread to start
            Thread.Sleep(100);
            while (ThreadsCount > 0)
            {
                Thread.Sleep(0);
            }
            RaiseOnDone(new BatchEventArgs(string.Empty));
        }

        private void Scanner_OnFail(object sender, ScannerEventArgs e)
        {
            RaiseOnMachineScanFail(new BatchEventArgs(e.MachineName));
        }

        private void Scanner_OnDone(object sender, ScannerEventArgs e)
        {
            RaiseOnMachineScanDone(new BatchEventArgs(e.MachineName));
        }

        private void Scanner_OnStart(object sender, ScannerEventArgs e)
        {
            RaiseOnMachineScanStart(new BatchEventArgs(e.MachineName));
        }

        #region events
        public event EventHandler<BatchEventArgs> OnStart = delegate { };
        public event EventHandler<BatchEventArgs> OnDone = delegate { };
        public event EventHandler<BatchEventArgs> OnMachineScanStart = delegate { };
        public event EventHandler<BatchEventArgs> OnMachineScanDone = delegate { };
        public event EventHandler<BatchEventArgs> OnMachineScanFail = delegate { };
        public event EventHandler<ThdEventArgs> OnThreadsCountChanged = delegate { };

        protected virtual void RaiseOnStart(BatchEventArgs e)
        {
            OnStart(this, e);
        }

        protected virtual void RaiseOnDone(BatchEventArgs e)
        {
            OnDone(this, e);
        }

        protected virtual void RaiseOnMachineScanStart(BatchEventArgs e)
        {
            OnMachineScanStart(this, e);
        }

        protected virtual void RaiseOnMachineScanDone(BatchEventArgs e)
        {
            OnMachineScanDone(this, e);
        }
        protected virtual void RaiseOnMachineScanFail(BatchEventArgs e)
        {
            OnMachineScanFail(this, e);
        }
        protected virtual void RaiseOnThreadsCountChanged(ThdEventArgs e)
        {
            OnThreadsCountChanged(this, e);
        }

        #endregion
    }
}
