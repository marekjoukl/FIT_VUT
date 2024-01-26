CC = gcc
CFLAGS = -std=c99 -Wall -Wextra -Werror
TARGET = t9search

all: $(TARGET)

$(TARGET): t9search.c
	$(CC) $(CFLAGS) $^ -o $@

clean:
	rm -f $(TARGET)
