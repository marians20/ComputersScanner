using System.Collections.Generic;
using System.DirectoryServices;
using Logger;

namespace Provisioning
{
    public class ActiveDirectoryScanner : IScanner
    {
        public string Domain { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public List<string> GetComputers()
        {
            Log.Debug("Start scanning domain {0} for computers.", Domain);
            var computerNames = new List<string>();

            var entry = new DirectoryEntry($"LDAP://{Domain}");
            var mySearcher = new DirectorySearcher(entry)
            {
                Filter = ("(objectClass=computer)"),
                SizeLimit = int.MaxValue,
                PageSize = int.MaxValue
            };

            foreach (SearchResult resEnt in mySearcher.FindAll())
            {
                var computerName = resEnt.GetDirectoryEntry().Name;
                if (computerName.StartsWith("CN="))
                    computerName = computerName.Remove(0, "CN=".Length);
                computerNames.Add(computerName);
            }

            mySearcher.Dispose();
            entry.Dispose();
            Log.Debug("Found {0} computers in domain {1}", computerNames.Count, Domain);
            return computerNames;
        }
    }
}
