# Batch 02 Review — C# Edition

## Chapters 04–06

Knowledge chain:

~~~text
Thread
→ shared state
→ interleaving
→ race
→ critical section
→ lock / Monitor / Interlocked / SemaphoreSlim
~~~

## C# Gate

ต้องเขียนได้:

~~~csharp
lock (lockObj)
{
    // critical section
}
~~~

~~~csharp
Interlocked.Increment(ref counter);
~~~

~~~csharp
using SemaphoreSlim semaphore = new(2, 2);
~~~

และอธิบายว่าแต่ละ primitive แก้ปัญหาคนละชนิด

## Misconceptions

- source line เดียว = atomic ❌
- Volatile = lock ❌
- Interlocked ตัวเดียวทำ transaction หลายขั้น atomic ❌
- Thread = CPU core ❌
- more threads = faster เสมอ ❌
