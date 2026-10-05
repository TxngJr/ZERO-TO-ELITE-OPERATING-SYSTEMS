# Chapter 06 — Synchronization Part I with C#

## Goals

ใช้ lock, Monitor, Mutex, SemaphoreSlim, Interlocked และ CompareExchange ได้อย่างถูกต้อง

## lock

รูปแบบหลักเหมือนที่ใช้ในงานเรียน:

~~~csharp
lock (lockObj)
{
    // critical section
}
~~~

lock สร้าง mutual exclusion และ synchronization/visibility guarantees ตาม .NET memory model

## Monitor

lock มีความสัมพันธ์กับ Monitor.Enter/Exit

สำหรับ wait condition:

~~~csharp
lock (lockObj)
{
    while (!condition)
    {
        Monitor.Wait(lockObj);
    }

    // change state

    Monitor.PulseAll(lockObj);
}
~~~

## Mutex

System.Threading.Mutex สามารถใช้ synchronization ที่มี OS-level capabilities และข้าม process ได้ในบางรูปแบบ

สำหรับ thread-only critical section ภายใน process, lock มักเบาและตรงกว่า

## SemaphoreSlim

~~~csharp
using SemaphoreSlim slots = new(2, 2);
slots.Wait();
try
{
    // limited resource
}
finally
{
    slots.Release();
}
~~~

ใช้เมื่อ resource มี permits มากกว่า 1

## Interlocked

~~~csharp
Interlocked.Increment(ref counter);
Interlocked.CompareExchange(ref stock, 0, 1);
~~~

เหมาะกับ atomic state transition ขนาดเล็ก

## Mutex vs Semaphore vs Interlocked

- lock/Monitor: protect complex critical section
- Mutex: mutex abstraction ที่ใช้ OS handle ได้
- SemaphoreSlim: permit counter
- Interlocked: atomic operation
- CompareExchange: conditional atomic transition

## C# Exercises

1. แก้ race counter ด้วย lock
2. แก้ด้วย Interlocked.Increment
3. วัดเวลา 20 รอบด้วย Stopwatch
4. สร้าง SemaphoreSlim capacity 3
5. ใช้ CompareExchange ทำ one-winner state
6. แปลง Producer/Consumer skeleton ให้ใช้ lock + Monitor.Wait/PulseAll
7. อธิบายว่า critical section ควรครอบ check+update เมื่อใด

## Quiz

- lock กับ SemaphoreSlim เหมือนกันหรือไม่?
- Monitor.Wait ต้องถือ monitor ก่อนหรือไม่?
- Interlocked.Increment atomic หรือไม่?
- CompareExchange ทำอะไร?
- critical section ใหญ่เกินไปมีผลอย่างไร?
