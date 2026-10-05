using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

class Program
{
    const int RUSAGE_SELF = 0;

    const int PROT_READ = 0x1;
    const int PROT_WRITE = 0x2;

    const int MAP_PRIVATE = 0x02;
    const int MAP_ANONYMOUS = 0x20;

    static readonly IntPtr MAP_FAILED = new(-1);

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

    [DllImport("libc", SetLastError = true)]
    static extern IntPtr mmap(
        IntPtr addr,
        nuint length,
        int prot,
        int flags,
        int fd,
        long offset
    );

    [DllImport("libc", SetLastError = true)]
    static extern int munmap(IntPtr addr, nuint length);

    [DllImport("libc", SetLastError = true)]
    static extern int mprotect(IntPtr addr, nuint len, int prot);

    static int Main(string[] args)
    {
        EnsureLinuxX64();

        string mode = args.Length == 0 ? "faults" : args[0];

        switch (mode)
        {
            case "mmap":
                FileMappingDemo();
                return 0;
            case "cow":
                FileBackedCopyOnWriteDemo();
                return 0;
            case "replacement":
                PageReplacementDemo();
                return 0;
            case "protection":
                return ProtectionParent();
            case "protection-child":
                ProtectionChild();
                return 0; // unreachable if protection works
            case "self-test":
                SelfTest();
                return 0;
            default:
                FaultDemo();
                return 0;
        }
    }

    static void EnsureLinuxX64()
    {
        if (!OperatingSystem.IsLinux() ||
            RuntimeInformation.ProcessArchitecture != Architecture.X64)
        {
            throw new PlatformNotSupportedException(
                "This lab intentionally targets Fedora/Linux x86-64."
            );
        }
    }

    static RUsage ReadUsage()
    {
        if (getrusage(RUSAGE_SELF, out RUsage usage) != 0)
            throw new InvalidOperationException(
                $"getrusage failed, errno={Marshal.GetLastPInvokeError()}"
            );

        return usage;
    }

    static void PrintFaults(string label, RUsage usage)
    {
        Console.WriteLine(
            $"{label,-20} minor={usage.MinorFaults} major={usage.MajorFaults}"
        );
    }

    static IntPtr MapAnonymous(nuint length, int protection)
    {
        IntPtr address = mmap(
            IntPtr.Zero,
            length,
            protection,
            MAP_PRIVATE | MAP_ANONYMOUS,
            -1,
            0
        );

        if (address == MAP_FAILED)
            throw new InvalidOperationException(
                $"mmap failed, errno={Marshal.GetLastPInvokeError()}"
            );

        return address;
    }

    static void Unmap(IntPtr address, nuint length)
    {
        if (munmap(address, length) != 0)
            throw new InvalidOperationException(
                $"munmap failed, errno={Marshal.GetLastPInvokeError()}"
            );
    }

    static void FaultDemo()
    {
        int pageSize = Environment.SystemPageSize;
        nuint bytes = 32u * 1024u * 1024u;

        RUsage before = ReadUsage();
        IntPtr region = MapAnonymous(bytes, PROT_READ | PROT_WRITE);

        try
        {
            RUsage afterMap = ReadUsage();

            for (nuint offset = 0; offset < bytes; offset += (nuint)pageSize)
                Marshal.WriteByte(region, checked((int)offset), 1);

            RUsage afterTouch = ReadUsage();

            PrintFaults("before mmap", before);
            PrintFaults("after mmap", afterMap);
            PrintFaults("after page touches", afterTouch);

            Console.WriteLine(
                $"region=0x{region.ToInt64():x} pageSize={pageSize} " +
                $"pages={bytes / (nuint)pageSize}"
            );

            Console.WriteLine(
                $"minor-fault delta after touch={afterTouch.MinorFaults - afterMap.MinorFaults}"
            );
        }
        finally
        {
            Unmap(region, bytes);
        }
    }

    static void FileMappingDemo()
    {
        string path = Path.Combine(
            Path.GetTempPath(),
            $"os-course-map-{Guid.NewGuid():N}.bin"
        );

        File.WriteAllBytes(path, new byte[4096]);

        try
        {
            using MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(
                path,
                FileMode.Open,
                null,
                4096,
                MemoryMappedFileAccess.ReadWrite
            );

            using MemoryMappedViewAccessor view = mmf.CreateViewAccessor(
                0,
                4096,
                MemoryMappedFileAccess.ReadWrite
            );

            view.Write(0, 123456);
            view.Flush();

            int value = BitConverter.ToInt32(File.ReadAllBytes(path), 0);
            Console.WriteLine($"shared file mapping readback={value}");

            if (value != 123456)
                throw new Exception("Memory-mapped file writeback failed.");
        }
        finally
        {
            File.Delete(path);
        }
    }

