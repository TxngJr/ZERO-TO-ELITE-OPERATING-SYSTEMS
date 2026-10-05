#define _GNU_SOURCE
#include <stdio.h>
#include <stdlib.h>
#include <sys/mman.h>
#include <sys/resource.h>
#include <unistd.h>

enum { MIB = 1024 * 1024 };
static const size_t REGION_SIZE = 32U * MIB;

static void print_faults(const char *label)
{
    struct rusage usage;

    if (getrusage(RUSAGE_SELF, &usage) != 0) {
        perror("getrusage");
        exit(EXIT_FAILURE);
    }

    printf("%-20s minor=%ld major=%ld\n",
           label, usage.ru_minflt, usage.ru_majflt);
}

int main(void)
{
    long page_size = sysconf(_SC_PAGESIZE);
    if (page_size <= 0) {
        perror("sysconf");
        return 1;
    }

    print_faults("before mmap");

    unsigned char *region = mmap(
        NULL,
        REGION_SIZE,
        PROT_READ | PROT_WRITE,
        MAP_PRIVATE | MAP_ANONYMOUS,
        -1,
        0
    );

    if (region == MAP_FAILED) {
        perror("mmap");
        return 1;
    }

    print_faults("after mmap");

    for (size_t offset = 0; offset < REGION_SIZE; offset += (size_t)page_size) {
        region[offset] = (unsigned char)(offset / (size_t)page_size);
    }

    print_faults("after touching pages");

    printf("region=%p size=%zu page_size=%ld pages=%zu\n",
           (void *)region,
           REGION_SIZE,
           page_size,
           REGION_SIZE / (size_t)page_size);

    if (munmap(region, REGION_SIZE) != 0) {
        perror("munmap");
        return 1;
    }

    return 0;
}
