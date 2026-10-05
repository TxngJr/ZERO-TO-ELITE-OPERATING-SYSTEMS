CC := gcc
CFLAGS := -Wall -Wextra -Wpedantic -std=c17
BIN := bin

PROGRAMS := fork-demo exec-demo wait-demo zombie-demo orphan-demo

.PHONY: all clean

all: $(PROGRAMS:%=$(BIN)/%)

$(BIN):
	mkdir -p $(BIN)

$(BIN)/%: examples/%.c | $(BIN)
	$(CC) $(CFLAGS) $< -o $@

clean:
	rm -rf $(BIN)
