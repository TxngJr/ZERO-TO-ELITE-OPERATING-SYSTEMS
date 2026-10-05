#define _POSIX_C_SOURCE 200809L
#include <pthread.h>
#include <stdatomic.h>
#include <stdio.h>
#include <time.h>

enum { ROUNDS = 6 };

static _Atomic int intent[2] = {0, 0};

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
    long id = *(long *)arg;
    int other = 1 - (int)id;

    for (int round = 0; round < ROUNDS; ++round) {
        atomic_store_explicit(&intent[id], 1, memory_order_seq_cst);
        sleep_ms(10);

        if (atomic_load_explicit(&intent[other], memory_order_seq_cst)) {
            printf("worker %ld: conflict at round %d, politely backing off\n",
                   id, round + 1);
            atomic_store_explicit(&intent[id], 0, memory_order_seq_cst);
            sleep_ms(10);
            continue;
        }

        printf("worker %ld: made useful progress\n", id);
        atomic_store_explicit(&intent[id], 0, memory_order_seq_cst);
        return NULL;
    }

    printf("worker %ld: bounded demo ended after repeated retries\n", id);
    atomic_store_explicit(&intent[id], 0, memory_order_seq_cst);
    return NULL;
}

int main(void)
{
    pthread_t threads[2];
    long ids[2] = {0, 1};

    pthread_create(&threads[0], NULL, worker, &ids[0]);
    pthread_create(&threads[1], NULL, worker, &ids[1]);

    pthread_join(threads[0], NULL);
    pthread_join(threads[1], NULL);

    puts("Demo is bounded intentionally: a real livelock could continue indefinitely.");
    return 0;
}
