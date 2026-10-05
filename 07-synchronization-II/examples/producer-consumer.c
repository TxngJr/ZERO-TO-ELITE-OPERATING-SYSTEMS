#define _POSIX_C_SOURCE 200809L
#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>

enum {
    CAPACITY = 4,
    PRODUCERS = 2,
    CONSUMERS = 2,
    ITEMS_PER_PRODUCER = 8
};

static int buffer[CAPACITY];
static int head = 0;
static int tail = 0;
static int count = 0;
static int next_item = 1;

static pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;
static pthread_cond_t not_empty = PTHREAD_COND_INITIALIZER;
static pthread_cond_t not_full = PTHREAD_COND_INITIALIZER;

static void *producer(void *arg)
{
    long id = *(long *)arg;

    for (int i = 0; i < ITEMS_PER_PRODUCER; ++i) {
        pthread_mutex_lock(&mutex);

        while (count == CAPACITY) {
            pthread_cond_wait(&not_full, &mutex);
        }

        int item = next_item++;
        buffer[tail] = item;
        tail = (tail + 1) % CAPACITY;
        ++count;

        printf("producer %ld -> item=%d count=%d\n", id, item, count);

        pthread_cond_signal(&not_empty);
        pthread_mutex_unlock(&mutex);
    }

    return NULL;
}

static void *consumer(void *arg)
{
    long id = *(long *)arg;
    const int total_items = PRODUCERS * ITEMS_PER_PRODUCER;
    const int items_for_this_consumer = total_items / CONSUMERS;

    for (int i = 0; i < items_for_this_consumer; ++i) {
        pthread_mutex_lock(&mutex);

        while (count == 0) {
            pthread_cond_wait(&not_empty, &mutex);
        }

        int item = buffer[head];
        head = (head + 1) % CAPACITY;
        --count;

        printf("consumer %ld <- item=%d count=%d\n", id, item, count);

        pthread_cond_signal(&not_full);
        pthread_mutex_unlock(&mutex);
    }

    return NULL;
}

int main(void)
{
    pthread_t producers[PRODUCERS];
    pthread_t consumers[CONSUMERS];
    long producer_ids[PRODUCERS];
    long consumer_ids[CONSUMERS];

    for (long i = 0; i < CONSUMERS; ++i) {
        consumer_ids[i] = i + 1;
        if (pthread_create(&consumers[i], NULL, consumer, &consumer_ids[i]) != 0) {
            fprintf(stderr, "pthread_create consumer failed\n");
            return 1;
        }
    }

    for (long i = 0; i < PRODUCERS; ++i) {
        producer_ids[i] = i + 1;
        if (pthread_create(&producers[i], NULL, producer, &producer_ids[i]) != 0) {
            fprintf(stderr, "pthread_create producer failed\n");
            return 1;
        }
    }

    for (int i = 0; i < PRODUCERS; ++i) {
        pthread_join(producers[i], NULL);
    }

    for (int i = 0; i < CONSUMERS; ++i) {
        pthread_join(consumers[i], NULL);
    }

    printf("final count=%d (expected 0)\n", count);

    pthread_cond_destroy(&not_empty);
    pthread_cond_destroy(&not_full);
    pthread_mutex_destroy(&mutex);
    return 0;
}
