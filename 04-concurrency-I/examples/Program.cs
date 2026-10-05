using System.Runtime.InteropServices;

class Program
{
    static int counter;

    [DllImport("libc")]
    static extern int gettid();

    static void Main(string[] args)
    {
        string mode = args.Length == 0 ? "basic" : args[0];

        if (mode == "race")
        {
            RaceDemo();
            return;
        }

        if (mode == "observe")
        {
            ObserveDemo();
            return;
        }

        BasicDemo();
    }

    static void BasicDemo()
    {
        Thread[] threads = new Thread[4];

        for (int i = 0; i < threads.Length; i++)
        {
            int id = i + 1;
            threads[i] = new Thread(() =>
            {
                Console.WriteLine(
                    $"worker={id} PID={Environment.ProcessId} ManagedId={Environment.CurrentManagedThreadId} TID={gettid()}"
                );
            });
            threads[i].Start();
        }

        foreach (Thread thread in threads)
            thread.Join();
    }

    static void RaceDemo()
    {
        counter = 0;
        const int iterations = 500_000;

        Thread a = new Thread(() =>
        {
            for (int i = 0; i < iterations; i++)
                counter++;
        });

        Thread b = new Thread(() =>
        {
            for (int i = 0; i < iterations; i++)
                counter++;
        });

        a.Start();
        b.Start();
        a.Join();
        b.Join();

        Console.WriteLine($"expected={iterations * 2} observed={counter}");
    }

    static void ObserveDemo()
    {
        Thread[] threads = new Thread[4];

        for (int i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(() => Thread.Sleep(15_000));
            threads[i].Start();
        }

        Console.WriteLine($"PID={Environment.ProcessId}");
        Console.WriteLine("Inspect with: ps -L -p PID -o pid,tid,psr,stat,comm");

        foreach (Thread thread in threads)
            thread.Join();
    }
}
