using System.Diagnostics;

class Program
{
    static int Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "child")
        {
            Console.WriteLine($"child PID={Environment.ProcessId}");
            Thread.Sleep(500);
            return 42;
        }

        string executable = Environment.ProcessPath
            ?? throw new InvalidOperationException("Process path unavailable");

        var startInfo = new ProcessStartInfo(executable)
        {
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add("child");

        using Process child = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Failed to start child");

        Console.WriteLine($"parent PID={Environment.ProcessId}");
        Console.WriteLine($"created child PID={child.Id}");

        child.WaitForExit();

        Console.WriteLine($"child exit code={child.ExitCode}");
        return 0;
    }
}
