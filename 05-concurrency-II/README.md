# Chapter 05 — Concurrency Part II with C#

## Goals

เข้าใจ shared mutable state, read-modify-write, check-then-act, Thread Safety, Reentrancy, visibility/order และ foreground/background threads

## Check-Then-Act

~~~text
check stock
↓
time passes / another thread runs
↓
act using old observation
~~~

แม้ใช้ Interlocked ตอน decrement ก็ยังเกิด logical race ได้ถ้า check กับ act ไม่เป็น transaction เดียว

run:

~~~bash
dotnet run --project 05-concurrency-II/examples/Chapter05.csproj -- check-then-act
~~~

## Volatile

C# มี:

~~~csharp
Volatile.Read(ref value);
Volatile.Write(ref value, newValue);
~~~

ใช้สำหรับ visibility/order pattern เฉพาะ ไม่ใช่ replacement ของ lock สำหรับ invariant หลายขั้น

## Interlocked

Interlocked ให้ atomic read-modify-write เช่น:

~~~csharp
Interlocked.Increment(ref counter);
Interlocked.CompareExchange(ref value, newValue, expected);
~~~

แต่ atomic primitive หนึ่งตัวไม่ทำให้ทั้ง business transaction atomic

## Thread Safety

code thread-safe เมื่อใช้งานพร้อมกันตาม contract แล้ว state ไม่เสีย

วิธีสร้าง thread safety:

- lock
- Monitor
- Interlocked
- immutable data
- ownership
- thread-local data

## Reentrancy

reentrant ไม่เท่ากับ thread-safe ทุกกรณี

ต้องดูว่า function ใช้ shared mutable state หรือ internal locks หรือไม่

## Background Thread

C# มี:

~~~csharp
thread.IsBackground = true;
~~~

background thread ไม่เท่ากับ POSIX detached thread แบบ 1:1

ถ้ามีแต่ background threads เหลือ process สามารถ terminate ได้

## C# Exercises

1. สร้าง check-then-act inventory 1 ชิ้นกับ buyer 2 threads
2. แก้ด้วย lock ให้ check+update อยู่ critical section เดียว
3. เปรียบเทียบ Volatile.Read/Write กับ lock
4. ใช้ ThreadLocal<int> เพื่อสร้าง per-thread state
5. สร้าง background thread แล้วทดลอง Main จบโดยไม่ Join
6. อธิบายเหตุผลที่ volatile ไม่แก้ compound invariant

## Quiz

- Interlocked.Decrement atomic หรือไม่?
- Atomic decrement ทำให้ check-then-act atomic ทั้งชุดหรือไม่?
- Volatile = lock หรือไม่?
- Background thread = process แยกหรือไม่?
- Thread-safe = reentrant เสมอหรือไม่?
