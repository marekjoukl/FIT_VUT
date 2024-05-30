
using System;
using System.Net;

namespace IPK_proj1;

abstract class Program
{
    static void Main(string[] args)
    {
        string ipOrHostname = "";
        int port = -1;
        string type = "";
        IPAddress? ipAddress = null;
        UInt16 timeout = 0;
        int retryCount = 0;

        if (args.Length > 0){
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-h")
                {
                    ArgsParser.PrintHelp();
                    Environment.Exit(0);
                }
                else if(args[i] == "-t") type = args[i + 1];
                else if(args[i] == "-p") port = int.Parse(args[i + 1]);
                else if(args[i] == "-s") ipOrHostname = args[i + 1];
                else if(args[i] == "-d") timeout = UInt16.Parse(args[i + 1]);
                else if(args[i] == "-r") retryCount = int.Parse(args[i + 1]);
                else continue;
            }
            try
            {
                // Try to parse the input as an IP address
                ipAddress = IPAddress.Parse(ipOrHostname);
            }
            catch (FormatException)
            {
                // If it's not an IP address, try to resolve it as a hostname
                try
                {
                    IPHostEntry hostEntry = Dns.GetHostEntry(ipOrHostname);
                    ipAddress = hostEntry.AddressList[0];
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: Unable to resolve hostname {ipOrHostname}: {ex.Message}");
                    Environment.Exit(-1);
                }
            }
            ArgsParser.ParseArgs(ipAddress, port, type);
        }
        else
        {
            ArgsParser.PrintHelp();
            Environment.Exit(-1);
        }

        if (type == "tcp")
        {
            TcpClientHandler tcpClientHandler = new TcpClientHandler();
            tcpClientHandler.OpenTcpConnection(ipAddress, port).Wait();
        }
        else if (type == "udp")
        {
            UdpClientHandler udpClientHandler = new UdpClientHandler();
            udpClientHandler.OpenUdpConnection(ipAddress, port, retryCount, timeout).Wait();
        }
        else
        {
            Console.WriteLine("Error: Invalid type");
            ArgsParser.PrintHelp();
            Environment.Exit(-1);
        }
    }
}