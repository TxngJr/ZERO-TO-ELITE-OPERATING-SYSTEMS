class Program
{
    static int stock = 1;
    static readonly Barrier barrier = new(2);
    static int payload;
    static int ready;

    static void Main(string[] args)
    {
        string mode = args.Length == 0 ? "check-then-act" : args[0];

        switch (mode)
        {
            case "visibility":
                VisibilityDemo();
                break;
            case "background":
                BackgroundDemo();
                break;
            default:
                CheckThenActDemo();
                break;
        }
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

    static void BackgroundDemo()
    {
        Thread worker = new Thread(() =>
        {
            Console.WriteLine("background worker started");
            Thread.Sleep(300);
            Console.WriteLine("background worker finished");
        })
        {
            IsBackground = true
        };

        worker.Start();
        worker.Join();
    }
}
