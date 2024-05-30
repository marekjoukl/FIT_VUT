#include "ArgumentParser.h"

ArgumentParser::ArgumentParser(int argc, char* argv[], InputFlags& flags) {
    parse(argc, argv, flags);
};

void ArgumentParser::parse(int argc, char* argv[],  InputFlags& flags) {
    if (argc == 1) {
        flags.printInterfaces = true;
        return;
    } else if (argc == 2 && string(argv[1]) != "-i") {
        printf("ERROR: Invalid arguments\n");
        exit(EXIT_FAILURE);
    }

    for (int i = 1; i < argc; i++) {
        
        // Interface parameter handle
        if (strcmp(argv[i], "-i") == 0 || strcmp(argv[i], "--interface") == 0) {
            // Print all interfaces
            if (i == argc - 1) {
                flags.printInterfaces = true;
                return;
            // The interface parameter is specified but the interface name is missing
            } else if (argv[i+1][0] == '-') {
                printf("ERROR: Interface parameter passed without name specified\n");
                exit(EXIT_FAILURE);
            // The interface paramteter is correct -> extract the interface name
            } else {
                flags.interfaceName = argv[i+1];
                i++;
            }

        // Protocol parameter handle
        } else if (strcmp(argv[i], "--tcp") == 0 || strcmp(argv[i], "-t") == 0){
            flags.tcp = true;
        } else if (strcmp(argv[i], "--udp") == 0 || strcmp(argv[i], "-u") == 0){
            flags.udp = true;
                
        // Port parameters handle
        } else if (strcmp(argv[i], "-p") == 0) {
            // If the port parameter is specified but the port number is missing
            if (argv[i+1][0] == '-') {
                printf("ERROR: Port parameter passed without port specified\n");
                exit(EXIT_FAILURE);
            } else {
                if (atoi(argv[i+1]) < 0 || atoi(argv[i+1]) > 65535) {
                    printf("ERROR: Invalid port number\n");
                    exit(EXIT_FAILURE);
                } else {
                    flags.port = atoi(argv[i+1]);
                    i++;
                }
            }                
        } else if (strcmp(argv[i], "--port-destination") == 0) {
            // If the port parameter is specified but the port number is missing
            if (argv[i+1][0] == '-') {
                printf("ERROR: Port parameter passed without port specified\n");
                exit(EXIT_FAILURE);
            } else {
                if (atoi(argv[i+1]) < 0 || atoi(argv[i+1]) > 65535) {
                    printf("ERROR: Invalid port number\n");
                    exit(EXIT_FAILURE);
                } else {
                    flags.destinationPort = atoi(argv[i+1]);
                    i++;
                }
            }                
        } else if (strcmp(argv[i], "--port-source") == 0) {
            // If the port parameter is specified but the port number is missing
            if (argv[i+1][0] == '-') {
                printf("ERROR: Port parameter passed without port specified\n");
                exit(EXIT_FAILURE);
            } else {
                if (atoi(argv[i+1]) < 0 || atoi(argv[i+1]) > 65535) {
                    printf("ERROR: Invalid port number\n");
                    exit(EXIT_FAILURE);
                } else {
                    flags.sourcePort = atoi(argv[i+1]);
                    i++;
                }
            }                
        } else if (strcmp(argv[i], "-n") == 0){
            // If the number of packets parameter is specified but the number is missing
            if (argv[i+1][0] == '-') {
                printf("ERROR: Number of packets parameter passed without number specified\n");
                exit(EXIT_FAILURE);
            } else {
                // If the number of packets is negative
                if (atoi(argv[i+1]) < 0) {
                    printf("ERROR: Invalid number of packets\n");
                    exit(EXIT_FAILURE);
                } else {
                    flags.numPackets = atoi(argv[i+1]);
                    i++;
                }
            }
        }
        else if (strcmp(argv[i], "--arp") == 0) {
            flags.arp = true;
        } else if (strcmp(argv[i], "--icmp4") == 0) {
            flags.icmp4 = true;
        } else if (strcmp(argv[i], "--icmp6") == 0) {
            flags.icmp6 = true;
        } else if (strcmp(argv[i], "--ndp") == 0) {
            flags.ndp = true;
        } else if (strcmp(argv[i], "--igmp") == 0) {
            flags.igmp = true;
        } else if (strcmp(argv[i], "--mld") == 0) {
            flags.mld = true;
        } else {
            printf("ERROR: Invalid arguments\n");
            exit(EXIT_FAILURE);
        
        }
    }
    if (!flags.tcp && !flags.udp && !flags.icmp4 && !flags.icmp6 && !flags.arp) {
        flags.tcp = true;
        flags.udp = true;
        flags.icmp4 = true;
        flags.icmp6 = true;
        flags.arp = true;
    }
}
