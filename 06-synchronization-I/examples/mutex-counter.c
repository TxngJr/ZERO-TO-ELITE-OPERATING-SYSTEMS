#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>

enum { THREAD_COUNT = 4, ITERATIONS = 100000 };

static long counter = 0;
static pthread_mutex_t counter_mutex = PTHREAD_MUTEX_INITIALIZER;

static void *worker(void *arg)
{
    (void)arg;

    for (int i = 0; i < ITERATIONS; ++i) {
        int rc = pthread_mutex_lock(&counter_mutex);
        if (rc != 0) {
            fprintf(stderr, "pthread_mutex_lock failed: %d\n", rc);
            return (void *)1;
        }

        ++counter;

        rc = pthread_mutex_unlock(&counter_mutex);
        if (rc != 0) {
            fprintf(stderr, "pthread_mutex_unlock failed: %d\n", rc);
            return (void *)1;
        }
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
        void *result = NULL;
        int rc = pthread_join(threads[i], &result);
        if (rc != 0 || result != NULL) {
            fprintf(stderr, "worker failed\n");
            return 1;
        }
    }

    printf("expected=%ld observed=%ld\n",
           (long)THREAD_COUNT * ITERATIONS, counter);

    pthread_mutex_destroy(&counter_mutex);
    return 0;
}
