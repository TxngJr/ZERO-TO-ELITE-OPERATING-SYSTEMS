using System.Globalization;
using System.Runtime.InteropServices;

record Pte(ulong Pfn, bool Readable, bool Writable, bool Executable);

sealed class Tlb
{
    private readonly int capacity;
    private readonly Dictionary<ulong, (ulong Pfn, LinkedListNode<ulong> Node)> entries = new();
    private readonly LinkedList<ulong> lru = new();

    public int Hits { get; private set; }
    public int Misses { get; private set; }

    public Tlb(int capacity)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
        this.capacity = capacity;
    }

    public bool TryLookup(ulong vpn, out ulong pfn)
    {
        if (entries.TryGetValue(vpn, out var entry))
        {
            Hits++;
            lru.Remove(entry.Node);
            lru.AddFirst(entry.Node);
            pfn = entry.Pfn;
            return true;
        }

        Misses++;
        pfn = 0;
        return false;
    }

    public void Insert(ulong vpn, ulong pfn)
    {
        if (entries.TryGetValue(vpn, out var existing))
        {
            lru.Remove(existing.Node);
            LinkedListNode<ulong> node = lru.AddFirst(vpn);
            entries[vpn] = (pfn, node);
            return;
        }

        if (entries.Count == capacity)
        {
            ulong victim = lru.Last!.Value;
            entries.Remove(victim);
            lru.RemoveLast();
        }

        LinkedListNode<ulong> newNode = lru.AddFirst(vpn);
        entries[vpn] = (pfn, newNode);
    }
}

class Program
{
    // This is the page size of the *teaching simulator*.
    // It is intentionally separate from Environment.SystemPageSize.
    const ulong SimulatedPageSize = 4096;

    static readonly Dictionary<ulong, Pte> PageTable = new()
    {
        [0x10] = new Pte(0x7A, true, false, true),
        [0x11] = new Pte(0x22, true, true, false),
        [0x12] = new Pte(0x9F, true, true, false)
    };

    static int Main(string[] args)
    {
        string mode = args.Length == 0 ? "addresses" : args[0];

        switch (mode)
        {
            case "split":
                if (args.Length < 2) throw new ArgumentException("split requires an address");
                SplitHostAddress(ParseAddress(args[1]));
                break;
            case "table":
                TableDemo();
                break;
            case "tlb":
                TlbDemo();
                break;
            case "self-test":
                SelfTest();
                break;
            default:
                AddressDemo();
                break;
        }

        return 0;
    }

    static ulong ParseAddress(string text)
    {
        if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            return ulong.Parse(text[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture);

        return ulong.Parse(text, CultureInfo.InvariantCulture);
    }

    static void AddressDemo()
    {
        int pageSize = Environment.SystemPageSize;
        IntPtr memory = Marshal.AllocHGlobal(pageSize);

        try
        {
            ulong address = unchecked((ulong)memory.ToInt64());
            Console.WriteLine($"host page size={pageSize}");
            Console.WriteLine($"unmanaged VA=0x{address:x}");
            SplitHostAddress(address);
            Console.WriteLine($"PID={Environment.ProcessId}");
            Console.WriteLine("Inspect /proc/PID/maps from another terminal.");
            Thread.Sleep(3000);
        }
        finally
        {
            Marshal.FreeHGlobal(memory);
        }
    }

    static (ulong Vpn, ulong Offset) Split(ulong virtualAddress, ulong pageSize)
    {
        if (pageSize == 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
        return (virtualAddress / pageSize, virtualAddress % pageSize);
    }

    static void SplitHostAddress(ulong virtualAddress)
    {
        ulong pageSize = (ulong)Environment.SystemPageSize;
        var (vpn, offset) = Split(virtualAddress, pageSize);

        Console.WriteLine($"VA=0x{virtualAddress:x}");
        Console.WriteLine($"host pageSize=0x{pageSize:x}");
        Console.WriteLine($"VPN=0x{vpn:x}");
        Console.WriteLine($"offset=0x{offset:x}");
    }

    static (bool Success, ulong PhysicalAddress, string Reason) Translate(
        ulong virtualAddress,
        bool write,
        bool execute)
    {
        var (vpn, offset) = Split(virtualAddress, SimulatedPageSize);

        if (!PageTable.TryGetValue(vpn, out Pte? pte))
            return (false, 0, "unmapped");

        if (!pte.Readable)
            return (false, 0, "not-readable");

        if (write && !pte.Writable)
            return (false, 0, "write-protection");

        if (execute && !pte.Executable)
            return (false, 0, "execute-protection");

        ulong physicalAddress = pte.Pfn * SimulatedPageSize + offset;
        return (true, physicalAddress, "ok");
    }

    static void PrintTranslation(ulong va, bool write = false, bool execute = false)
    {
        var (vpn, offset) = Split(va, SimulatedPageSize);
        var result = Translate(va, write, execute);

        Console.WriteLine(
            $"SIM VA=0x{va:x} VPN=0x{vpn:x} offset=0x{offset:x} " +
            (result.Success ? $"PA=0x{result.PhysicalAddress:x}" : $"FAULT={result.Reason}")
        );
    }

    static void TableDemo()
    {
        Console.WriteLine($"simulated page size={SimulatedPageSize} bytes");
        PrintTranslation(0x10 * SimulatedPageSize + 0x20);
        PrintTranslation(0x10 * SimulatedPageSize + 0x20, write: true);
        PrintTranslation(0x11 * SimulatedPageSize + 0xABC, write: true);
        PrintTranslation(0x12 * SimulatedPageSize + 0xFED, execute: true);
        PrintTranslation(0x13 * SimulatedPageSize);
    }

    static void TlbDemo()
    {
        Tlb tlb = new(2);
        ulong[] refs = { 0x10, 0x11, 0x10, 0x12, 0x11 };

        foreach (ulong vpn in refs)
        {
            if (tlb.TryLookup(vpn, out ulong pfn))
            {
                Console.WriteLine($"VPN 0x{vpn:x}: TLB HIT -> PFN 0x{pfn:x}");
                continue;
            }

            Console.WriteLine($"VPN 0x{vpn:x}: TLB MISS");

            if (PageTable.TryGetValue(vpn, out Pte? pte))
                tlb.Insert(vpn, pte.Pfn);
        }

        Console.WriteLine($"hits={tlb.Hits} misses={tlb.Misses}");
    }

    static void SelfTest()
    {
        if (Split(0x12345, 4096) != (0x12UL, 0x345UL))
            throw new Exception("Address split failed.");

        var ok = Translate(0x11 * SimulatedPageSize + 0xABC, write: true, execute: false);
        if (!ok.Success || ok.PhysicalAddress != 0x22 * SimulatedPageSize + 0xABC)
            throw new Exception("Translation failed.");

        var denied = Translate(0x10 * SimulatedPageSize, write: true, execute: false);
        if (denied.Success || denied.Reason != "write-protection")
            throw new Exception("Protection check failed.");

        Tlb tlb = new(2);
        tlb.Insert(1, 10);
        if (!tlb.TryLookup(1, out ulong pfn) || pfn != 10)
            throw new Exception("TLB lookup failed.");

        Console.WriteLine("Chapter10 self-test PASS");
    }
}
