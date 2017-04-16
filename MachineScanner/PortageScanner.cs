namespace MachineScanner
{
    class PortageScanner : NixScanner
    {
        #region ctor
        public PortageScanner()
        {
            Command = "equery list";
        }
        #endregion
    }
}
