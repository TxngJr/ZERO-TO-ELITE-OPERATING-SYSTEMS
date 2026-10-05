CC := gcc
CFLAGS := -Wall -Wextra -Wpedantic -std=c17
BIN := bin
PROGRAMS := address-demo address-split

.PHONY: all clean

all: $(PROGRAMS:%=$(BIN)/%)

$(BIN):
	mkdir -p $(BIN)

$(BIN)/%: examples/%.c | $(BIN)
	$(CC) $(CFLAGS) $< -o $@

clean:
	rm -rf $(BIN)
