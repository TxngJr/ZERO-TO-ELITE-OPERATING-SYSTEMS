# Zero to Elite Operating Systems — C# Edition

หลักสูตร Operating Systems บน **Fedora Linux + C#/.NET** จาก Absolute Zero ไปถึง Core OS Foundations โดยใช้รูปแบบการเขียนโปรแกรมให้สอดคล้องกับงานที่เรียนจริง เช่น:

~~~csharp
using System.Threading;

Thread t = new Thread(...);
t.Start();
t.Join();

lock (lockObj)
{
    Monitor.Wait(lockObj);
    Monitor.PulseAll(lockObj);
}
~~~

## Core Roadmap

1. ✅ OS Introduction
2. ✅ Process & Context Switch I
3. ✅ Process & Context Switch II
4. ✅ Concurrency I
5. ✅ Concurrency II
6. ✅ Synchronization I
7. ✅ Synchronization II
8. ✅ Synchronization III
9. ✅ Scheduling
10. ✅ Address Translation
11. ✅ Virtual Memory

## Language Rule

**ทุก source-code example, programming lab, simulator และ programming exercise ใช้ C#**

Linux commands เช่น ps, strace, pmap และ vmstat ยังคงใช้ตามจริง เพราะเป็นเครื่องมือสังเกต OS ไม่ใช่ภาษาโปรแกรมหลักของคอร์ส

## Start Here

1. [COURSE-GUIDE.md](./COURSE-GUIDE.md)
2. [SETUP-FEDORA.md](./SETUP-FEDORA.md)
3. [STUDY-ORDER.md](./STUDY-ORDER.md)

## C# Concepts Used Across the Course

~~~text
Thread
Thread.Start
Thread.Join
Thread.Sleep
lock
Monitor.Wait
Monitor.Pulse / PulseAll
Interlocked
Volatile
Mutex
SemaphoreSlim
ReaderWriterLockSlim
Process / ProcessStartInfo
MemoryMappedFile
Marshal / IntPtr
P/Invoke
~~~

## Build All C# Projects

~~~bash
./build.sh
~~~

## Run a Chapter

~~~bash
dotnet run --project 04-concurrency-I/examples/Chapter04.csproj -- race
dotnet run --project 07-synchronization-II/examples/Chapter07.csproj -- producer-consumer
dotnet run --project 09-scheduling/examples/Chapter09.csproj -- rr 2
~~~

## Managed-Runtime Mental Model

~~~text
C# Source
↓
.NET Compiler / IL
↓
CLR / CoreCLR
↓
Managed Thread / GC / BCL
↓
Linux System Calls
↓
Kernel
↓
Hardware
~~~

ดังนั้นคอร์สจะสอนทั้ง OS concept, Linux behavior, C# abstraction และจุดที่ abstraction ไม่ใช่ syscall แบบ 1:1

## Reviews

- [Midterm Review 01–05](./MIDTERM-REVIEW-01-05.md)
- [Final Review](./FINAL-REVIEW.md)
- [Capstone](./capstone/README.md)
- [Full Coverage Audit](./FULL-COVERAGE-AUDIT.md)
