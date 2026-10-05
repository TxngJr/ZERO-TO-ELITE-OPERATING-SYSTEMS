class Program
{
    static void Main(string[] args)
    {
        string mode = args.Length == 0 ? "monitor" : args[0];

        switch (mode)
        {
            case "graph":
                WaitForGraph();
                break;
            case "livelock":
                Livelock();
                break;
            default:
                MonitorQueue();
                break;
        }
    }

    static void MonitorQueue()
    {
        Queue<int> queue = new();
        object sync = new();
        const int capacity = 3;

        void Put(int item)
        {
            lock (sync)
            {
                while (queue.Count == capacity)
                    Monitor.Wait(sync);

                queue.Enqueue(item);
                Console.WriteLine($"put {item}, count={queue.Count}");
                Monitor.PulseAll(sync);
            }
        }

        int Get()
        {
            lock (sync)
            {
                while (queue.Count == 0)
                    Monitor.Wait(sync);

                int item = queue.Dequeue();
                Console.WriteLine($"get {item}, count={queue.Count}");
                Monitor.PulseAll(sync);
                return item;
            }
        }

        Thread producer = new Thread(() =>
        {
            for (int i = 1; i <= 12; i++) Put(i);
        });

        Thread consumer = new Thread(() =>
        {
            for (int i = 1; i <= 12; i++) Get();
        });

        consumer.Start();
        producer.Start();
        producer.Join();
        consumer.Join();
    }

    static void WaitForGraph()
    {
        int[][] edges =
        {
            new[] { 1 },
            new[] { 2 },
            new[] { 0 },
            new[] { 2 }
        };

        bool[] visited = new bool[edges.Length];
        bool[] active = new bool[edges.Length];

        bool Dfs(int node)
        {
            visited[node] = true;
            active[node] = true;

            foreach (int next in edges[node])
            {
                if (!visited[next] && Dfs(next)) return true;
                if (active[next]) return true;
            }

            active[node] = false;
            return false;
        }

        bool cycle = false;
        for (int i = 0; i < edges.Length && !cycle; i++)
            if (!visited[i]) cycle = Dfs(i);

        Console.WriteLine($"cycle_detected={cycle}");
    }

    static void Livelock()
    {
        int[] intent = new int[2];
        Barrier barrier = new(2);

        Thread[] workers = new Thread[2];

        for (int i = 0; i < 2; i++)
        {
            int id = i;
            int other = 1 - i;

            workers[i] = new Thread(() =>
            {
                for (int round = 1; round <= 6; round++)
                {
                    Volatile.Write(ref intent[id], 1);
                    barrier.SignalAndWait();

                    if (Volatile.Read(ref intent[other]) == 1)
                    {
                        Console.WriteLine($"worker-{id}: conflict round={round}, back off");
                        Volatile.Write(ref intent[id], 0);
                        barrier.SignalAndWait();
                        continue;
                    }

                    Console.WriteLine($"worker-{id}: progress");
                    Volatile.Write(ref intent[id], 0);
                    return;
                }

                Console.WriteLine($"worker-{id}: bounded retry ended");
            });
            workers[i].Start();
        }

        foreach (Thread worker in workers) worker.Join();
    }
}
