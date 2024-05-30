CC=g++
CFLAGS=-std=c++20

all: clean ipk-sniffer

ipk-sniffer: ipk-sniffer.o ArgumentParser.o Program.o
	$(CC) $(CFLAGS) ipk-sniffer.o ArgumentParser.o Program.o -lpcap -o ipk-sniffer

ipk-sniffer.o: ipk-sniffer.cpp
	$(CC) $(CFLAGS) -c ipk-sniffer.cpp

ArgumentParser.o: ArgumentParser.cpp
	$(CC) $(CFLAGS) -c ArgumentParser.cpp

Program.o: Program.cpp
	$(CC) $(CFLAGS) -c Program.cpp

clean:
	rm -f ipk-sniffer *.o

pack: clean
	zip -r xjoukl00.zip .

