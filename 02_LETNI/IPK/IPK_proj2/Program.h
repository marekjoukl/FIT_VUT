#ifndef PROGRAM_H
#define PROGRAM_H

#include "ipk-sniffer.h"

class Program {
private:
    InputFlags flags;

public:
    Program(int argc, char *argv[]);
    std::string createFilters(const InputFlags& flags);
    void run();
    static void packetPrinter (u_char *args, struct pcap_pkthdr *header, const u_char *frame);
    static void printHexDump(const u_char *frame, int len);
    static void printFrameAscii(const u_char *packet, int start, int end);
    void printInterfaces();
    static void printTime(const struct pcap_pkthdr *header);
    static void ipv6PacketPrinter(const u_char *frame);
    static void ipv4PacketPrinter(const u_char *frame);
    static void arpPacketPrinter(const u_char *frame);
    static void tcpPacketPrinter(const u_char *frame, int headerLength);
    static void udpPacketPrinter(const u_char *frame, int headerLength);
    static void icmpPacketPrinter(const u_char *frame, int headerLength);
};


#endif // PROGRAM_H