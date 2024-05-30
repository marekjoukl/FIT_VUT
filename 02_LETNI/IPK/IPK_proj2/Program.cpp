#include "Program.h"
#include "ArgumentParser.h"

Program::Program(int argc, char *argv[]) {
    ArgumentParser parser(argc, argv, flags);
}

std::string Program::createFilters(const InputFlags& flags) {
    string filter = "";
    if (flags.port != -1) {
        if (flags.tcp) {
            filter += "tcp port " + to_string(flags.port) + " or ";
        }
        if (flags.udp) {
            filter += "udp port " + to_string(flags.port) + " or ";
        }
        if (flags.icmp4) {
            filter += "icmp or ";
        }
        if (flags.icmp6) {
            filter += "icmp6 or ";
        }
        if (flags.arp) {
            filter += "arp or ";
        }
        if (flags.ndp) {
            filter += "(icmp6 and icmp6[0] >= 133 and icmp6 and icmp6[0] <= 137) or ";
        }
        if (flags.igmp) {
            filter += "igmp or ";
        }
        if (flags.mld) {
            filter += "(icmp6 and icmp6[0] >= 130 and icmp6 and icmp6[0] <= 132) or ";
        }
    } else {
        if (flags.tcp) {
            filter += "tcp or ";
        }
        if (flags.udp) {
            filter += "udp or ";
        }
        if (flags.icmp4) {
            filter += "icmp or ";
        }
        if (flags.icmp6) {
            filter += "icmp6 or ";
        }
        if (flags.arp) {
            filter += "arp or ";
        }
        if (flags.ndp) {
            filter += "(icmp6 and icmp6[0] >= 133 and icmp6 and icmp6[0] <= 137) or ";
        }
        if (flags.igmp) {
            filter += "igmp or ";
        }
        if (flags.mld) {
            filter += "(icmp6 and icmp6[0] >= 130 and icmp6 and icmp6[0] <= 132) or ";
        }
    }
    filter = filter.substr(0, filter.size() - 4);
    return filter; 
}

void Program::run() {
    if (flags.printInterfaces) Program::printInterfaces();
    char errbuff[PCAP_ERRBUF_SIZE];
    struct bpf_program fp;
    string filter = createFilters(flags);
    
    pcap_if_t *interfaces;
    if (pcap_findalldevs(&interfaces, errbuff) == -1) {
        std::cerr << "Error finding network interfaces: " << errbuff << std::endl;
        exit(PCAP_ERROR);
    }
    
    const char *dev = nullptr;

    // Loop through the list of interfaces to find a match with flags.interfaceName
    for (pcap_if_t *iface = interfaces; iface != nullptr; iface = iface->next) {
        if (iface->name && strcmp(iface->name, flags.interfaceName.c_str()) == 0) {
            dev = iface->name;
            break;
        }
    }
    
    if (!dev) {
        std::cerr << "Error: Interface not found: " << flags.interfaceName << std::endl;
        exit(PCAP_ERROR);
    }

    bpf_u_int32 net, mask;
    if (pcap_lookupnet(dev, &net, &mask, errbuff) == PCAP_ERROR) {
        cerr << "Error looking up network: " << errbuff << std::endl;
        exit(PCAP_ERROR);
    }

    // open sniffing session
    pcap_t *handle = pcap_open_live(dev, BUFSIZ, 1, 1000, errbuff);
    if (!handle) {
        fprintf(stderr, "Error opening device %s: %s\n", dev, errbuff);
        exit(PCAP_ERROR);
    }
    
    // Check ethernet support
    if(pcap_datalink(handle) != DLT_EN10MB) {
        cout << "Ethernet is not supported on specified interface" << endl;
        exit(PCAP_ERROR);
    }

    // Compile and set the filter
    if (mask){
        if (pcap_compile(handle, &fp, filter.c_str(), 0, mask) == -1) {
            cerr << "Error compiling filter: " << pcap_geterr(handle) << endl;
            exit(PCAP_ERROR);
        }

        if (pcap_setfilter(handle, &fp) == -1) {
            cerr << "Error setting filter: " << pcap_geterr(handle) << endl;
            exit(PCAP_ERROR);
        }
    }

    pcap_loop(handle, flags.numPackets, (pcap_handler)&Program::packetPrinter, NULL);

    pcap_freealldevs(interfaces);
    pcap_close(handle);
}

