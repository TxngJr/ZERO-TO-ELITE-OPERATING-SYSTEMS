#define _POSIX_C_SOURCE 200809L
#include <errno.h>
#include <pthread.h>
#include <semaphore.h>
#include <stdatomic.h>
#include <stdio.h>
#include <stdlib.h>
#include <time.h>

enum { WORKER_COUNT = 6, CAPACITY = 2 };

static sem_t slots;
static _Atomic int active = 0;

static void sleep_ms(long ms)
{
    struct timespec ts = {
        .tv_sec = ms / 1000,
        .tv_nsec = (ms % 1000) * 1000000L
    };
    while (nanosleep(&ts, &ts) == -1 && errno == EINTR) {
    }
}

static int wait_slot(void)
{
    while (sem_wait(&slots) == -1) {
        if (errno != EINTR) {
            perror("sem_wait");
            return -1;
        }
    }
    return 0;
}

static void *worker(void *arg)
{
    long id = *(long *)arg;

    if (wait_slot() != 0) {
        return (void *)1;
    }

    int now = atomic_fetch_add_explicit(&active, 1, memory_order_relaxed) + 1;
    printf("worker %ld entered; active=%d\n", id, now);
    fflush(stdout);

    sleep_ms(300);

    now = atomic_fetch_sub_explicit(&active, 1, memory_order_relaxed) - 1;
    printf("worker %ld leaving; active=%d\n", id, now);

    if (sem_post(&slots) == -1) {
        perror("sem_post");
        return (void *)1;
    }

    return NULL;
}

int main(void)
{
    pthread_t threads[WORKER_COUNT];
    long ids[WORKER_COUNT];

    if (sem_init(&slots, 0, CAPACITY) == -1) {
        perror("sem_init");
        return 1;
    }

    for (long i = 0; i < WORKER_COUNT; ++i) {
        ids[i] = i + 1;
        int rc = pthread_create(&threads[i], NULL, worker, &ids[i]);
        if (rc != 0) {
            fprintf(stderr, "pthread_create failed: %d\n", rc);
            return 1;
        }
    }

    for (int i = 0; i < WORKER_COUNT; ++i) {
        void *result = NULL;
        int rc = pthread_join(threads[i], &result);
        if (rc != 0 || result != NULL) {
            fprintf(stderr, "worker failed\n");
            return 1;
        }
    }

    sem_destroy(&slots);
    return 0;
}
