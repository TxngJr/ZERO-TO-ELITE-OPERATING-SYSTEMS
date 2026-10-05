#include <stdbool.h>
#include <stdio.h>

enum { N = 4 };

static bool graph[N][N] = {
    {false, true,  false, false},
    {false, false, true,  false},
    {true,  false, false, false},
    {false, false, true,  false}
};

static bool dfs(int node, bool visited[N], bool active[N])
{
    visited[node] = true;
    active[node] = true;

    for (int next = 0; next < N; ++next) {
        if (!graph[node][next]) {
            continue;
        }

        if (!visited[next] && dfs(next, visited, active)) {
            return true;
        }

        if (active[next]) {
            return true;
        }
    }

    active[node] = false;
    return false;
}

int main(void)
{
    bool visited[N] = {false};
    bool active[N] = {false};

    puts("wait-for edges:");
    for (int i = 0; i < N; ++i) {
        for (int j = 0; j < N; ++j) {
            if (graph[i][j]) {
                printf("T%d -> T%d\n", i, j);
            }
        }
    }

    bool cycle = false;
    for (int i = 0; i < N && !cycle; ++i) {
        if (!visited[i]) {
            cycle = dfs(i, visited, active);
        }
    }

    printf("cycle_detected=%s\n", cycle ? "yes" : "no");
    return cycle ? 0 : 1;
}
