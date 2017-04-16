namespace MachineScanner
{
    public class RpmScanner : NixScanner
    {
        #region ctor
        public RpmScanner()
        {
            Command = "rpm -qa";
        }
        #endregion
        public override object LineToObject(string line)
        {
            return new { name = line };
        }
    }
}
