class Program
{
    static int stock = 1;
    static readonly Barrier barrier = new(2);

    static int payload;
    static int ready;

    static int Main(string[] args)
    {
        string mode = args.Length == 0 ? "check-then-act" : args[0];

        switch (mode)
        {
            case "visibility":
                VisibilityDemo();
                break;
            case "background-nojoin":
                BackgroundNoJoinDemo();
                break;
            case "foreground-nojoin":
                ForegroundNoJoinDemo();
                break;
            case "background-join":
                BackgroundJoinDemo();
                break;
            default:
                CheckThenActDemo();
                break;
        }

        return 0;
    }

    static void CheckThenActDemo()
    {
        stock = 1;

        Thread[] buyers = new Thread[2];

        for (int i = 0; i < buyers.Length; i++)
        {
            int id = i + 1;

            buyers[i] = new Thread(() =>
            {
                int observed = Volatile.Read(ref stock);
                Console.WriteLine($"buyer {id} checked stock={observed}");

                barrier.SignalAndWait();

                if (observed > 0)
                {
                    int after = Interlocked.Decrement(ref stock);
                    Console.WriteLine($"buyer {id} acted; stock={after}");
                }
            });

            buyers[i].Start();
        }

        foreach (Thread buyer in buyers)
            buyer.Join();

        Console.WriteLine($"final stock={stock}");
        Console.WriteLine(
            "This demonstrates that an atomic decrement does not make the earlier check part of the same atomic transaction."
        );
    }

    static void VisibilityDemo()
    {
        payload = 0;
        ready = 0;

        Thread reader = new Thread(() =>
        {
            while (Volatile.Read(ref ready) == 0)
                Thread.Yield();

            Console.WriteLine($"payload={payload}");
        });

        Thread writer = new Thread(() =>
        {
            payload = 42;
            Volatile.Write(ref ready, 1);
        });

        reader.Start();
        writer.Start();

        reader.Join();
        writer.Join();
    }

    static Thread CreateLifetimeWorker(bool background)
    {
        return new Thread(() =>
        {
            Console.WriteLine(
                $"worker started: IsBackground={Thread.CurrentThread.IsBackground}"
            );

            Thread.Sleep(500);
            Console.WriteLine("worker finished");
        })
        {
            IsBackground = background
        };
    }

    static void BackgroundNoJoinDemo()
    {
        Thread worker = CreateLifetimeWorker(background: true);
        worker.Start();

        Console.WriteLine(
            "Main returns without Join. The runtime does not keep the process alive only for background threads."
        );
    }

    static void ForegroundNoJoinDemo()
    {
        Thread worker = CreateLifetimeWorker(background: false);
        worker.Start();

        Console.WriteLine(
            "Main returns without Join, but the foreground thread keeps the process alive until it finishes."
        );
    }

    static void BackgroundJoinDemo()
    {
        Thread worker = CreateLifetimeWorker(background: true);
        worker.Start();
        worker.Join();

        Console.WriteLine(
            "Join explicitly waits, so the background worker completes before Main exits."
        );
    }
}
