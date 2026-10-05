#define _POSIX_C_SOURCE 200809L
#include <stdio.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/wait.h>
#include <unistd.h>

int main(void)
{
    pid_t pid = fork();

    if (pid < 0) {
        perror("fork");
        return 1;
    }

    if (pid == 0) {
        _exit(0);
    }

    printf("Parent PID=%ld, child PID=%ld\n", (long)getpid(), (long)pid);
    printf("Child has exited. Inspect it during the next 20 seconds.\n");
    printf("Try: ps -o pid,ppid,stat,comm -p %ld,%ld\n", (long)getpid(), (long)pid);
    fflush(stdout);

    sleep(20);

    int status = 0;
    if (waitpid(pid, &status, 0) < 0) {
        perror("waitpid");
        return 1;
    }

    printf("Child reaped; zombie entry should be gone.\n");
    return 0;
}
