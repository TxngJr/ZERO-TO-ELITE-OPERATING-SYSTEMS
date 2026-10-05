CC := gcc
CFLAGS := -Wall -Wextra -Wpedantic -std=c17
THREAD_FLAGS := -pthread
BIN := bin
PROGRAMS := thread-basic lost-update thread-observe

.PHONY: all clean

all: $(PROGRAMS:%=$(BIN)/%)

$(BIN):
	mkdir -p $(BIN)

$(BIN)/%: examples/%.c | $(BIN)
	$(CC) $(CFLAGS) $< -o $@ $(THREAD_FLAGS)

clean:
	rm -rf $(BIN)
