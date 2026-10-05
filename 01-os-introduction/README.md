# Chapter 01 — OS Introduction with C#

## Goals

เข้าใจ OS, Kernel, User Space, Kernel Space, System Call, Hardware–OS–Application และตำแหน่งของ .NET runtime

## Big Picture

~~~text
C# Application
↓
.NET BCL / CoreCLR
↓
Linux syscall interface
↓
Kernel
↓
Drivers
↓
Hardware
~~~

## C# Library Call vs System Call

~~~csharp
Console.WriteLine("Hello");
~~~

Console.WriteLine ไม่ใช่ Linux syscall ชื่อเดียวกัน แต่ผ่าน .NET runtime ก่อน

สังเกต:

~~~bash
strace -e trace=write dotnet run --project 01-os-introduction/examples/Chapter01.csproj
~~~

## Native Boundary from C#

ตัวอย่างใช้ P/Invoke เรียก libc write:

~~~csharp
[DllImport("libc")]
static extern nint write(...);
~~~

run:

~~~bash
dotnet run --project 01-os-introduction/examples/Chapter01.csproj -- raw-write
~~~

## Fedora Observation

~~~bash
uname -a
cat /etc/os-release
lscpu
free -h
lsblk
cat /proc/$$/status
~~~

## C# Exercises

1. เพิ่ม mode info ให้ print ProcessId, ProcessorCount และ SystemPageSize
2. ใช้ strace เปรียบเทียบ hello กับ raw-write
3. อธิบายว่าทำไม Console.WriteLine 1 ครั้งไม่รับประกัน write syscall 1 ครั้ง
4. เขียน C# อ่าน /proc/self/status แล้วแสดง Name, State, Threads

## Quiz

1. C# program รันใน user space หรือ kernel space
2. Console.WriteLine เป็น syscall โดยตรงหรือไม่
3. Fedora คือ kernel หรือ distribution
4. P/Invoke ใช้ทำอะไร

## Explain-It-Back

~~~text
C# source
→ .NET runtime
→ syscall boundary
→ kernel
→ hardware
~~~
