using System.Collections.Generic;

namespace Provisioning
{
    public interface IScanner
    {
        List<string> GetComputers();
    }
}
