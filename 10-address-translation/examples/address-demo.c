#define _POSIX_C_SOURCE 200809L
#include <inttypes.h>
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>

static int global_value = 123;
static const char readonly_value[] = "read-only sample";

static void print_address(const char *name, const void *ptr, long page_size)
{
    uintptr_t address = (uintptr_t)ptr;
    uintptr_t vpn = address / (uintptr_t)page_size;
    uintptr_t offset = address % (uintptr_t)page_size;

    printf("%-12s VA=%p VPN=0x%" PRIxPTR " offset=0x%" PRIxPTR "\n",
           name, ptr, vpn, offset);
}

int main(void)
{
    long page_size = sysconf(_SC_PAGESIZE);
    if (page_size <= 0) {
        perror("sysconf");
        return 1;
    }

    int stack_value = 456;
    int *heap_value = malloc(sizeof(*heap_value));

    if (heap_value == NULL) {
        perror("malloc");
        return 1;
    }

    *heap_value = 789;

    printf("page_size=%ld bytes\n", page_size);
    print_address("rodata", readonly_value, page_size);
    print_address("global", &global_value, page_size);
    print_address("heap", heap_value, page_size);
    print_address("stack", &stack_value, page_size);

    free(heap_value);
    return 0;
}
