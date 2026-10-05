#define _POSIX_C_SOURCE 200809L
#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>

enum { CAPACITY = 3, ITEMS = 12 };

typedef struct {
    int data[CAPACITY];
    int head;
    int tail;
    int count;
    pthread_mutex_t mutex;
    pthread_cond_t not_empty;
    pthread_cond_t not_full;
} monitor_queue;

static monitor_queue queue = {
    .head = 0,
    .tail = 0,
    .count = 0,
    .mutex = PTHREAD_MUTEX_INITIALIZER,
    .not_empty = PTHREAD_COND_INITIALIZER,
    .not_full = PTHREAD_COND_INITIALIZER
};

static void queue_put(monitor_queue *q, int item)
{
    pthread_mutex_lock(&q->mutex);

    while (q->count == CAPACITY) {
        pthread_cond_wait(&q->not_full, &q->mutex);
    }

    q->data[q->tail] = item;
    q->tail = (q->tail + 1) % CAPACITY;
    ++q->count;

    pthread_cond_signal(&q->not_empty);
    pthread_mutex_unlock(&q->mutex);
}

static int queue_get(monitor_queue *q)
{
    pthread_mutex_lock(&q->mutex);

    while (q->count == 0) {
        pthread_cond_wait(&q->not_empty, &q->mutex);
    }

    int item = q->data[q->head];
    q->head = (q->head + 1) % CAPACITY;
    --q->count;

    pthread_cond_signal(&q->not_full);
    pthread_mutex_unlock(&q->mutex);
    return item;
}

static void *producer(void *arg)
{
    (void)arg;

    for (int i = 1; i <= ITEMS; ++i) {
        queue_put(&queue, i);
        printf("put %d\n", i);
    }

    return NULL;
}

static void *consumer(void *arg)
{
    (void)arg;

    for (int i = 0; i < ITEMS; ++i) {
        int item = queue_get(&queue);
        printf("got %d\n", item);
    }

    return NULL;
}

int main(void)
{
    pthread_t p;
    pthread_t c;

    if (pthread_create(&c, NULL, consumer, NULL) != 0 ||
        pthread_create(&p, NULL, producer, NULL) != 0) {
        fprintf(stderr, "pthread_create failed\n");
        return 1;
    }

    pthread_join(p, NULL);
    pthread_join(c, NULL);

    printf("final queue count=%d\n", queue.count);

    pthread_cond_destroy(&queue.not_empty);
    pthread_cond_destroy(&queue.not_full);
    pthread_mutex_destroy(&queue.mutex);
    return 0;
}
