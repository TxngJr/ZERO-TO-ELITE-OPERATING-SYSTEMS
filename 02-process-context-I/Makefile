CC := gcc
CFLAGS := -Wall -Wextra -Wpedantic -std=c17
BIN := bin

.PHONY: all clean

all: $(BIN)/memory-layout $(BIN)/process-info

$(BIN):
	mkdir -p $(BIN)

$(BIN)/memory-layout: examples/memory-layout.c | $(BIN)
	$(CC) $(CFLAGS) $< -o $@

$(BIN)/process-info: examples/process-info.c | $(BIN)
	$(CC) $(CFLAGS) $< -o $@

clean:
	rm -rf $(BIN)
