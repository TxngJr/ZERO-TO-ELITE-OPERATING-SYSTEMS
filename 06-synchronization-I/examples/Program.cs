class Program
{
    static readonly object MonitorLock = new();
    static readonly Lock ModernLock = new();

    static int counter;
    static int stock = 1;

    static int Main(string[] args)
    {
        string mode = args.Length == 0 ? "monitor-lock-counter" : args[0];

        switch (mode)
        {
            case "modern-lock-counter":
                ModernLockCounterDemo();
                break;
            case "semaphore":
                SemaphoreDemo();
                break;
            case "interlocked":
                InterlockedDemo();
                break;
            case "cas":
                CasDemo();
                break;
            case "self-test":
                SelfTest();
                break;
            default:
                MonitorCompatibleLockCounterDemo();
                break;
        }

        return 0;
    }

    static int RunCounter(Action increment)
    {
        counter = 0;
        const int iterations = 100_000;

        Thread[] threads = new Thread[4];

        for (int i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(() =>
            {
                for (int j = 0; j < iterations; j++)
                    increment();
            });

            threads[i].Start();
        }

        foreach (Thread thread in threads)
            thread.Join();

        return iterations * threads.Length;
    }

    static void MonitorCompatibleLockCounterDemo()
    {
        int expected = RunCounter(() =>
        {
            lock (MonitorLock)
            {
                counter++;
            }
        });

        Console.WriteLine($"object+lock expected={expected} observed={counter}");
    }

    static void ModernLockCounterDemo()
    {
        int expected = RunCounter(() =>
        {
            lock (ModernLock)
            {
                counter++;
            }
        });

        Console.WriteLine($"System.Threading.Lock expected={expected} observed={counter}");
    }

    static void InterlockedDemo()
    {
        int expected = RunCounter(() => Interlocked.Increment(ref counter));
        Console.WriteLine($"Interlocked expected={expected} observed={counter}");
    }

    static void SemaphoreDemo()
    {
        using SemaphoreSlim slots = new(2, 2);
        int active = 0;
        int maximumObserved = 0;

        Thread[] workers = new Thread[6];

        for (int i = 0; i < workers.Length; i++)
        {
            int id = i + 1;

            workers[i] = new Thread(() =>
            {
                slots.Wait();

                try
                {
                    int now = Interlocked.Increment(ref active);

                    int snapshot;
                    do
                    {
                        snapshot = Volatile.Read(ref maximumObserved);
                        if (now <= snapshot) break;
                    }
                    while (Interlocked.CompareExchange(
                        ref maximumObserved,
                        now,
                        snapshot
                    ) != snapshot);

                    Console.WriteLine($"worker={id} entered active={now}");
                    Thread.Sleep(100);
                    now = Interlocked.Decrement(ref active);
                    Console.WriteLine($"worker={id} leaving active={now}");
                }
                finally
                {
                    slots.Release();
                }
            });

            workers[i].Start();
        }

        foreach (Thread worker in workers)
            worker.Join();

        Console.WriteLine($"maximum active={maximumObserved}");

        if (maximumObserved > 2)
            throw new Exception("SemaphoreSlim capacity invariant failed.");
    }

    static void CasDemo()
    {
        stock = 1;
        Thread[] buyers = new Thread[2];

        for (int i = 0; i < buyers.Length; i++)
        {
            int id = i + 1;

            buyers[i] = new Thread(() =>
            {
                int original = Interlocked.CompareExchange(ref stock, 0, 1);

                Console.WriteLine(
                    original == 1
                        ? $"buyer {id}: purchase succeeded"
                        : $"buyer {id}: sold out"
                );
            });

            buyers[i].Start();
        }

        foreach (Thread buyer in buyers)
            buyer.Join();

        Console.WriteLine($"final stock={stock}");
    }

    static void SelfTest()
    {
        int expected = RunCounter(() =>
        {
            lock (ModernLock)
                counter++;
        });

        if (counter != expected)
            throw new Exception("System.Threading.Lock counter failed.");

        expected = RunCounter(() =>
        {
            lock (MonitorLock)
                counter++;
        });

        if (counter != expected)
            throw new Exception("Monitor-compatible lock counter failed.");

        expected = RunCounter(() => Interlocked.Increment(ref counter));

        if (counter != expected)
            throw new Exception("Interlocked counter failed.");

        Console.WriteLine("Chapter06 self-test PASS");
    }
}
