using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using Configuration;
using System.Text.RegularExpressions;

namespace MachineScanner
{
    public class GenericScanner : IScanner
    {
        #region events
        public event EventHandler<ScannerEventArgs> OnStart;
        public event EventHandler<ScannerEventArgs> OnDone;
        public event EventHandler<ScannerEventArgs> OnFail;
        protected virtual void OnRaiseStart(ScannerEventArgs e)
        {
            var handler = OnStart;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRaiseDone(ScannerEventArgs e)
        {
            var handler = OnDone;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRaiseFail(ScannerEventArgs e)
        {
            var handler = OnFail;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region props
        public string Machine { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public string PrivateKeyFileName { get; set; }
        public string PassPhrase { get; set; }
        public IEnumerable<Kpi> Kpis { get; set; }
        public Dictionary<string, List<object>> Result { get; protected set; }
        public int Retries { get; set; }

        protected int _retries;
        public Exception LastError { get; protected set; }

        public IPersistence Persistence { get; set; }
        #endregion

        #region IScanner Setters
        public void SetMachine(string machine)
        {
            Machine = machine;
        }
        public void SetUserName(string user)
        {
            UserName = user;
        }
        public void SetPassword(string password)
        {
            Password = password;
        }
        public void SetKpis(IEnumerable<Kpi> kpis)
        {
            Kpis = kpis;
        }
        #endregion

        #region IPrivateKeyAuth Setters
        public void SetPrivateKeyFileName(string privateKeyFileName)
        {
            //Do nothing
        }
        public void SetPassPhrase(string passPhrase)
        {
            // Do nothing
        }
        #endregion

        #region IWindowsAuth Setters
        public void SetDomain(string domain)
        {
            Domain = domain;
        }
        #endregion

        #region ctor
        public GenericScanner(int retries = 3)
        {
            _retries = retries;
            Machine = string.Empty;
            UserName = string.Empty;
            Password = string.Empty;
            Kpis = null;
            Result = new Dictionary<string, List<object>>();
        }

        public GenericScanner(string machine, int retries)
            :this(retries)
        {
            Machine = machine;
        }

        public GenericScanner(string machine, string userName, string password, IEnumerable<Kpi> kpis, int retries)
            : this(machine, retries)
        {
            UserName = userName;
            Password = password;
            Kpis = kpis;

        }
        #endregion

        #region methods
        public virtual bool Open()
        {
            return true;
        }

        public virtual int Scan()
        {
            OnRaiseStart(new ScannerEventArgs(Machine));
            Result.Clear();
            Result.Add("Scanning",
                (new object[] { new
                {
                    machine = Machine,
                    time = DateTime.Now
                } }).ToList());
            return 1;
        }

        public virtual bool Close()
        {
            return true;
        }

        public virtual string GetResult()
        {
            return string.Empty;
        }

        public virtual bool Persist()
        {
            return true;
        }
        #endregion

        #region helpers
        /// <summary>
        /// In progress...
        /// Get the operating system of specified machine. The credentials must be provided.
        /// For now it can identify Ubuntu, Debian and Windows
        /// </summary>
        /// <param name="machineName"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="privateKeyFileName"></param>
        /// <returns></returns>
        public static OperatingSystems GetMachineOs(string machineName, string userName, string password, string privateKeyFileName)
        {
            //TODO: This function did not detect Windows on localhost!
            var result = OperatingSystems.Unknown;
            var ssh = new SshHelper.SshHelper()
            {
                Host = machineName,
                UserName = userName,
                Password = password,
                PrivateKeyFileName = privateKeyFileName
            };
            var os = ssh.Exec("cat /etc/issue");
            if(!string.IsNullOrEmpty(os))
            {
                os = os.ToLower();
                if(os.IndexOf("ubuntu", StringComparison.Ordinal) >= 0 ||
                    os.IndexOf("debian", StringComparison.Ordinal) >= 0)
                {
                    result = OperatingSystems.Aptitude;
                }
            }
            else
            {
                var ws = new WindowsScanner(machineName, userName, password, null);
                if (!ws.Open())
                {
                    return result;
                }
                ws.ScanWmiClass("Win32_OperatingSystem", (new[] { new Field { Name = "Name", MapTo = "Name" } }).ToList());
                if (ws.Result.Count > 0)
                {
                    result = OperatingSystems.Windows;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns indexes for each occurence of subString in sourceString
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="subString"></param>
        /// <returns></returns>
        public static IEnumerable<int> IndexOfAll(string sourceString, string subString)
        {
            return Regex.Matches(sourceString, subString).Cast<Match>().Select(m => m.Index);
        }
        #endregion
    }
}
