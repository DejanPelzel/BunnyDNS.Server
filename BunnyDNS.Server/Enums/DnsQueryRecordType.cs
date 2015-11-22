using System;

namespace BunnyDNS.Server
{
	/// <summary>
	/// A list of possible DnsQuery request types
	/// </summary>
	public enum DnsQueryRequestType
	{
		/// <summary>
		/// A normal query
		/// </summary>
		Query,

		/// <summary>
		/// Inversed query. Optional.
		/// </summary>
		InverseQuery,

		/// <summary>
		/// DNS status request
		/// </summary>
		Status
	}
}

