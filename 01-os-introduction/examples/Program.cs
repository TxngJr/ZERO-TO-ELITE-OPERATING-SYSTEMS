using System.Runtime.InteropServices;
using System.Text;

class Program
{
    [DllImport("libc", SetLastError = true)]
    private static extern nint write(int fd, byte[] buffer, nuint count);

    static void Main(string[] args)
    {
        string mode = args.Length == 0 ? "hello" : args[0];

        if (mode == "raw-write")
        {
            byte[] bytes = Encoding.UTF8.GetBytes("Hello through libc write from C#\n");
            nint written = write(1, bytes, (nuint)bytes.Length);
            Console.Error.WriteLine($"write returned {written}");
            return;
        }

        Console.WriteLine("Hello from C# user space");
        Console.WriteLine($"PID={Environment.ProcessId}");
        Console.WriteLine($".NET={Environment.Version}");
    }
}
