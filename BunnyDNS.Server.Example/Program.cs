using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BunnyDNS.Server;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using BunnyDNS.Server.RequestProcessor;

namespace BunnyDNS.Server.Example
{


    class Program
    {
        static void Main(string[] args)
        {
            var dnsServer = new DnsServer(new DummyDnsRecordProvider());
            dnsServer.Start();
            


            var data = System.IO.File.ReadAllBytes("request.dat");
            for (int i = 0; i < 4; i++)
            {
                Task.Run(async () =>
                {
                    var udpClient = new UdpClient("127.0.0.1", 53);
                    while (true)
                    {
                        try
                        {
                            await udpClient.SendAsync(data, data.Length);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("BANANA: " + e.Message);
                        }
                    }


                    /*while (true)
                    {
                        try
                        {
                            while (true)
                            {
                                var tcpClient = new TcpClient("127.0.0.1", 53);
                                var stream = tcpClient.GetStream();

                                await stream.WriteAsync(data, 0, data.Length);
                                await stream.FlushAsync();

                                tcpClient.Close();
                            }

                        }
                        catch { }
                    }*/
                });
            }

            // Stats
            Task.Run(() =>
            {
                while (true)
                {
                    Console.WriteLine("Received: " + UdpDnsRequestListener.packetsReceived);
                    Console.WriteLine("MB recei: " + (UdpDnsRequestListener.bytesReceived / 1048576));
                    UdpDnsRequestListener.packetsReceived = 0;
                    UdpDnsRequestListener.bytesReceived = 0;
                    Thread.Sleep(1000);
                }
            });

            Console.ReadLine();
        }
    }
}
