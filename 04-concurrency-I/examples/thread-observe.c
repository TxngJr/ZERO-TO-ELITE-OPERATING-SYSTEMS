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
    long id = *(long *)arg;
    printf("worker %ld alive: TID=%ld\n", id, linux_tid());
    fflush(stdout);
    sleep(20);
    return NULL;
}

int main(void)
{
    pthread_t threads[THREAD_COUNT];
    long ids[THREAD_COUNT];

    printf("PID=%ld. Inspect with: ps -L -p %ld -o pid,tid,psr,stat,comm\n",
           (long)getpid(), (long)getpid());

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
