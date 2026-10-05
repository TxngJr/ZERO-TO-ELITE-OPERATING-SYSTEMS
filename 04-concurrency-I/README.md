# Chapter 04 — Concurrency Part I with C#

## Goals

เข้าใจ Thread, Concurrency, Parallelism, Interleaving, Race Condition, Critical Section และ Thread lifecycle

## Thread in C#

รูปแบบหลักตรงกับงานเรียน:

~~~csharp
Thread t = new Thread(Work);
t.Start();
t.Join();
~~~

Thread.Start ทำให้ thread พร้อม execute

Thread.Join ทำให้ caller รอ thread จบ

## Process vs Thread

threads ใน process เดียวกัน share:

- process memory
- static fields
- heap objects
- open resources หลายชนิด

แต่แต่ละ thread มี:

- execution state
- stack
- scheduling state

## Concurrency vs Parallelism

Concurrency = หลาย execution flows มี progress ทับซ้อน/interleave

Parallelism = execute พร้อมกันจริงบนหลาย logical CPUs

Thread ไม่เท่ากับ CPU core

## Race Condition in C#

~~~csharp
counter++;
~~~

ไม่ได้ atomic เพียงเพราะเป็น C# statement หนึ่งบรรทัด

สอง threads สามารถเกิด lost update ได้

run:

~~~bash
dotnet run --project 04-concurrency-I/examples/Chapter04.csproj -- race
~~~

ต่างจาก C language data race ที่มี undefined-behavior concerns บางแบบ, C#/.NET มี memory model ของตนเอง แต่ unsynchronized shared-state code ก็ยัง incorrect และ visibility/order ต้องพิจารณา

## Thread IDs

C# มี Managed Thread ID:

~~~csharp
Environment.CurrentManagedThreadId
~~~

Lab ยังเรียก Linux gettid ผ่าน P/Invoke เพื่อเปรียบเทียบกับ OS TID

## Observe Linux Threads

~~~bash
dotnet run --project 04-concurrency-I/examples/Chapter04.csproj -- observe
~~~

อีก terminal:

~~~bash
ps -L -p PID -o pid,tid,psr,stat,comm
ls /proc/PID/task
~~~

## C# Exercises

1. สร้าง Thread 10 ตัวและ Join ทุกตัว
2. ให้แต่ละ thread print ManagedThreadId
3. สร้าง shared counter แบบไม่ lock และรัน 20 รอบ
4. เพิ่ม Thread.Yield ใน loop แล้วเปรียบเทียบ
5. อธิบายว่า output order ใด guaranteed / ไม่ guaranteed
6. ใช้ Stopwatch เปรียบเทียบ sequential vs two threads
7. อธิบายว่าทำไม more threads ไม่แปลว่า faster เสมอ

## Quiz

- Thread share heap หรือไม่?
- Thread มี stack ของตัวเองหรือไม่?
- counter++ atomic หรือไม่?
- Concurrency ต้องมีหลาย cores หรือไม่?
- Join มีไว้ทำอะไร?
