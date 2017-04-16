namespace MachineScanner
{
    class PkgScanner : NixScanner
    {
        #region ctor
        public PkgScanner()
        {
            Command = "pkg_info";
        }
        #endregion
    }
}
