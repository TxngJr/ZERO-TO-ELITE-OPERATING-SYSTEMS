class Program
{
    static readonly object lockObj = new();
    static int counter;
    static int stock = 1;

    static void Main(string[] args)
    {
        string mode = args.Length == 0 ? "lock-counter" : args[0];

        switch (mode)
        {
            case "semaphore":
                SemaphoreDemo();
                break;
            case "interlocked":
                InterlockedDemo();
                break;
            case "cas":
                CasDemo();
                break;
            default:
                LockCounterDemo();
                break;
        }
    }

    static void LockCounterDemo()
    {
        counter = 0;
        const int iterations = 200_000;

        Thread[] threads = new Thread[4];
        for (int i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(() =>
            {
                for (int j = 0; j < iterations; j++)
                {
                    lock (lockObj)
                    {
                        counter++;
                    }
                }
            });
            threads[i].Start();
        }

        foreach (Thread thread in threads)
            thread.Join();

        Console.WriteLine($"expected={iterations * threads.Length} observed={counter}");
    }

    static void InterlockedDemo()
    {
        counter = 0;
        Thread[] threads = new Thread[4];

        for (int i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(() =>
            {
                for (int j = 0; j < 200_000; j++)
                    Interlocked.Increment(ref counter);
            });
            threads[i].Start();
        }

        foreach (Thread thread in threads)
            thread.Join();

        Console.WriteLine($"counter={counter}");
    }

    static void SemaphoreDemo()
    {
        using SemaphoreSlim slots = new(2, 2);
        int active = 0;

        Thread[] workers = new Thread[6];
        for (int i = 0; i < workers.Length; i++)
        {
            int id = i + 1;
            workers[i] = new Thread(() =>
            {
                slots.Wait();
                int now = Interlocked.Increment(ref active);
                Console.WriteLine($"worker={id} entered active={now}");
                Thread.Sleep(200);
                now = Interlocked.Decrement(ref active);
                Console.WriteLine($"worker={id} leaving active={now}");
                slots.Release();
            });
            workers[i].Start();
        }

        foreach (Thread worker in workers)
            worker.Join();
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
                Console.WriteLine(original == 1
                    ? $"buyer {id}: purchase succeeded"
                    : $"buyer {id}: sold out");
            });
            buyers[i].Start();
        }

        foreach (Thread buyer in buyers)
            buyer.Join();

        Console.WriteLine($"final stock={stock}");
    }
}
