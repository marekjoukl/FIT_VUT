CC=gcc
CFLAGS=-std=gnu99 -g -Wall -Werror -Wextra -pedantic -pthread

all: proj2

proj2: proj2.o
	$(CC) $(CFLAGS) proj2.o -o proj2

proj2.o: proj2.c proj2.h
	$(CC) $(CFLAGS) -c proj2.c

clean:
	rm -f *.o proj2

run:
	./proj2 3 2 100 100 100 && cat proj2.out
	
zip:
	zip proj2.zip proj2.c proj2.h Makefile
