using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyDNS.Server
{
    /// <summary>
    /// A list of supported EDNS options
    /// </summary>
    public enum EdnsOptionCode
    {
        /// <summary>
        /// EDNS0 client subnet option
        /// </summary>
        ClientSubnet = 8
    }
}
