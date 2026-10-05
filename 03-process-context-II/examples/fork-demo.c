#define _POSIX_C_SOURCE 200809L
#include <errno.h>
#include <stdio.h>
#include <stdlib.h>
#include <sys/types.h>
#include <unistd.h>

int main(void)
{
    printf("Before fork: PID=%ld PPID=%ld\n", (long)getpid(), (long)getppid());
    fflush(stdout);

    pid_t pid = fork();

    if (pid < 0) {
        perror("fork");
        return 1;
    }

    if (pid == 0) {
        printf("Child : PID=%ld PPID=%ld fork_return=%ld\n",
               (long)getpid(), (long)getppid(), (long)pid);
    } else {
        printf("Parent: PID=%ld PPID=%ld child_PID=%ld\n",
               (long)getpid(), (long)getppid(), (long)pid);
    }

    return 0;
}
