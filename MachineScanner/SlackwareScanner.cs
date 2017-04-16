namespace MachineScanner
{
    class SlackwareScanner : NixScanner
    {
        #region ctor
        public SlackwareScanner()
        {
            Command = "slapt-get --installed";
        }
        #endregion
    }
}
