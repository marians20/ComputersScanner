using System;
using Newtonsoft.Json;
using System.Linq;

namespace MachineScanner
{
    public class NixScanner : GenericScanner
    {
        #region properties
        protected string Command { get; set; }
        #endregion

        #region ctor
        public NixScanner()
        {
            Command = String.Empty;
        }
        #endregion

        #region private properties
        protected SshHelper.SshHelper Ssh;
        //ConnectionInfo connectionInfo;

        #endregion

        #region Methods

        public override bool Open()
        {
            Ssh = new SshHelper.SshHelper()
            {
                Host = Machine,
                UserName = UserName,
                Password = Password,
                SudoPassword = Password,
                PrivateKeyFileName = PrivateKeyFileName
            };
            return Ssh.CheckConnected;
        }

        public override int Scan()
        {
            if(!Open())
            {
                return -1;
            }

            base.Scan();

            var totalLines = 0;

            foreach(var kpi in Kpis)
            {
                var lines = Ssh.Exec(kpi.Command).Split(Environment.NewLine.ToCharArray())
                    .Select(i => i.Trim()).Where(i => !string.IsNullOrEmpty(i))
                    .Select(LineToObject).ToList();

                totalLines += lines.Count;

                Result.Add(kpi.Name, lines);
            }

            if (Persistence != null)
            {
                Persistence.Save(Machine, GetResult());
            }
            return totalLines;
        }

        public override bool Close()
        {
            var result = true;
            return result;
        }

        public virtual object LineToObject(string line)
        {
            return new { line };
        }

        public override string GetResult()
        {
            return JsonConvert.SerializeObject(Result, Formatting.Indented);
        }
        #endregion
    }
}