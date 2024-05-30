#include "ipk-sniffer.h"
#include "ArgumentParser.h"
#include "Program.h"

int main (int argc, char *argv[]) {
    Program program(argc, argv);
    program.run();

    return 0;
}