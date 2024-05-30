#ifndef ARGUMENT_PARSER_H
#define ARGUMENT_PARSER_H

#include "ipk-sniffer.h"

class ArgumentParser {
public:
    ArgumentParser(int argc, char* argv[], InputFlags& flags);
    void parse(int argc, char* argv[], InputFlags& flags);
};


#endif // ARGUMENT_PARSER_H