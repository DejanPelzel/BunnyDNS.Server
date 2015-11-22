using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyDNS.Server.RequestProcessor
{
    /// <summary>
    /// DnsRecordProvider is used to provide answer records for the received answers
    /// </summary>
    public abstract class DnsRecordProvider
    {
        /// <summary>
        /// Returns a list of DnsAnswer objects that will be returned for the given question
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public abstract void HandleQuestion(DnsQuestion question, ref List<DnsAnswer> answerList);
    }
}
