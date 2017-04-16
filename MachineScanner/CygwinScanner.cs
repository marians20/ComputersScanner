using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger;
using SshHelper;

namespace MachineScanner
{
    class CygwinScanner : NixScanner, IScanner
    {
        #region ctor
        public CygwinScanner()
            :base()
        {
            Command = "cygcheck --check-setup --dump-only *";
        }
        #endregion
    }
}