void Program::packetPrinter (u_char *args, struct pcap_pkthdr *header, const u_char *frame){
    struct ether_header *eth = (struct ether_header *)(frame);
    
    Program::printTime(header);
    cout << "src MAC: ";
    for (int i = 0; i < 6; i++) {
        printf("%02x", eth->ether_shost[i]);
        if (i < 5) {
            cout << ":";
        }
    }
    cout << endl;

    cout << "dst MAC: ";
    for (int i = 0; i < 6; i++) {
        printf("%02x", eth->ether_dhost[i]);
        if (i < 5) {
            cout << ":";
        }
    }
    cout << endl;
    
    cout << "frame length: " << std::dec << header->len << " bytes" << endl;

    switch (ntohs(eth->ether_type)) {
        case ETHERTYPE_IPV6:
            Program::ipv6PacketPrinter(frame);
            break;
        case ETHERTYPE_IP:
            Program::ipv4PacketPrinter(frame);
            break;
        case ETHERTYPE_ARP:
            Program::arpPacketPrinter(frame);
            break;
        default:
            cout << "Unsupported protocol" << endl;
            break;
    }
    cout << endl;

    Program::printHexDump(frame, header->len);
    cout << endl;
    std::cout << std::flush;
}

void Program::printHexDump(const u_char *frame, int len) {
    int i;
    for (i = 0; i < len; i++) {
        int position = i % 16; 
        if (position == 0)
            cout << "0x" << setfill('0') << setw(4) << hex << i << " ";

        cout << setfill('0') << setw(2) << hex << static_cast<int>(frame[i]) << " ";
        if (i % 16 == 7)
            cout << " ";

        if (position == 15 || i == len - 1) {
            for (int j = 0; j < 15 - position; j++) cout << "   ";
            if (position < 7) cout << " ";

            Program::printFrameAscii(frame, i - position, i);
            cout << endl;
        }
    }
}

void Program::printFrameAscii(const u_char *packet, int start, int end) {
    for (int i = start; i <= end; i++) {
        if (packet[i] >= 33 && packet[i] <= 126)
            cout << static_cast<char>(packet[i]);
        else
            cout << ".";

        if (i % 16 == 7)
            cout << " ";
    }
}

void Program::printInterfaces() {
    pcap_if_t *interfaces;
    char errbuff[PCAP_ERRBUF_SIZE];
    if (pcap_findalldevs(&interfaces, errbuff) == -1) {
        std::cerr << "Error finding network interfaces: " << errbuff << std::endl;
        exit(PCAP_ERROR);
    }

    pcap_if_t *interface = interfaces;
    while (interface) {
        cout << interface->name << endl;
        interface = interface->next;
    }
    pcap_freealldevs(interfaces);
    exit(EXIT_SUCCESS);
}

