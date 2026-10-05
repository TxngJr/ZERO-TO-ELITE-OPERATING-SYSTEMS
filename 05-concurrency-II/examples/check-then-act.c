#define _XOPEN_SOURCE 700
#include <pthread.h>
#include <stdatomic.h>
#include <stdio.h>
#include <stdlib.h>

static _Atomic int stock = 1;
static pthread_barrier_t checked_barrier;

static void *buyer(void *arg)
{
    long id = *(long *)arg;
    int observed = atomic_load_explicit(&stock, memory_order_relaxed);

    printf("buyer %ld checked stock=%d\n", id, observed);

    int rc = pthread_barrier_wait(&checked_barrier);
    if (rc != 0 && rc != PTHREAD_BARRIER_SERIAL_THREAD) {
        fprintf(stderr, "pthread_barrier_wait failed: %d\n", rc);
        return NULL;
    }

    if (observed > 0) {
        int before = atomic_fetch_sub_explicit(&stock, 1, memory_order_relaxed);
        printf("buyer %ld acted; stock changed %d -> %d\n",
               id, before, before - 1);
    }

    return NULL;
}

int main(void)
{
    pthread_t threads[2];
    long ids[2] = {1, 2};

    int rc = pthread_barrier_init(&checked_barrier, NULL, 2);
    if (rc != 0) {
        fprintf(stderr, "pthread_barrier_init failed: %d\n", rc);
        return 1;
    }

    for (int i = 0; i < 2; ++i) {
        rc = pthread_create(&threads[i], NULL, buyer, &ids[i]);
        if (rc != 0) {
            fprintf(stderr, "pthread_create failed: %d\n", rc);
            return 1;
        }
    }

    for (int i = 0; i < 2; ++i) {
        rc = pthread_join(threads[i], NULL);
        if (rc != 0) {
            fprintf(stderr, "pthread_join failed: %d\n", rc);
            return 1;
        }
    }

    printf("final stock=%d\n",
           atomic_load_explicit(&stock, memory_order_relaxed));

    pthread_barrier_destroy(&checked_barrier);
    return 0;
}
