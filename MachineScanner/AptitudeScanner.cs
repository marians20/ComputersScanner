using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger;
using System.IO;

namespace MachineScanner
{
    public class AptitudeScanner : NixScanner, IScanner
    {
        #region ctor
        public AptitudeScanner()
            :base()
        {
            Command = "dpkg -l";
        }
        #endregion

        public override int Scan()
        {

            #region base.base.Scan()
            OnRaiseStart(new ScannerEventArgs(Machine));
            Result.Clear();
            Result.Add("Scanning",
                (new object[] { new
                {
                    machine = Machine,
                    time = DateTime.Now
                } }).ToList());
            #endregion

            if (!Open())
            {
                return -1;
            }

            var totalLines = 0;

            foreach (var kpi in Kpis)
            {
                var lines = Ssh.Exec(kpi.Command).Split(Environment.NewLine.ToCharArray())
                    .Select(i => (i as string).Trim()).Where(i => !string.IsNullOrEmpty(i));

                var indices = new List<int>();
                var canRead = false;

                var items = new List<object>();
                foreach(var line in lines)
                {
                    if (!canRead)
                    {
                        if (string.Compare(line.Substring(0, 3), "+++") == 0)
                        {
                            indices = IndexOfAll(line, "-").ToList();
                            canRead = true;
                        }
                    }
                    else
                    {
                        items.Add(new
                        {
                            Name = line.Substring(indices[0], indices[1]-indices[0]).Trim(),
                            Version = line.Substring(indices[1], indices[2] - indices[1]).Trim(),
                            Architecture = line.Substring(indices[2], indices[3] - indices[2]).Trim(),
                            Description = line.Substring(indices[3]).Trim(),
                        });
                    }
                }

                totalLines += lines.Count();

                Result.Add(kpi.Name, items);
            }

            if (Persistence != null)
            {
                Persistence.Save(Machine, GetResult());
            }

            return totalLines;
        }

        public override object LineToObject(string line)
        {
            return new { line };
        }
    }
}
