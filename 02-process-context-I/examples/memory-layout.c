#include <stdio.h>
#include <stdlib.h>

static int initialized_global = 1234;
static int bss_global;

int main(void)
{
    int stack_local = 42;
    int *heap_value = malloc(sizeof(*heap_value));

    if (heap_value == NULL) {
        perror("malloc");
        return 1;
    }

    *heap_value = 99;

    printf("PID-visible address sample (addresses may change across runs)\n");
    printf("initialized global : %p value=%d\n", (void *)&initialized_global, initialized_global);
    printf("BSS/global zero    : %p value=%d\n", (void *)&bss_global, bss_global);
    printf("heap allocation    : %p value=%d\n", (void *)heap_value, *heap_value);
    printf("stack local        : %p value=%d\n", (void *)&stack_local, stack_local);

    free(heap_value);
    return 0;
}
