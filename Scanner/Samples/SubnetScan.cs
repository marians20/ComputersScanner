﻿using System.Linq;
using Configuration;
using Logger;
using Persistence;
using Provisioning;

namespace Scanner.Samples
{

    public class SubnetScan
    {
        public string ConfigFileName
        {
            get
            {
                return _configFileName;
            }
            set
            {
                if (_configFileName == value) return;
                _configFileName = value;
                _config = Config.Load(ConfigFileName);
            }
        }
        public string Destination { get; set; }
        public string Domain { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        private string _configFileName = string.Empty;
        private Config _config;
        private readonly MachinesBatch _batch;

        #region ctor
        public SubnetScan(string configFileName, string destination, string domain, string userName, string password)
        {
            ConfigFileName = configFileName;
            Destination = destination;
            Domain = domain;
            UserName = userName;
            Password = password;

            var machinesSource = new IpRangeScanner
            {
                Ip = "192.168.1.1",
                Subnet = "255.255.255.0"
            };

            _batch = new MachinesBatch()
            {
                Name = "MySubnetMachines",
                OperatingSystem = OperatingSystems.Unknown,
                User = userName,
                Password = password,
                Kpis = _config.First(i => i.OperatingSystem == OperatingSystems.Windows).Kpis,
                Persistence = new FileSystemPersistence(destination),
                MachinesSource = machinesSource
            };
            _batch.OnStart += _batch_OnStart;
            _batch.OnDone += _batch_OnDone;
            _batch.OnMachineScanStart += _batch_OnMachineScanStart;
            _batch.OnMachineScanDone += _batch_OnMachineScanDone;
            _batch.OnMachineScanFail += _batch_OnMachineScanFail;
            _batch.OnThreadsCountChanged += _batch_OnThreadsCountChanged;
        }

        private void _batch_OnThreadsCountChanged(object sender, ThdEventArgs e)
        {
            Log.Debug("Threads count changed to {0}", e.ThreadsCount);
        }

        private void _batch_OnMachineScanFail(object sender, BatchEventArgs e)
        {
            Log.Debug("Fail scanning machine {0}", e.MachineName);
        }

        private void _batch_OnMachineScanDone(object sender, BatchEventArgs e)
        {
            Log.Debug("Done scanning machine {0}", e.MachineName);
        }

        private void _batch_OnMachineScanStart(object sender, BatchEventArgs e)
        {
            Log.Debug("Start scanning machine {0}", e.MachineName);
        }

        private void _batch_OnDone(object sender, BatchEventArgs e)
        {
            Log.Debug("Batch {0} done.", e.Data);
        }

        private void _batch_OnStart(object sender, BatchEventArgs e)
        {
            Log.Debug("Batch {0} started.", e.Data);
        }

        #endregion

        public void Scan()
        {
            _batch.GetMachinesFromSource();
            _batch.Scan();
        }
    }

    public class ProgramSubnet
    {
        public static void Main_Subnet()
        {
            var scanner = new AdScan(@"d:\\Scanner.yaml", @"d:\temp", string.Empty, "username", "password");
            scanner.Scan();
        }
    }
}
