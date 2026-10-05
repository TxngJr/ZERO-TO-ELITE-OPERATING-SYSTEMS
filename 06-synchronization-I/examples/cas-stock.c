#include <pthread.h>
#include <stdatomic.h>
#include <stdio.h>
#include <stdlib.h>

static _Atomic int stock = 1;

static void *buyer(void *arg)
{
    long id = *(long *)arg;
    int expected = 1;

    if (atomic_compare_exchange_strong_explicit(
            &stock,
            &expected,
            0,
            memory_order_relaxed,
            memory_order_relaxed)) {
        printf("buyer %ld: purchase succeeded\n", id);
    } else {
        printf("buyer %ld: sold out; observed stock=%d\n", id, expected);
    }

    return NULL;
}

int main(void)
{
    pthread_t threads[2];
    long ids[2] = {1, 2};

    for (int i = 0; i < 2; ++i) {
        int rc = pthread_create(&threads[i], NULL, buyer, &ids[i]);
        if (rc != 0) {
            fprintf(stderr, "pthread_create failed: %d\n", rc);
            return 1;
        }
    }

    for (int i = 0; i < 2; ++i) {
        int rc = pthread_join(threads[i], NULL);
        if (rc != 0) {
            fprintf(stderr, "pthread_join failed: %d\n", rc);
            return 1;
        }
    }

    printf("final stock=%d\n",
           atomic_load_explicit(&stock, memory_order_relaxed));
    return 0;
}
