#define _GNU_SOURCE
#include <signal.h>
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

    unsigned char *page = mmap(
        NULL,
        (size_t)page_size,
        PROT_READ | PROT_WRITE,
        MAP_PRIVATE | MAP_ANONYMOUS,
        -1,
        0
    );

    if (page == MAP_FAILED) {
        perror("mmap");
        return 1;
    }

    page[0] = 42;

    if (mprotect(page, (size_t)page_size, PROT_READ) != 0) {
        perror("mprotect");
        munmap(page, (size_t)page_size);
        return 1;
    }

    printf("parent: mapping %p is now read-only; value=%u\n",
           (void *)page, (unsigned)page[0]);
    fflush(stdout);

    pid_t pid = fork();
    if (pid < 0) {
        perror("fork");
        munmap(page, (size_t)page_size);
        return 1;
    }

    if (pid == 0) {
        printf("child: attempting a write to read-only mapping...\n");
        fflush(stdout);
        page[0] = 99;
        _exit(0);
    }

    int status = 0;
    if (waitpid(pid, &status, 0) < 0) {
        perror("waitpid");
        munmap(page, (size_t)page_size);
        return 1;
    }

    if (WIFSIGNALED(status)) {
        int sig = WTERMSIG(status);
        printf("parent: child terminated by signal %d (%s)\n",
               sig, sig == SIGSEGV ? "SIGSEGV" : "other");
    } else {
        printf("parent: child did not terminate by signal; status=%d\n", status);
    }

    munmap(page, (size_t)page_size);
    return 0;
}