void Program::printTime(const struct pcap_pkthdr *header) {
    struct tm time_data;
    localtime_r(&(header->ts.tv_sec), &time_data);

    char datetime[20];
    strftime(datetime, sizeof(datetime), "%FT%T", &time_data);
    
    char time_zone[10];
    strftime(time_zone, sizeof(time_zone), "%z", &time_data);

    auto mili_secs = header->ts.tv_usec / 1000;
    printf("timestamp: %s.%03ld%.3s:%.2s\n", datetime, mili_secs,
                                                time_zone, time_zone + 3);
}
void Program::ipv6PacketPrinter(const u_char *frame) {
    struct ip6_hdr *ipv6Header = (struct ip6_hdr *)(frame + sizeof(struct ether_header));
    char ipv6Addr[INET6_ADDRSTRLEN];

    inet_ntop(AF_INET6, &(ipv6Header->ip6_src), ipv6Addr, INET6_ADDRSTRLEN);
    cout << "src IP: " << ipv6Addr << endl;
    inet_ntop(AF_INET6, &(ipv6Header->ip6_dst), ipv6Addr, INET6_ADDRSTRLEN);
    cout << "dst IP: " << ipv6Addr << endl;

    int headerLength = sizeof(struct ip6_hdr);
    if (ipv6Header->ip6_nxt == IPPROTO_TCP) {
        Program::tcpPacketPrinter(frame, headerLength);
    } else if (ipv6Header->ip6_nxt == IPPROTO_UDP) {
        Program::udpPacketPrinter(frame, headerLength);
    } else if (ipv6Header->ip6_nxt == IPPROTO_ICMPV6) {
        struct icmp6_hdr *icmp6Header = (struct icmp6_hdr *)(frame + sizeof(struct ether_header) + headerLength);
        Program::icmpPacketPrinter(frame, headerLength);
    } else {
        cout << "Unsupported protocol" << endl;
    }
}

void Program::ipv4PacketPrinter(const u_char *frame) {
    struct ip *ipv4Header = (struct ip *)(frame + sizeof(struct ether_header));

    cout << "src IP: " << inet_ntoa(ipv4Header->ip_src) << endl;
    cout << "dst IP: " << inet_ntoa(ipv4Header->ip_dst) << endl;

    int headerLength = ipv4Header->ip_hl * 4;
    if (ipv4Header->ip_p == IPPROTO_TCP) {
        Program::tcpPacketPrinter(frame, headerLength);
    } else if (ipv4Header->ip_p == IPPROTO_UDP) {
        Program::udpPacketPrinter(frame, headerLength);
    } else if (ipv4Header->ip_p == IPPROTO_ICMP) {
        Program::icmpPacketPrinter(frame, headerLength);
    } else {
        cout << "Unsupported protocol" << endl;
    }

};

void Program::arpPacketPrinter(const u_char *frame) {
    auto *arpHeader = (struct ArpHeader *)(frame + sizeof(struct ether_header));
    cout << "sha MAC: ";
    for (int i = 0; i < 6; i++) {
        printf("%02x", arpHeader->sha[i]);
        if (i < 5) {
            cout << ":";
        }
    }
    cout << endl;
    cout << "tha MAC: ";
    for (int i = 0; i < 6; i++) {
        printf("%02x", arpHeader->tha[i]);
        if (i < 5) {
            cout << ":";
        }
    }
    cout << endl;
    cout << "spa IP: " << inet_ntoa(*(in_addr *)&arpHeader->spa) << endl;
    cout << "tpa IP: " << inet_ntoa(*(in_addr *)&arpHeader->tpa) << endl;
}

void Program::tcpPacketPrinter(const u_char *frame, int headerLength) {
    struct tcphdr *tcpHeader = (struct tcphdr *)(frame + sizeof(struct ether_header) + headerLength);
    cout << "src port: " << std::dec << ntohs(tcpHeader->th_sport) << endl;
    cout << "dst port: " << std::dec << ntohs(tcpHeader->th_dport) << endl;
}
void Program::udpPacketPrinter(const u_char *frame, int headerLength) {
    struct tcphdr *udpHeader = (struct tcphdr *)(frame + sizeof(struct ether_header) + headerLength);
    cout << "src port: " << std::dec << ntohs(udpHeader->th_sport) << endl;
    cout << "dst port: " << std::dec << ntohs(udpHeader->th_dport) << endl;
}
void Program::icmpPacketPrinter(const u_char *frame, int headerLength) {
    struct tcphdr *icmpHeader = (struct tcphdr *)(frame + sizeof(struct ether_header) + headerLength);
}


