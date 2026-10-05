using System.Diagnostics;
using System.Runtime.InteropServices;

class Program
{
    static void Main()
    {
        using Process process = Process.GetCurrentProcess();

        Console.WriteLine($"PID={Environment.ProcessId}");
        Console.WriteLine($"ProcessName={process.ProcessName}");
        Console.WriteLine($"Threads={process.Threads.Count}");
        Console.WriteLine($"WorkingSet={process.WorkingSet64} bytes");
        Console.WriteLine($"PrivateMemory={process.PrivateMemorySize64} bytes");
        Console.WriteLine($"ManagedThreadId={Environment.CurrentManagedThreadId}");

        IntPtr unmanaged = Marshal.AllocHGlobal(4096);
        try
        {
            Console.WriteLine($"Unmanaged allocation address=0x{unmanaged.ToInt64():x}");
            Console.WriteLine("Inspect /proc/<PID>/maps now. Sleeping 15 seconds...");
            Thread.Sleep(15000);
        }
        finally
        {
            Marshal.FreeHGlobal(unmanaged);
        }
    }
}
