#define _POSIX_C_SOURCE 200809L
#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>
#include <time.h>

enum { PHILOSOPHERS = 5, ROUNDS = 3 };

static pthread_mutex_t forks[PHILOSOPHERS];

static void sleep_ms(long ms)
{
    struct timespec ts = {
        .tv_sec = ms / 1000,
        .tv_nsec = (ms % 1000) * 1000000L
    };
    nanosleep(&ts, NULL);
}

static void *philosopher(void *arg)
{
    long id = *(long *)arg;
    int left = (int)id;
    int right = ((int)id + 1) % PHILOSOPHERS;

    int first = left < right ? left : right;
    int second = left < right ? right : left;

    for (int round = 0; round < ROUNDS; ++round) {
        printf("P%ld thinking\n", id);
        sleep_ms(20 + id * 5);

        pthread_mutex_lock(&forks[first]);
        pthread_mutex_lock(&forks[second]);

        printf("P%ld eating round=%d using forks %d,%d\n",
               id, round + 1, first, second);
        sleep_ms(30);

        pthread_mutex_unlock(&forks[second]);
        pthread_mutex_unlock(&forks[first]);
    }

    return NULL;
}

int main(void)
{
    pthread_t threads[PHILOSOPHERS];
    long ids[PHILOSOPHERS];

    for (int i = 0; i < PHILOSOPHERS; ++i) {
        pthread_mutex_init(&forks[i], NULL);
    }

    for (long i = 0; i < PHILOSOPHERS; ++i) {
        ids[i] = i;
        if (pthread_create(&threads[i], NULL, philosopher, &ids[i]) != 0) {
            fprintf(stderr, "pthread_create failed\n");
            return 1;
        }
    }

    for (int i = 0; i < PHILOSOPHERS; ++i) {
        pthread_join(threads[i], NULL);
    }

    for (int i = 0; i < PHILOSOPHERS; ++i) {
        pthread_mutex_destroy(&forks[i]);
    }

    puts("all philosophers completed without circular-wait deadlock");
    return 0;
}
