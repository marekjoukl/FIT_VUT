using System;
using System.Net;

namespace IPK_proj1;

public abstract class ArgsParser
{
    public static void PrintHelp()
    {
        Console.WriteLine("Usage: ipk24chat-client -t <type> -s <server> -p <port> -d <timeout> -r <retransmissions> -h");
        Console.WriteLine("       <type>: Transport protocol used for connection (tcp/udp)");
        Console.WriteLine("       <server>: Server IP or hostname");
        Console.WriteLine("       <port>: Server port number");
        Console.WriteLine("       <timeout>: UDP confirmation timeout in ms [250]");
        Console.WriteLine("       <retransmissions>: Maximum number of UDP retransmissions [3]");
        Console.WriteLine("       -h: Print this help message");
    }

    public static void ParseArgs(IPAddress? ip, int port, string type)
    {
        if (ip == null || port == -1 || type == "")
        {
            Console.WriteLine("Error: Missing arguments");
            PrintHelp();
            Environment.Exit(-1);
        }
        else if (type is not ("tcp" or "udp"))
        {
            Console.WriteLine("Error: Invalid type");
            PrintHelp();
            Environment.Exit(-1);
        }
    }
}