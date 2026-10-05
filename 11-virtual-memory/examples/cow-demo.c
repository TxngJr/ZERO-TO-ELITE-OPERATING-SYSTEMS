#define _GNU_SOURCE
#include <stdio.h>
#include <stdlib.h>
#include <sys/mman.h>
#include <sys/wait.h>
#include <unistd.h>

int main(void)
{
    long page_size = sysconf(_SC_PAGESIZE);
    if (page_size <= 0) {
        perror("sysconf");
        return 1;
    }

    int *value = mmap(
        NULL,
        (size_t)page_size,
        PROT_READ | PROT_WRITE,
        MAP_PRIVATE | MAP_ANONYMOUS,
        -1,
        0
    );

    if (value == MAP_FAILED) {
        perror("mmap");
        return 1;
    }

    *value = 10;
    printf("before fork: PID=%ld VA=%p value=%d\n",
           (long)getpid(), (void *)value, *value);
    fflush(stdout);

    pid_t pid = fork();
    if (pid < 0) {
        perror("fork");
        munmap(value, (size_t)page_size);
        return 1;
    }

    if (pid == 0) {
        printf("child before write: PID=%ld VA=%p value=%d\n",
               (long)getpid(), (void *)value, *value);
        *value = 99;
        printf("child after write : PID=%ld VA=%p value=%d\n",
               (long)getpid(), (void *)value, *value);
        fflush(stdout);
        _exit(0);
    }

    int status = 0;
    if (waitpid(pid, &status, 0) < 0) {
        perror("waitpid");
        munmap(value, (size_t)page_size);
        return 1;
    }

    printf("parent after child: PID=%ld VA=%p value=%d\n",
           (long)getpid(), (void *)value, *value);

    munmap(value, (size_t)page_size);
    return 0;
}
