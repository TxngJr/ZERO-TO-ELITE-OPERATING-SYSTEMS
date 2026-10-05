using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

class Program
{
    [StructLayout(LayoutKind.Sequential)]
    struct TimeVal
    {
        public long Sec;
        public long Usec;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct RUsage
    {
        public TimeVal UserTime;
        public TimeVal SystemTime;
        public long MaxRss;
        public long IxRss;
        public long IdRss;
        public long IsRss;
        public long MinorFaults;
        public long MajorFaults;
        public long Swap;
        public long InBlock;
        public long OutBlock;
        public long MsgSend;
        public long MsgReceive;
        public long Signals;
        public long VoluntaryContextSwitches;
        public long InvoluntaryContextSwitches;
    }

    [DllImport("libc", SetLastError = true)]
    static extern int getrusage(int who, out RUsage usage);

    const int RUSAGE_SELF = 0;

    static void Main(string[] args)
    {
        string mode = args.Length == 0 ? "faults" : args[0];

        switch (mode)
        {
            case "mmap":
                MmapDemo();
                break;
            case "cow":
                CopyOnWriteDemo();
                break;
            case "replacement":
                PageReplacementDemo();
                break;
            default:
                FaultDemo();
                break;
        }
    }

    static void PrintFaults(string label)
    {
        if (getrusage(RUSAGE_SELF, out RUsage usage) != 0)
            throw new InvalidOperationException("getrusage failed");

        Console.WriteLine($"{label,-18} minor={usage.MinorFaults} major={usage.MajorFaults}");
    }

    static void FaultDemo()
    {
        int pageSize = Environment.SystemPageSize;
        int bytes = 32 * 1024 * 1024;

        PrintFaults("before alloc");

        IntPtr memory = Marshal.AllocHGlobal(bytes);
        try
        {
            PrintFaults("after alloc");

            for (int offset = 0; offset < bytes; offset += pageSize)
                Marshal.WriteByte(memory, offset, 1);

            PrintFaults("after touch");
            Console.WriteLine($"pageSize={pageSize} pages={bytes / pageSize}");
        }
        finally
        {
            Marshal.FreeHGlobal(memory);
        }
    }

    static void MmapDemo()
    {
        string path = Path.Combine(Path.GetTempPath(), $"os-course-{Guid.NewGuid():N}.bin");
        File.WriteAllBytes(path, new byte[4096]);

        try
        {
            using MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(
                path,
                FileMode.Open,
                null,
                4096,
                MemoryMappedFileAccess.ReadWrite);

            using MemoryMappedViewAccessor view = mmf.CreateViewAccessor(
                0,
                4096,
                MemoryMappedFileAccess.ReadWrite);

            view.Write(0, 123456);
            view.Flush();

            byte[] data = File.ReadAllBytes(path);
            int value = BitConverter.ToInt32(data, 0);
            Console.WriteLine($"file readback={value}");
        }
        finally
        {
            File.Delete(path);
        }
    }

    static void CopyOnWriteDemo()
    {
        string path = Path.Combine(Path.GetTempPath(), $"os-course-cow-{Guid.NewGuid():N}.bin");
        File.WriteAllBytes(path, BitConverter.GetBytes(10).Concat(new byte[4092]).ToArray());

        try
        {
            using MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(
                path,
                FileMode.Open,
                null,
                4096,
                MemoryMappedFileAccess.CopyOnWrite);

            using MemoryMappedViewAccessor view = mmf.CreateViewAccessor(
                0,
                4096,
                MemoryMappedFileAccess.CopyOnWrite);

            Console.WriteLine($"mapped before={view.ReadInt32(0)}");
            view.Write(0, 99);
            Console.WriteLine($"mapped after={view.ReadInt32(0)}");

            int fileValue = BitConverter.ToInt32(File.ReadAllBytes(path), 0);
            Console.WriteLine($"file still={fileValue}");
        }
        finally
        {
            File.Delete(path);
        }
    }

    static void PageReplacementDemo()
    {
        int[] refs = { 7, 0, 1, 2, 0, 3, 0, 4, 2, 3, 0, 3, 2 };

        Console.WriteLine($"FIFO={Fifo(refs, 3)}");
        Console.WriteLine($"LRU={Lru(refs, 3)}");
        Console.WriteLine($"OPT={Optimal(refs, 3)}");
        Console.WriteLine($"CLOCK={Clock(refs, 3)}");

        int[] belady = { 1, 2, 3, 4, 1, 2, 5, 1, 2, 3, 4, 5 };
        Console.WriteLine($"Belady FIFO frames=3 faults={Fifo(belady, 3)}");
        Console.WriteLine($"Belady FIFO frames=4 faults={Fifo(belady, 4)}");
    }

    static int Fifo(int[] refs, int frameCount)
    {
        HashSet<int> frames = new();
        Queue<int> order = new();
        int faults = 0;

        foreach (int page in refs)
        {
            if (frames.Contains(page)) continue;

            faults++;
            if (frames.Count == frameCount)
                frames.Remove(order.Dequeue());

            frames.Add(page);
            order.Enqueue(page);
        }

        return faults;
    }

    static int Lru(int[] refs, int frameCount)
    {
        HashSet<int> frames = new();
        Dictionary<int, int> last = new();
        int faults = 0;

        for (int time = 0; time < refs.Length; time++)
        {
            int page = refs[time];

            if (!frames.Contains(page))
            {
                faults++;

                if (frames.Count == frameCount)
                {
                    int victim = frames.OrderBy(p => last[p]).First();
                    frames.Remove(victim);
                    last.Remove(victim);
                }

                frames.Add(page);
            }

            last[page] = time;
        }

        return faults;
    }

    static int Optimal(int[] refs, int frameCount)
    {
        HashSet<int> frames = new();
        int faults = 0;

        for (int i = 0; i < refs.Length; i++)
        {
            int page = refs[i];
            if (frames.Contains(page)) continue;

            faults++;

            if (frames.Count < frameCount)
            {
                frames.Add(page);
                continue;
            }

            int victim = frames
                .OrderByDescending(p =>
                {
                    int next = Array.IndexOf(refs, p, i + 1);
                    return next < 0 ? int.MaxValue : next;
                })
                .First();

            frames.Remove(victim);
            frames.Add(page);
        }

        return faults;
    }

    static int Clock(int[] refs, int frameCount)
    {
        int?[] frames = new int?[frameCount];
        int[] referenced = new int[frameCount];
        int hand = 0;
        int faults = 0;

        foreach (int page in refs)
        {
            int hit = Array.FindIndex(frames, p => p == page);
            if (hit >= 0)
            {
                referenced[hit] = 1;
                continue;
            }

            faults++;

            while (frames[hand].HasValue && referenced[hand] == 1)
            {
                referenced[hand] = 0;
                hand = (hand + 1) % frameCount;
            }

            frames[hand] = page;
            referenced[hand] = 1;
            hand = (hand + 1) % frameCount;
        }

        return faults;
    }
}
