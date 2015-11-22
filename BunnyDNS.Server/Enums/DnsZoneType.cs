using System;

namespace BunnyDNS.Server
{
    /// <summary>
    /// The list of DNS zone types
    /// </summary>
	public enum DnsZoneType
	{
		A = 1,
        AAAA = 28,
        NS = 2,
		CNAME = 5,
		SOA = 6,
		WKS = 11,
		PTR = 12,
		MX = 15,
        TXT = 16,
		SRV = 33,
		A6 = 38,
        OPT = 41,
        SPF = 99,
		ANY = 255
	}
}

