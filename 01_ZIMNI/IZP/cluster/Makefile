CC = gcc
CFLAGS = -std=c99 -Wall -Wextra -Werror -DNDEBUG
LIBS = -lm
TARGET = cluster

all: $(TARGET)

$(TARGET): cluster.c
	$(CC) $(CFLAGS) $^ -o $@ $(LIBS)

clean:
	rm -f $(TARGET)
