#define _POSIX_C_SOURCE 200809L
#include <stdio.h>
#include <stdlib.h>
#include <sys/types.h>
#include <unistd.h>

int main(void)
{
    pid_t pid = fork();

    if (pid < 0) {
        perror("fork");
        return 1;
    }

    if (pid > 0) {
        printf("Parent PID=%ld created child PID=%ld and will exit now.\n",
               (long)getpid(), (long)pid);
        fflush(stdout);
        return 0;
    }

    printf("Child PID=%ld initial PPID=%ld\n", (long)getpid(), (long)getppid());
    fflush(stdout);

    sleep(3);

    printf("Child PID=%ld later PPID=%ld\n", (long)getpid(), (long)getppid());
    printf("The new parent is environment-dependent; do not assume it must always be PID 1.\n");
    return 0;
}
