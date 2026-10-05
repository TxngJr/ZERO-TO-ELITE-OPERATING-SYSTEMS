#define _POSIX_C_SOURCE 200809L
#include <stdio.h>
#include <unistd.h>

int main(void)
{
    printf("PID  = %ld\n", (long)getpid());
    printf("PPID = %ld\n", (long)getppid());
    printf("Inspect /proc/<PID> from another terminal. Sleeping for 30 seconds...\n");
    fflush(stdout);

    sleep(30);
    return 0;
}
