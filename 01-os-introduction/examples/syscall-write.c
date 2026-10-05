#define _GNU_SOURCE
#include <errno.h>
#include <stdio.h>
#include <string.h>
#include <sys/syscall.h>
#include <unistd.h>

int main(void)
{
    const char message[] = "Hello through SYS_write\n";
    long rc = syscall(SYS_write, STDOUT_FILENO, message, sizeof(message) - 1U);

    if (rc == -1) {
        fprintf(stderr, "syscall(SYS_write) failed: %s\n", strerror(errno));
        return 1;
    }

    return 0;
}
