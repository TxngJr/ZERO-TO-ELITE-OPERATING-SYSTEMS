class Program
{
    static void Main(string[] args)
    {
        string mode = args.Length == 0 ? "producer-consumer" : args[0];

        switch (mode)
        {
            case "readers-writers":
                ReadersWriters();
                break;
            case "dining":
                DiningPhilosophers();
                break;
            default:
                ProducerConsumer();
                break;
        }
    }

    static void ProducerConsumer()
    {
        const int capacity = 10;
        int[] buffer = new int[capacity];
        int front = 0;
        int back = 0;
        int count = 0;
        int producersFinished = 0;
        object bufferLock = new();

        void Enqueue(int value, int producerId)
        {
            lock (bufferLock)
            {
                while (count == buffer.Length)
                {
                    Console.WriteLine($"producer-{producerId}: queue full, waiting");
                    Monitor.Wait(bufferLock);
                }

                buffer[back] = value;
                back = (back + 1) % buffer.Length;
                count++;

                Console.WriteLine($"producer-{producerId}: enqueue {value}, count={count}");
                Monitor.PulseAll(bufferLock);
            }
        }

        bool Dequeue(out int value, int consumerId)
        {
            lock (bufferLock)
            {
                while (count == 0 && producersFinished < 2)
                {
                    Console.WriteLine($"consumer-{consumerId}: queue empty, waiting");
                    Monitor.Wait(bufferLock);
                }

                if (count == 0 && producersFinished == 2)
                {
                    value = 0;
                    return false;
                }

                value = buffer[front];
                front = (front + 1) % buffer.Length;
                count--;

                Console.WriteLine($"consumer-{consumerId}: dequeue {value}, count={count}");
                Monitor.PulseAll(bufferLock);
                return true;
            }
        }

        void ProducerDone()
        {
            lock (bufferLock)
            {
                producersFinished++;
                Monitor.PulseAll(bufferLock);
            }
        }

        Thread p1 = new Thread(() =>
        {
            for (int i = 1; i <= 30; i++)
            {
                Enqueue(i, 1);
                Thread.Sleep(5);
            }
            ProducerDone();
        });

        Thread p2 = new Thread(() =>
        {
            for (int i = 100; i <= 130; i++)
            {
                Enqueue(i, 2);
                Thread.Sleep(7);
            }
            ProducerDone();
        });

        Thread[] consumers = new Thread[3];
        for (int i = 0; i < consumers.Length; i++)
        {
            int id = i + 1;
            consumers[i] = new Thread(() =>
            {
                while (Dequeue(out int value, id))
                    Thread.Sleep(16);
            });
        }

        p1.Start();
        p2.Start();
        foreach (Thread c in consumers) c.Start();

        p1.Join();
        p2.Join();
        foreach (Thread c in consumers) c.Join();

        Console.WriteLine($"final count={count}");
    }

    static void ReadersWriters()
    {
        using ReaderWriterLockSlim rw = new();
        int sharedValue = 0;

        Thread[] readers = new Thread[4];
        for (int i = 0; i < readers.Length; i++)
        {
            int id = i + 1;
            readers[i] = new Thread(() =>
            {
                for (int round = 0; round < 4; round++)
                {
                    rw.EnterReadLock();
                    try
                    {
                        Console.WriteLine($"reader-{id}: {sharedValue}");
                        Thread.Sleep(20);
                    }
                    finally
                    {
                        rw.ExitReadLock();
                    }
                }
            });
        }

        Thread[] writers = new Thread[2];
        for (int i = 0; i < writers.Length; i++)
        {
            int id = i + 1;
            writers[i] = new Thread(() =>
            {
                for (int round = 0; round < 4; round++)
                {
                    rw.EnterWriteLock();
                    try
                    {
                        sharedValue++;
                        Console.WriteLine($"writer-{id}: set {sharedValue}");
                    }
                    finally
                    {
                        rw.ExitWriteLock();
                    }
                    Thread.Sleep(30);
                }
            });
        }

        foreach (Thread r in readers) r.Start();
        foreach (Thread w in writers) w.Start();
        foreach (Thread r in readers) r.Join();
        foreach (Thread w in writers) w.Join();

        Console.WriteLine($"final={sharedValue}");
    }

    static void DiningPhilosophers()
    {
        const int n = 5;
        object[] forks = Enumerable.Range(0, n).Select(_ => new object()).ToArray();
        Thread[] philosophers = new Thread[n];

        for (int i = 0; i < n; i++)
        {
            int id = i;
            philosophers[i] = new Thread(() =>
            {
                int left = id;
                int right = (id + 1) % n;

                object first = forks[Math.Min(left, right)];
                object second = forks[Math.Max(left, right)];

                for (int round = 0; round < 3; round++)
                {
                    lock (first)
                    {
                        lock (second)
                        {
                            Console.WriteLine($"P{id} eating round {round + 1}");
                            Thread.Sleep(25);
                        }
                    }
                }
            });
            philosophers[i].Start();
        }

        foreach (Thread p in philosophers) p.Join();
        Console.WriteLine("completed without circular-wait deadlock");
    }
}
