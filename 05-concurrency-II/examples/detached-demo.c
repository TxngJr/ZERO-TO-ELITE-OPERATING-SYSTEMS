#define _POSIX_C_SOURCE 200809L
#include <pthread.h>
#include <stdatomic.h>
#include <stdio.h>
#include <stdlib.h>
#include <time.h>

static _Atomic int done = 0;

static void sleep_ms(long ms)
{
    struct timespec ts = {
        .tv_sec = ms / 1000,
        .tv_nsec = (ms % 1000) * 1000000L
    };
    nanosleep(&ts, NULL);
}

static void *worker(void *arg)
{
    (void)arg;
    printf("detached worker: starting\n");
    sleep_ms(300);
    printf("detached worker: finished\n");
    atomic_store_explicit(&done, 1, memory_order_release);
    return NULL;
}

int main(void)
{
    pthread_t thread;

    int rc = pthread_create(&thread, NULL, worker, NULL);
    if (rc != 0) {
        fprintf(stderr, "pthread_create failed: %d\n", rc);
        return 1;
    }

    rc = pthread_detach(thread);
    if (rc != 0) {
        fprintf(stderr, "pthread_detach failed: %d\n", rc);
        return 1;
    }

    printf("main: thread detached; pthread_join is intentionally not used\n");

    while (atomic_load_explicit(&done, memory_order_acquire) == 0) {
        sleep_ms(10);
    }

    printf("main: observed completion and will exit\n");
    return 0;
}
