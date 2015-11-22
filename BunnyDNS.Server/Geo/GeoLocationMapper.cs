using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using MaxMind.GeoIP2;

namespace BunnyDNS.Server.Geo
{
    class GeoLocationMapper
    {
        /// <summary>
        /// The MaxMinds geo location reader used for mapping the IP to a city location
        /// </summary>
        private static DatabaseReader _maxMindsDatabaseReader = new DatabaseReader("MaxMinds_DB.mmdb", MaxMind.Db.FileAccessMode.Memory);


        /// <summary>
        /// Map the given IP as a location
        /// </summary>
        /// <returns></returns>
        public static GeoLocation MapIP(IPAddress ip)
        {
            try
            {
                var maxMindResult = _maxMindsDatabaseReader.City(ip);

                // Make sure we don't pass null values as the results are returned as a 'double?'
                var longitude = maxMindResult.Location.Longitude.HasValue ? maxMindResult.Location.Longitude.Value : 0;
                var latitude = maxMindResult.Location.Latitude.HasValue ? maxMindResult.Location.Latitude.Value : 0;

                return new GeoLocation(maxMindResult.Country.IsoCode, longitude, latitude);
            }
            catch {
                return new GeoLocation("", 0, 0);
            }
        }
    }
}
