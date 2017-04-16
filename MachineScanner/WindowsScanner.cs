using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using Newtonsoft.Json;
using Configuration;

namespace MachineScanner
{
    public class WindowsScanner : GenericScanner
    {
        #region props
        public int Timeout { get; set; } = 3;

        private ManagementScope _managementScope;
        public bool IsConnected
        {
            get
            {
                if (_managementScope == null || !_managementScope.IsConnected)
                    Open();
                return _managementScope != null && _managementScope.IsConnected;
            }
        }
        #endregion

        #region ctor
        public WindowsScanner()
        {
            Machine = string.Empty;
            UserName = string.Empty;
            Password = string.Empty;
            Kpis = null;
            Result = new Dictionary<string, List<object>>();
        }

        public WindowsScanner(string machine)
            :this()
        {
            Machine = machine;
        }
        public WindowsScanner(string machine, string userName, string password, IEnumerable<Kpi> kpis)
            : this(machine)
        {
            UserName = userName;
            Password = password;
            Kpis = kpis;

        }
        #endregion

        #region methods
        public override bool Open()
        {
            var options = !string.IsNullOrEmpty(UserName)
                ? new ConnectionOptions()
                {
                    Username = UserName,
                    Password = Password,
                    EnablePrivileges = true,
                    Impersonation = ImpersonationLevel.Impersonate,
                    Timeout = TimeSpan.FromSeconds(Timeout)
                }
                : new ConnectionOptions()
                {
                    EnablePrivileges = true,
                    Impersonation = ImpersonationLevel.Impersonate,
                    Timeout = TimeSpan.FromSeconds(Timeout)
                };

            var retries = _retries;
            while (retries-- >= 0)
            {
                try
                {
                    _managementScope = new ManagementScope(
                        $"\\\\{Machine}\\root\\cimv2", options);
                    _managementScope.Connect();
                    return true;
                }
                catch (ManagementException ex)
                {
                    LastError = ex;
                    if (ex.ErrorCode == ManagementStatus.LocalCredentials)
                    {
                        options = new ConnectionOptions()
                        {
                            EnablePrivileges = true,
                            Impersonation = ImpersonationLevel.Impersonate,
                            Timeout = TimeSpan.FromSeconds(Timeout)
                        };
                        retries++;
                    }
                    else
                    {
                        LastError = ex;
                    }

                }
                catch (Exception ex)
                {
                    LastError = ex;
                }
            }
            return false;
        }

        public bool ScanWmiClass(string wmiClass, List<Field> fields)
        {
            if (!Result.ContainsKey(wmiClass))
            {
                Result.Add(wmiClass, new List<object>());
            }

            var strProperties = "*";
            if (fields != null && fields.Count != 0)
            {
                strProperties = string.Join(",", fields.Select(f => f.Name));
            }

            var query = new ObjectQuery($"SELECT {strProperties} FROM {wmiClass}");
            var searcher = new ManagementObjectSearcher(_managementScope, query);
            var queryCollection = searcher.Get();

            foreach (var o in queryCollection)
            {
                var m = (ManagementObject) o;
                var item = m.Properties.Cast<PropertyData>()
                    .ToDictionary(property => property.Name, property => m.GetPropertyValue(property.Name));
                Result[wmiClass].Add(item);
            }
            return true;
        }

        public override int Scan()
        {
            base.Scan();

            if (!IsConnected)
            {
                OnRaiseFail(new ScannerEventArgs(Machine));
                return 0;
            }
            foreach (var kpi in Kpis)
            {
                ScanWmiClass(kpi.Command, kpi.Fields);
            }
            Persistence?.Save(Machine, GetResult());
            OnRaiseDone(new ScannerEventArgs(Machine));
            return 1;
        }

        public override bool Close()
        {
            _managementScope = null;
            return true;
        }

        public override string GetResult()
        {
            return JsonConvert.SerializeObject(Result, Formatting.Indented);
        }
        #endregion
    }

}
