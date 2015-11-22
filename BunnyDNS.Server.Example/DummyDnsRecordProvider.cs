using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BunnyDNS.Server.RequestProcessor;
using System.Net;

namespace BunnyDNS.Server.Example
{
    public class DummyDnsRecordProvider : DnsRecordProvider
    {
        /// <summary>
        /// Provide dummy answers for DNS questions
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public override void HandleQuestion(DnsQuestion question, ref List<DnsAnswer> answerList)
        {
            //query.GeoLocation.ToString();
            if (question.ZoneType == DnsZoneType.SOA || question.ZoneType == DnsZoneType.ANY)
            {
                //Console.WriteLine("SOA RECORD");
                answerList.Add(new DnsAnswer(question)
                {
                    Record = new SoaDnsRecord(900, "ns1.bunnydns.com", "hostmaster.bunnydns.com", 1, 7200, 3600, 1209600, 3600),
                });
            }
            if (question.ZoneType == DnsZoneType.A || question.ZoneType == DnsZoneType.ANY)
            {
                //Console.WriteLine("A RECORD");
                answerList.Add(new DnsAnswer(question)
                {
                    Record = new ADnsRecord(30, IPAddress.Parse("23.249.224.31")),
                });
            }
            if (question.ZoneType == DnsZoneType.MX || question.ZoneType == DnsZoneType.ANY)
            {
                //Console.WriteLine("MX RECORD");
                answerList.Add(new DnsAnswer(question)
                {
                    Record = new MxRecord(30, 1, "ASPMX.L.GOOGLE.com"),
                });
                answerList.Add(new DnsAnswer(question)
                {
                    Record = new MxRecord(30, 5, "ALT1.ASPMX.L.GOOGLE.com"),
                });
            }


            // Only do this if NS are requested
            if (question.ZoneType == DnsZoneType.NS || question.ZoneType == DnsZoneType.ANY)
            {
                //Console.WriteLine("NS RECORD");
                answerList.Add(new DnsAnswer(question)
                {
                    Record = new NsDnsRecord(3600, "ns1.bunnydns.com")

                });
                answerList.Add(new DnsAnswer(question)
                {
                    Record = new NsDnsRecord(3600, "ns2.bunnydns.com")
                });
            }
            else if (answerList.Count > 0)
            {
                //Console.WriteLine("AUTHORITY NS RECORD");
                answerList.Add(new DnsAnswer(question)
                {
                    Record = new NsDnsRecord(3600, "ns1.bunnydns.com")
                    {
                        IsAuthorityRecord = true,
                    }
                });
                answerList.Add(new DnsAnswer(question)
                {
                    Record = new NsDnsRecord(3600, "ns2.bunnydns.com")
                    {
                        IsAuthorityRecord = true,
                    }
                });
            }
        }
    }
}
