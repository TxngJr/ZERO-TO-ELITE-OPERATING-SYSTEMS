using System.Globalization;
using System.Runtime.InteropServices;

record Pte(ulong Pfn, bool Writable);

class Program
{
    static readonly Dictionary<ulong, Pte> PageTable = new()
    {
        [0x10] = new Pte(0x7A, false),
        [0x11] = new Pte(0x22, true),
        [0x12] = new Pte(0x9F, true)
    };

    static void Main(string[] args)
    {
        string mode = args.Length == 0 ? "addresses" : args[0];

        switch (mode)
        {
            case "split":
                if (args.Length < 2) throw new ArgumentException("split requires an address");
                Split(ParseAddress(args[1]));
                break;
            case "table":
                TableDemo();
                break;
            default:
                AddressDemo();
                break;
        }
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
            Console.WriteLine($"page size={pageSize}");
            Console.WriteLine($"unmanaged VA=0x{address:x}");
            Split(address);
            Console.WriteLine($"PID={Environment.ProcessId}");
            Console.WriteLine("Inspect /proc/PID/maps from another terminal.");
            Thread.Sleep(10000);
        }
        finally
        {
            Marshal.FreeHGlobal(memory);
        }
    }

    static void Split(ulong virtualAddress)
    {
        ulong pageSize = (ulong)Environment.SystemPageSize;
        ulong vpn = virtualAddress / pageSize;
        ulong offset = virtualAddress % pageSize;

        Console.WriteLine($"VA=0x{virtualAddress:x}");
        Console.WriteLine($"pageSize=0x{pageSize:x}");
        Console.WriteLine($"VPN=0x{vpn:x}");
        Console.WriteLine($"offset=0x{offset:x}");
    }

    static void Translate(ulong virtualAddress, bool write)
    {
        ulong pageSize = (ulong)Environment.SystemPageSize;
        ulong vpn = virtualAddress / pageSize;
        ulong offset = virtualAddress % pageSize;

        Console.WriteLine($"VA=0x{virtualAddress:x} VPN=0x{vpn:x} offset=0x{offset:x}");

        if (!PageTable.TryGetValue(vpn, out Pte? pte))
        {
            Console.WriteLine("translation fault: unmapped VPN");
            return;
        }

        if (write && !pte.Writable)
        {
            Console.WriteLine("protection fault: page is read-only");
            return;
        }

        ulong physicalAddress = pte.Pfn * pageSize + offset;
        Console.WriteLine($"PFN=0x{pte.Pfn:x} PA=0x{physicalAddress:x}");
    }

    static void TableDemo()
    {
        Translate(0x10020, false);
        Translate(0x11ABC, true);
        Translate(0x12FED, false);
        Translate(0x13000, false);
    }
}
