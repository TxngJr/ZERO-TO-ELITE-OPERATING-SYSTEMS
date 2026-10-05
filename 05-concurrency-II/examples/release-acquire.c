#include <pthread.h>
#include <stdatomic.h>
#include <stdio.h>
#include <stdlib.h>

static int payload = 0;
static _Atomic int ready = 0;

static void *writer(void *arg)
{
    (void)arg;

    payload = 42;
    atomic_store_explicit(&ready, 1, memory_order_release);
    return NULL;
}

static void *reader(void *arg)
{
    (void)arg;

    while (atomic_load_explicit(&ready, memory_order_acquire) == 0) {
        /* Spin only for this small teaching demo. */
    }

    printf("reader observed payload=%d\n", payload);
    return NULL;
}

int main(void)
{
    pthread_t writer_thread;
    pthread_t reader_thread;

    int rc = pthread_create(&reader_thread, NULL, reader, NULL);
    if (rc != 0) {
        fprintf(stderr, "pthread_create(reader) failed: %d\n", rc);
        return 1;
    }

    rc = pthread_create(&writer_thread, NULL, writer, NULL);
    if (rc != 0) {
        fprintf(stderr, "pthread_create(writer) failed: %d\n", rc);
        return 1;
    }

    pthread_join(writer_thread, NULL);
    pthread_join(reader_thread, NULL);
    return 0;
}
