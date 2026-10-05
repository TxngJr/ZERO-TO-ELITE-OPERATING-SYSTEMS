#define _XOPEN_SOURCE 700
#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>
#include <time.h>

enum { READERS = 4, WRITERS = 2, ROUNDS = 4 };

static int shared_value = 0;
static pthread_rwlock_t rwlock = PTHREAD_RWLOCK_INITIALIZER;

static void sleep_ms(long ms)
{
    struct timespec ts = {
        .tv_sec = ms / 1000,
        .tv_nsec = (ms % 1000) * 1000000L
    };
    nanosleep(&ts, NULL);
}

static void *reader(void *arg)
{
    long id = *(long *)arg;

    for (int i = 0; i < ROUNDS; ++i) {
        pthread_rwlock_rdlock(&rwlock);
        int value = shared_value;
        printf("reader %ld saw %d\n", id, value);
        sleep_ms(30);
        pthread_rwlock_unlock(&rwlock);
        sleep_ms(20);
    }

    return NULL;
}

static void *writer(void *arg)
{
    long id = *(long *)arg;

    for (int i = 0; i < ROUNDS; ++i) {
        pthread_rwlock_wrlock(&rwlock);
        ++shared_value;
        printf("writer %ld set %d\n", id, shared_value);
        sleep_ms(40);
        pthread_rwlock_unlock(&rwlock);
        sleep_ms(50);
    }

    return NULL;
}

int main(void)
{
    pthread_t readers[READERS];
    pthread_t writers[WRITERS];
    long reader_ids[READERS];
    long writer_ids[WRITERS];

    for (long i = 0; i < READERS; ++i) {
        reader_ids[i] = i + 1;
        pthread_create(&readers[i], NULL, reader, &reader_ids[i]);
    }

    for (long i = 0; i < WRITERS; ++i) {
        writer_ids[i] = i + 1;
        pthread_create(&writers[i], NULL, writer, &writer_ids[i]);
    }

    for (int i = 0; i < READERS; ++i) {
        pthread_join(readers[i], NULL);
    }

    for (int i = 0; i < WRITERS; ++i) {
        pthread_join(writers[i], NULL);
    }

    printf("final shared_value=%d expected=%d\n",
           shared_value, WRITERS * ROUNDS);

    pthread_rwlock_destroy(&rwlock);
    return 0;
}
