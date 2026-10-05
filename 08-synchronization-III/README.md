# Chapter 08 — Synchronization Part III with C#

## Monitor

C# มี Monitor โดยตรง:

~~~csharp
lock (obj)
{
    while (!condition)
        Monitor.Wait(obj);

    Monitor.PulseAll(obj);
}
~~~

Monitor ช่วยสร้าง monitor-style abstraction:

- shared state
- mutual exclusion
- wait conditions
- operations

## Deadlock

classic example:

~~~text
Thread A holds Lock1, waits Lock2
Thread B holds Lock2, waits Lock1
~~~

Coffman Conditions:

1. Mutual Exclusion
2. Hold and Wait
3. No Preemption
4. Circular Wait

## Detection

Lab ใช้ C# graph + DFS หา cycle โดยไม่สร้าง program ที่ค้างจริง

## Starvation

thread พร้อมทำงานแต่ไม่ได้ resource นานมากเพราะ policy/fairness

## Livelock

threads ยังเปลี่ยน state/retry แต่ไม่มี useful progress

Lab ใช้ bounded retry เพื่อไม่ให้โปรแกรม infinite

## Priority Inversion

high-priority thread รอ lock ที่ low-priority thread ถือ ขณะที่ medium-priority work แทรก

เรียน concept ก่อน ไม่ตั้ง real-time scheduling บนเครื่องหลัก

## C# Exercises

1. เขียน Wait-For Graph class
2. เพิ่ม cycle 4 nodes แล้วตรวจด้วย DFS
3. สร้าง two-lock ordering rule
4. ใช้ Monitor.TryEnter ทำ retry
5. อธิบายว่าทำไม symmetric retry อาจ livelock
6. เพิ่ม random backoff แล้วเปรียบเทียบ
7. เขียน monitor-style bounded queue class

## Quiz

- Deadlock ต้องมี circular wait ใน classic model หรือไม่?
- Starvation ต้องมี cycle หรือไม่?
- Livelock threads active ได้หรือไม่?
- Monitor.PulseAll เก็บ token แบบ Semaphore หรือไม่?
- lock ordering ทำลาย Coffman condition ใด?
