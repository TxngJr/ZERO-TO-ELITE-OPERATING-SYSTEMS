#include <pthread.h>
#include <sched.h>
#include <stdatomic.h>
#include <stdio.h>
#include <stdlib.h>

enum { THREAD_COUNT = 4, ITERATIONS = 100000 };

static _Atomic long counter = 0;

static void *worker(void *arg)
{
    (void)arg;

    for (int i = 0; i < ITERATIONS; ++i) {
        long old = atomic_load_explicit(&counter, memory_order_relaxed);

        if ((i & 0x3ff) == 0) {
            sched_yield();
        }

        atomic_store_explicit(&counter, old + 1, memory_order_relaxed);
    }

    return NULL;
}

int main(void)
{
    pthread_t threads[THREAD_COUNT];

    for (int i = 0; i < THREAD_COUNT; ++i) {
        int rc = pthread_create(&threads[i], NULL, worker, NULL);
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

    const long expected = (long)THREAD_COUNT * ITERATIONS;
    const long observed = atomic_load_explicit(&counter, memory_order_relaxed);

    printf("expected=%ld observed=%ld lost=%ld\n",
           expected, observed, expected - observed);

    return 0;
}