    static void FileBackedCopyOnWriteDemo()
    {
        string path = Path.Combine(
            Path.GetTempPath(),
            $"os-course-cow-{Guid.NewGuid():N}.bin"
        );

        byte[] initial = new byte[4096];
        BitConverter.GetBytes(10).CopyTo(initial, 0);
        File.WriteAllBytes(path, initial);

        try
        {
            using MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(
                path,
                FileMode.Open,
                null,
                4096,
                MemoryMappedFileAccess.CopyOnWrite
            );

            using MemoryMappedViewAccessor view = mmf.CreateViewAccessor(
                0,
                4096,
                MemoryMappedFileAccess.CopyOnWrite
            );

            int before = view.ReadInt32(0);
            view.Write(0, 99);
            int privateValue = view.ReadInt32(0);
            int fileValue = BitConverter.ToInt32(File.ReadAllBytes(path), 0);

            Console.WriteLine($"mapping before={before}");
            Console.WriteLine($"private mapping after write={privateValue}");
            Console.WriteLine($"backing file remains={fileValue}");

            if (before != 10 || privateValue != 99 || fileValue != 10)
                throw new Exception("File-backed CopyOnWrite semantics failed.");
        }
        finally
        {
            File.Delete(path);
        }
    }

    static int ProtectionParent()
    {
        string executable = Environment.ProcessPath
            ?? throw new InvalidOperationException("Process path unavailable.");

        var startInfo = new ProcessStartInfo(executable)
        {
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add("protection-child");

        using Process child = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Failed to start protection child.");

        child.WaitForExit();

        Console.WriteLine($"protection child exit code={child.ExitCode}");

        if (child.ExitCode == 0)
            throw new Exception(
                "Child unexpectedly wrote to a read-only page without failing."
            );

        Console.WriteLine(
            "Protection demo PASS: the child could not complete the forbidden write."
        );

        return 0;
    }

    static void ProtectionChild()
    {
        nuint pageSize = (nuint)Environment.SystemPageSize;
        IntPtr page = MapAnonymous(pageSize, PROT_READ | PROT_WRITE);

        Marshal.WriteByte(page, 0, 42);

        if (mprotect(page, pageSize, PROT_READ) != 0)
            throw new InvalidOperationException(
                $"mprotect failed, errno={Marshal.GetLastPInvokeError()}"
            );

        Console.WriteLine(
            $"child PID={Environment.ProcessId} mapped page=0x{page.ToInt64():x} as read-only"
        );
        Console.Out.Flush();

        // Intentionally violates page protection in the child process.
        // The parent survives and observes that this child did not exit normally.
        Marshal.WriteByte(page, 0, 99);

        // If execution reaches here, the expected protection did not occur.
        Unmap(page, pageSize);
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

    static void ValidateFrameCount(int frameCount)
    {
        if (frameCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(frameCount));
    }

    static int Fifo(IReadOnlyList<int> refs, int frameCount)
    {
        ValidateFrameCount(frameCount);

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

    static int Lru(IReadOnlyList<int> refs, int frameCount)
    {
        ValidateFrameCount(frameCount);

        HashSet<int> frames = new();
        Dictionary<int, int> last = new();
        int faults = 0;

        for (int time = 0; time < refs.Count; time++)
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

    static int Optimal(IReadOnlyList<int> refs, int frameCount)
    {
        ValidateFrameCount(frameCount);

        HashSet<int> frames = new();
        int faults = 0;

        for (int i = 0; i < refs.Count; i++)
        {
            int page = refs[i];

            if (frames.Contains(page)) continue;

            faults++;

            if (frames.Count < frameCount)
            {
                frames.Add(page);
                continue;
            }

            int NextUse(int resident)
            {
                for (int j = i + 1; j < refs.Count; j++)
                    if (refs[j] == resident)
                        return j;

                return int.MaxValue;
            }

            int victim = frames.OrderByDescending(NextUse).First();
            frames.Remove(victim);
            frames.Add(page);
        }

        return faults;
    }

    static int Clock(IReadOnlyList<int> refs, int frameCount)
    {
        ValidateFrameCount(frameCount);

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

    static void SelfTest()
    {
        int[] refs = { 7, 0, 1, 2, 0, 3, 0, 4, 2, 3, 0, 3, 2 };

        if (Fifo(refs, 3) != 10) throw new Exception("FIFO self-test failed.");
        if (Lru(refs, 3) != 9) throw new Exception("LRU self-test failed.");
        if (Optimal(refs, 3) != 7) throw new Exception("OPT self-test failed.");
        if (Clock(refs, 3) != 9) throw new Exception("CLOCK self-test failed.");

        int[] belady = { 1, 2, 3, 4, 1, 2, 5, 1, 2, 3, 4, 5 };

        if (Fifo(belady, 3) != 9 || Fifo(belady, 4) != 10)
            throw new Exception("Belady anomaly self-test failed.");

        FileBackedCopyOnWriteDemo();

        Console.WriteLine("Chapter11 self-test PASS");
    }
}
