namespace MachineScanner
{
    public interface IPrivateKeyAuth
    {
        void SetPrivateKeyFileName(string privateKeyFileName);
        void SetPassPhrase(string passPhrase);
    }
}
