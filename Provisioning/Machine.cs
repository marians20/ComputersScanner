namespace Provisioning
{
    public class Machine
    {
        public string Name { get; set; }

        /// <summary>
        /// Json wits scan results
        /// </summary>
        public string Data { get; set; }

        #region ctor
        public Machine()
        {
            Name = string.Empty;
            Data = string.Empty;
        }

        public Machine(string name)
            :this()
        {
            Name = name;
        }
        #endregion
    }
}
