using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace BunnyDNS.Server.RequestProcessor
{
    public class DnsRequestProcessor
    {
        /// <summary>
        /// Process the UDP result
        /// </summary>
        /// <param name="receivedData"></param>
        /// <returns></returns>
        public static int ProcessReceivedData(DnsRequestListenerSession request)
        {
            // Parse the query
            DnsQuery query;
            try
            {
                query = DnsQuery.Parse(request.Buffer, ((IPEndPoint)request.SocketAsyncEventArgs.RemoteEndPoint).Address);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                // If parsing fails, there isn't much we can do
                return 0;
            }

            try
            {
                var answers = new List<DnsAnswer>();
                foreach (var question in query.Questions)
                {
                    // Request answers for this question
                    request.DnsRequestListener.DnsRecordProvider.HandleQuestion(question, ref answers);
                }

                // Create the response
                return new DnsResponse(query, answers).GetData(request.Buffer);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                // Proceed to returning a sever failure
            }


            // if we end up here, return server failure
            return GetErrorResponse(query, DnsQueryStatusCode.ServerFailure).GetData(request.Buffer);
        }

        /// <summary>
        /// Get an empty error response with the given status code
        /// </summary>
        /// <param name="query"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        private static DnsResponse GetErrorResponse(DnsQuery query, DnsQueryStatusCode statusCode)
        {
            var response = new DnsResponse(query, null);
            response.StatusCode = statusCode;
            return response;
        }
    }
}
