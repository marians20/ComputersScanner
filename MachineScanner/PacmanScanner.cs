namespace MachineScanner
{
    class PacmanScanner : NixScanner
    {
        #region ctor
        public PacmanScanner()
        {
            Command = "pacman -Q";
        }
        #endregion
    }
}
