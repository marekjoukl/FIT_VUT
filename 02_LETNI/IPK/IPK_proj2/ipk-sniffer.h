#ifndef IPK_SNIFFER_H
#define IPK_SNIFFER_H

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <pcap.h>
#include <sys/socket.h>
#include <stdbool.h>
#include <unistd.h>
#include <vector>
#include <string>
#include <net/ethernet.h>
#include <netinet/ip.h>
#include <netinet/tcp.h>
#include <netinet/udp.h>
#include <netinet/ip6.h>
#include <netinet/icmp6.h>
#include <arpa/inet.h>
#include <iomanip>
#include <sstream>
#include <iostream>

using namespace std;

struct ArpHeader {
    u_int16_t htype;
    u_int16_t ptype;
    u_char hlen;
    u_char plen;
    u_int16_t operation;
    u_char sha[6];
    u_char spa[4];
    u_char tha[6];
    u_char tpa[4];
};

struct InputFlags {
    bool printInterfaces = false;
    bool tcp = false;
    bool udp = false;
    bool arp = false;
    bool icmp4 = false;
    bool icmp6 = false;
    bool ndp = false;
    bool igmp = false;
    bool mld = false;
    int port = -1;
    int sourcePort = -1;
    int destinationPort = -1;
    int numPackets = -1;
    string interfaceName = "";
};

#endif //IPK_SNIFFER_H