# Batch 01 Review — C# Edition

## Chapters 01–03

ต้องอธิบาย:

- OS vs Kernel
- User Space vs Kernel Space
- System Call
- C#/.NET runtime layer
- Program vs Process
- PID / PPID
- Process State
- CPU Context
- Context Switch
- fork/exec/wait concept
- Process.Start / WaitForExit
- Zombie concept

## C# Practical Gate

1. Trace Console.WriteLine ด้วย strace
2. อ่าน /proc/self/status ด้วย C#
3. ใช้ Process.GetCurrentProcess
4. ใช้ Marshal.AllocHGlobal และ inspect maps
5. สร้าง child ด้วย Process.Start
6. รอ child ด้วย WaitForExit
7. อ่าน ExitCode

## Code Reading

อธิบาย output และ lifecycle ของ:

~~~csharp
using Process child = Process.Start(info)!;
Console.WriteLine(child.Id);
child.WaitForExit();
Console.WriteLine(child.ExitCode);
~~~

## Important

Process.Start เป็น high-level .NET API ไม่ใช่ชื่อ syscall
