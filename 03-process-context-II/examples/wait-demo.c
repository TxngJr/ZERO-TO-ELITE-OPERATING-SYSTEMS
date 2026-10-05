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
        printf("Child PID=%ld: exiting with status 42\n", (long)getpid());
        fflush(stdout);
        _exit(42);
    }

    int status = 0;
    pid_t waited = waitpid(pid, &status, 0);

    if (waited < 0) {
        perror("waitpid");
        return 1;
    }

    printf("Parent reaped PID=%ld\n", (long)waited);

    if (WIFEXITED(status)) {
        printf("Exit status=%d\n", WEXITSTATUS(status));
    }

    return 0;
}
