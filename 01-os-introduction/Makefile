CC := gcc
CFLAGS := -Wall -Wextra -Wpedantic -std=c17
BIN := bin

.PHONY: all clean

all: $(BIN)/hello $(BIN)/syscall-write

$(BIN):
	mkdir -p $(BIN)

$(BIN)/hello: examples/hello.c | $(BIN)
	$(CC) $(CFLAGS) $< -o $@

$(BIN)/syscall-write: examples/syscall-write.c | $(BIN)
	$(CC) $(CFLAGS) $< -o $@

clean:
	rm -rf $(BIN)
