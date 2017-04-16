using System.Collections.Generic;
using Configuration;

namespace MachineScanner
{
    public class ScannerFactory
    {
        public static Dictionary<OperatingSystems, IScanner>
            Scanners = new Dictionary<OperatingSystems, IScanner>();
        public static IScanner GetScanner(OperatingSystems operatingSystem)
        {
            if(Scanners.Count == 0)
            {
                Scanners.Add(OperatingSystems.Windows, new WindowsScanner());
                Scanners.Add(OperatingSystems.Aptitude, new AptitudeScanner());
                Scanners.Add(OperatingSystems.Cygwin, new CygwinScanner());
                Scanners.Add(OperatingSystems.Pacman, new PacmanScanner());
                Scanners.Add(OperatingSystems.Pkg, new PkgScanner());
                Scanners.Add(OperatingSystems.Portage, new PortageScanner());
                Scanners.Add(OperatingSystems.Rpm, new RpmScanner());
                Scanners.Add(OperatingSystems.Slackware, new SlackwareScanner());
            }
            switch (operatingSystem)
            {
                case OperatingSystems.Windows:
                    return new WindowsScanner();
                case OperatingSystems.Aptitude:
                    return new AptitudeScanner();
                case OperatingSystems.Cygwin:
                    return new CygwinScanner();
                case OperatingSystems.Pacman:
                    return new PacmanScanner();
                case OperatingSystems.Pkg:
                    return new PkgScanner();
                case OperatingSystems.Portage:
                    return new PortageScanner();
                case OperatingSystems.Rpm:
                    return new RpmScanner();
                case OperatingSystems.Slackware:
                    return new SlackwareScanner();
            }

            return null;// scanners[operatingSystem];
        }
    }
}