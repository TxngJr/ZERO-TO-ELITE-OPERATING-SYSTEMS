class Program
{
    static int Main(string[] args)
    {
        string mode = args.Length == 0 ? "monitor" : args[0];

        switch (mode)
        {
            case "graph":
                WaitForGraph();
                return 0;
            case "livelock":
                Livelock();
                return 0;
            case "self-test":
                SelfTest();
                return 0;
            default:
                MonitorQueue();
                return 0;
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

        if (queue.Count != 0)
            throw new InvalidOperationException("Monitor queue invariant failed.");
    }

    static bool HasCycle(int[][] edges)
    {
        bool[] visited = new bool[edges.Length];
        bool[] active = new bool[edges.Length];

        bool Dfs(int node)
        {
            visited[node] = true;
            active[node] = true;

            foreach (int next in edges[node])
            {
                if (next < 0 || next >= edges.Length)
                    throw new ArgumentOutOfRangeException(nameof(edges), "Invalid graph edge.");

                if (!visited[next] && Dfs(next)) return true;
                if (active[next]) return true;
            }

            active[node] = false;
            return false;
        }

        for (int i = 0; i < edges.Length; i++)
            if (!visited[i] && Dfs(i))
                return true;

        return false;
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

        Console.WriteLine($"cycle_detected={HasCycle(edges)}");
    }

    static void Livelock()
    {
        // Deterministic bounded livelock demonstration.
        // Three barriers make the phases explicit:
        // 1) both announce intent,
        // 2) both snapshot the conflict before either backs off,
        // 3) both finish backing off before the next round.
        const int rounds = 6;
        int[] intent = new int[2];
        bool[] conflict = new bool[2];

        using Barrier barrier = new(2);
        Thread[] workers = new Thread[2];

        for (int i = 0; i < workers.Length; i++)
        {
            int id = i;
            int other = 1 - i;

            workers[i] = new Thread(() =>
            {
                for (int round = 1; round <= rounds; round++)
                {
                    Volatile.Write(ref intent[id], 1);
                    barrier.SignalAndWait();

                    conflict[id] = Volatile.Read(ref intent[other]) == 1;
                    barrier.SignalAndWait();

                    if (conflict[id])
                    {
                        Console.WriteLine($"worker-{id}: conflict round={round}, backing off");
                        Volatile.Write(ref intent[id], 0);
                    }
                    else
                    {
                        Console.WriteLine($"worker-{id}: useful progress");
                    }

                    barrier.SignalAndWait();
                }

                Console.WriteLine($"worker-{id}: bounded livelock demo ended");
            });

            workers[i].Start();
        }

        foreach (Thread worker in workers)
        {
            if (!worker.Join(TimeSpan.FromSeconds(5)))
                throw new TimeoutException("Livelock demo did not terminate.");
        }

        if (intent.Any(v => v != 0))
            throw new InvalidOperationException("Livelock demo left intent set.");
    }

    static void SelfTest()
    {
        int[][] cyclic =
        {
            new[] { 1 },
            new[] { 2 },
            new[] { 0 }
        };

        int[][] acyclic =
        {
            new[] { 1 },
            new[] { 2 },
            Array.Empty<int>()
        };

        if (!HasCycle(cyclic))
            throw new Exception("Cycle detector missed a cycle.");

        if (HasCycle(acyclic))
            throw new Exception("Cycle detector reported a false cycle.");

        Console.WriteLine("Chapter08 self-test PASS");
    }
}
