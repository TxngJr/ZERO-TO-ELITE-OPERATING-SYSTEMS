# Batch 02 Review — Chapters 04–06

## Knowledge Chain

~~~text
Thread
→ shared state
→ interleaving
→ race
→ atomicity / visibility / ordering
→ critical section
→ choose synchronization primitive
~~~

## ต้องแยกให้ได้

### System.Threading.Lock

ใช้ใน Activity 02 เพื่อ mutual exclusion

### object + lock + Monitor

ใช้ใน Activity 03 / Thread-Safe Buffer เมื่อมี condition waiting

### Interlocked

atomic read-modify-write ขนาดเล็ก

### SemaphoreSlim

permit counting

## C# Gate

ต้องเขียนได้:

~~~csharp
static readonly Lock Gate = new();

lock (Gate)
{
    // mutual exclusion
}
~~~

และ:

~~~csharp
static readonly object MonitorGate = new();

lock (MonitorGate)
{
    while (!condition)
        Monitor.Wait(MonitorGate);

    Monitor.PulseAll(MonitorGate);
}
~~~

## Misconceptions

- source line เดียว = atomic ❌
- Volatile = lock ❌
- Interlocked หนึ่ง operation ทำ transaction หลายขั้น atomic ❌
- System.Threading.Lock = Monitor object ทุกประการ ❌
- Thread = CPU core ❌
- more threads = faster เสมอ ❌

## Source Alignment Gate

ต้องอธิบาย:

- Activity 02-1
- Activity 02-2
- Activity 03-1
- Activity 03-2
- Case Study local reduction
