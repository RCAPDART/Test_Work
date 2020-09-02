using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test_Work.Helpers
{
    // Do you really need to make it public? Why it is not internal?
    // Why it is helper? It looks like service. IUrlMinificatorService for example.
    public interface IUrlMinificationHelper
    {
        string UrlMinificate(string longUrl, bool withSeed = true);

    }
}
