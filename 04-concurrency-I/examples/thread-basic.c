#define _GNU_SOURCE
#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>
#include <sys/syscall.h>
#include <unistd.h>

enum { THREAD_COUNT = 4 };

static long linux_tid(void)
{
    return syscall(SYS_gettid);
}

static void *worker(void *arg)
{
    long index = *(long *)arg;
    printf("worker=%ld PID=%ld TID=%ld\n",
           index, (long)getpid(), linux_tid());
    return NULL;
}

int main(void)
{
    pthread_t threads[THREAD_COUNT];
    long ids[THREAD_COUNT];

    printf("main PID=%ld TID=%ld\n", (long)getpid(), linux_tid());

    for (long i = 0; i < THREAD_COUNT; ++i) {
        ids[i] = i;
        int rc = pthread_create(&threads[i], NULL, worker, &ids[i]);
        if (rc != 0) {
            fprintf(stderr, "pthread_create failed: %d\n", rc);
            return 1;
        }
    }

    for (int i = 0; i < THREAD_COUNT; ++i) {
        int rc = pthread_join(threads[i], NULL);
        if (rc != 0) {
            fprintf(stderr, "pthread_join failed: %d\n", rc);
            return 1;
        }
    }

    return 0;
}
