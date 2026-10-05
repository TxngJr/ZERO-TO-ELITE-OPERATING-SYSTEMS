#define _POSIX_C_SOURCE 200809L
#include <errno.h>
#include <inttypes.h>
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>

int main(int argc, char **argv)
{
    if (argc != 2) {
        fprintf(stderr, "usage: %s <virtual-address, e.g. 0x12345678>\n", argv[0]);
        return 2;
    }

    errno = 0;
    char *end = NULL;
    uintmax_t value = strtoumax(argv[1], &end, 0);

    if (errno != 0 || end == argv[1] || *end != '\0' || value > UINTPTR_MAX) {
        fprintf(stderr, "invalid address: %s\n", argv[1]);
        return 2;
    }

    long page_size = sysconf(_SC_PAGESIZE);
    if (page_size <= 0) {
        perror("sysconf");
        return 1;
    }

    uintptr_t va = (uintptr_t)value;
    uintptr_t vpn = va / (uintptr_t)page_size;
    uintptr_t offset = va % (uintptr_t)page_size;

    printf("VA        = 0x%" PRIxPTR " (%" PRIuPTR ")\n", va, va);
    printf("page size = %ld bytes\n", page_size);
    printf("VPN       = 0x%" PRIxPTR "\n", vpn);
    printf("offset    = 0x%" PRIxPTR " (%" PRIuPTR ")\n", offset, offset);

    return 0;
}
