# Chapter 07 — Synchronization Part II with C#

## Producer–Consumer

บทนี้ใช้รูปแบบเดียวกับงาน Thread Safe Buffer:

~~~csharp
lock (BufferLock)
{
    while (Count == Buffer.Length)
        Monitor.Wait(BufferLock);

    // enqueue

    Monitor.PulseAll(BufferLock);
}
~~~

consumer ใช้ pattern เดียวกันกับ condition Queue Empty

สำคัญ:

- Wait ต้องอยู่ใน while
- PulseAll ไม่ได้แปลว่า condition เป็นจริงเสมอ
- thread ที่ตื่นต้อง reacquire lock แล้วตรวจ condition ใหม่

## Readers–Writers

ใช้:

~~~csharp
ReaderWriterLockSlim
~~~

reader หลายตัวอ่านพร้อมกันได้ แต่ writer ต้อง exclusive

ต้องแยก correctness ออกจาก fairness

## Dining Philosophers

ใช้ object locks เป็น forks

แก้ circular wait ด้วย global lock ordering:

~~~text
lock resource เลขน้อยก่อน
แล้วค่อยเลขมาก
~~~

## C# Exercises

1. จาก Thread Safe Buffer เปลี่ยน capacity 10 เป็น 3 แล้วอธิบายการ Wait
2. เพิ่ม producer เป็น 3 threads
3. เพิ่ม consumer เป็น 5 threads
4. เปลี่ยน PulseAll เป็น Pulse แล้ววิเคราะห์ผล
5. ทำ Readers–Writers ด้วย ReaderWriterLockSlim
6. สร้าง writer-heavy workload แล้ววัดเวลา
7. เขียน Dining Philosophers แบบ naive แล้ววาด deadlock schedule บนกระดาษ
8. แก้ด้วย lock ordering

## Quiz

- Monitor.Wait ปล่อย lock ชั่วคราวหรือไม่?
- Wait return แล้ว thread ถือ lock อีกครั้งหรือไม่?
- ReaderWriterLockSlim read lock exclusive หรือไม่?
- lock ordering ช่วยป้องกัน circular wait หรือไม่?
- deadlock-free แปลว่า fair หรือไม่?
