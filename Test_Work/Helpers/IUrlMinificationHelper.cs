using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test_Work.Helpers
{
    public interface IUrlMinificationHelper
    {
        string UrlMinificate(string longUrl, bool withSeed = true);

    }
}
