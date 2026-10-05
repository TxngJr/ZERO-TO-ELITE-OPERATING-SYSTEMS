CC := gcc
CFLAGS := -Wall -Wextra -Wpedantic -std=c17
THREAD_FLAGS := -pthread
BIN := bin
PROGRAMS := check-then-act release-acquire detached-demo

.PHONY: all clean

all: $(PROGRAMS:%=$(BIN)/%)

$(BIN):
	mkdir -p $(BIN)

$(BIN)/%: examples/%.c | $(BIN)
	$(CC) $(CFLAGS) $< -o $@ $(THREAD_FLAGS)

clean:
	rm -rf $(BIN)
