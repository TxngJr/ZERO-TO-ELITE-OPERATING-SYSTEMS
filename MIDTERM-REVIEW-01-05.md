# Midterm Review — Chapters 01–05 — C# Edition

## Must Know

- OS / Kernel / System Call
- Process / Thread
- PID / context switch
- Process.Start / WaitForExit
- Thread.Start / Join
- Concurrency vs Parallelism
- Race Condition
- Critical Section
- Shared Mutable State
- Check-Then-Act
- Volatile / Interlocked intro

## C# Code Questions

### 1

~~~csharp
int counter = 0;

Thread a = new Thread(() =>
{
    for (int i = 0; i < 100000; i++)
        counter++;
});
~~~

ถาม:

- มี shared state อะไร
- counter++ atomic หรือไม่
- race เกิดได้อย่างไร

### 2

~~~csharp
Thread t = new Thread(Work);
t.Start();
t.Join();
~~~

อธิบาย Start และ Join

### 3

~~~csharp
int observed = Volatile.Read(ref stock);
if (observed > 0)
    Interlocked.Decrement(ref stock);
~~~

อธิบายว่าทำไมยัง check-then-act race ได้

## Linux Observation

ต้องใช้ได้:

~~~bash
ps
ps -L
pstree
strace
/proc/PID/status
/proc/PID/maps
~~~

## Practice

1. เขียน C# program สร้าง 4 threads
2. ทำ race counter
3. แก้ด้วย lock
4. อธิบาย mode switch vs context switch
5. อธิบาย Process vs Thread
