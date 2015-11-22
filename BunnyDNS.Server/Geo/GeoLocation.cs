using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyDNS.Server.Geo
{
    /// <summary>
    /// GeoLocation contains 
    /// </summary>
    public class GeoLocation
    {
        /// <summary>
        /// Gets the latitude of the location
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets the latitude of the location
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The ISO code of the country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Create a new geo location object
        /// </summary>
        public GeoLocation(string countryIsoCode, double longitude, double latitude)
        {
            this.Country = countryIsoCode;
            this.Longitude = longitude;
            this.Latitude = latitude;
        }

        /// <summary>
        /// Get a textual representation of the GeoLocation object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("GeoLocation Country: {0}, Latitude: {1}, Longitude: {2}", this.Country, this.Latitude, this.Longitude);
        }
    }
}
