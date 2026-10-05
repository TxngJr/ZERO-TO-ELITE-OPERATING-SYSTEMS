# Chapter 06 — Synchronization Part I: C# Locking Correctly

## เป้าหมาย

หลังบทนี้ต้องแยกให้ออกว่า:

- mutual exclusion คืออะไร
- critical section คืออะไร
- System.Threading.Lock ใช้เมื่อใด
- object + lock + Monitor ใช้เมื่อใด
- SemaphoreSlim ต่างจาก lock อย่างไร
- Interlocked เหมาะกับ operation แบบใด
- CompareExchange/CAS ทำอะไร
- lock granularity มีผลต่อ correctness และ performance อย่างไร

---

## 1. ปัญหาที่ synchronization แก้

เมื่อหลาย Thread share mutable state:

~~~text
read state
modify
write state
~~~

interleaving สามารถทำให้ invariant พัง

ตัวอย่าง:

~~~csharp
counter++;
~~~

หนึ่ง statement ใน source ไม่ได้แปลว่าเป็น atomic operation

---

## 2. Critical Section

critical section คือช่วง code ที่แตะ shared state ซึ่งต้องรักษา invariant เป็นหน่วยเดียว

ตัวอย่าง inventory:

~~~text
if stock > 0
    stock--
~~~

ถ้า check และ update ต้องสัมพันธ์กัน ทั้งสองขั้นต้องอยู่ใน synchronization protocol เดียวกัน

---

## 3. System.Threading.Lock — แบบที่ Activity 02 ใช้

ไฟล์เรียน Activity 02 ใช้รูปแบบ:

~~~csharp
private static Lock _lock = new Lock();

lock (_lock)
{
    sum += i;
}
~~~

สำหรับ .NET 9 / C# 13 ขึ้นไป System.Threading.Lock เป็น primitive ที่ออกแบบมาเพื่อ mutual exclusion โดยเฉพาะ

ใน C# รุ่นใหม่ เมื่อ expression ของ lock statement มี type เป็น System.Threading.Lock compiler ใช้ Lock.EnterScope() semantics

ดังนั้นมันไม่ใช่เพียง object monitor แบบเก่า

### ใช้เมื่อใด

ใช้เมื่อ:

- ต้องการ mutual exclusion ภายใน process
- ไม่ต้องใช้ Monitor.Wait/Pulse กับ lock object นั้น
- ต้องการ dedicated lock object

---

## 4. object + lock + Monitor — แบบที่ Activity 03 และ Thread-Safe Buffer ใช้

Producer–Consumer ของวิชาใช้:

~~~csharp
static readonly object BufferLock = new object();

lock (BufferLock)
{
    while (Count == TSBuffer.Length)
    {
        Monitor.Wait(BufferLock);
    }

    // modify shared state

    Monitor.PulseAll(BufferLock);
}
~~~

นี่คือ monitor condition synchronization

### กฎสำคัญ

Monitor.Wait, Monitor.Pulse และ Monitor.PulseAll ต้องถูกเรียกโดย Thread ที่ถือ monitor ของ object นั้น

Wait:

~~~text
caller owns monitor
↓
Wait releases monitor
↓
thread enters waiting queue
↓
Pulse/PulseAll makes it eligible to compete again
↓
thread reacquires monitor
↓
Wait returns
↓
predicate must be checked again
~~~

จึงใช้ while ไม่ใช่ if

---

## 5. ห้ามผสม Lock กับ Monitor แบบไม่เข้าใจ

อย่าเขียน mental model ว่า:

~~~text
System.Threading.Lock
=
object monitor ทุกประการ
~~~

ในคอร์สนี้ใช้กฎ:

~~~text
System.Threading.Lock
→ mutual exclusion

dedicated object + lock + Monitor
→ condition synchronization
~~~

นี่ทำให้ตรงทั้ง Activity 02 และ Activity 03

---

## 6. SemaphoreSlim

SemaphoreSlim เก็บจำนวน permits

ตัวอย่าง capacity 2:

~~~csharp
using SemaphoreSlim slots = new(2, 2);

slots.Wait();

try
{
    // at most 2 participants here
}
finally
{
    slots.Release();
}
~~~

Release ต้องอยู่ใน finally เมื่อมีโอกาสที่ code ภายใน throw exception

### Semaphore ไม่ใช่ mutex เสมอ

ถ้า initial count > 1 จะมีหลาย Thread เข้า protected region ได้พร้อมกัน

---

## 7. Interlocked

Interlocked เหมาะกับ atomic operation ขนาดเล็ก เช่น:

~~~csharp
Interlocked.Increment(ref counter);
Interlocked.Decrement(ref counter);
Interlocked.Exchange(ref value, newValue);
Interlocked.CompareExchange(ref value, newValue, expected);
~~~

มันไม่ทำให้ multi-step business rule กลายเป็น atomic โดยอัตโนมัติ

---

## 8. CAS / CompareExchange

แนวคิด:

~~~text
if current == expected
    current = newValue
return old current
~~~

ทั้งหมดเกิดเป็น atomic operation ที่ primitive รับประกัน

ใช้สร้าง state transition เช่น “มีผู้ชนะเพียงหนึ่ง Thread”

---

## 9. Lock Granularity

Activity 02 lock ทุก iteration:

~~~csharp
for (...)
{
    lock (_lock)
    {
        sum += i;
    }
}
~~~

correct แต่มี lock acquisition จำนวนมากและ serialize shared update

Case Study 02 ใช้ pattern ที่ดีกว่าสำหรับงานแบ่งช่วงได้:

~~~text
compute localResult without global lock
↓
lock once
↓
merge localResult
~~~

นี่คือ local reduction

---

## 10. Thread Safety vs Performance

correctness มาก่อน performance

ห้ามเอา lock ออกเพียงเพราะ benchmark ช้าลง

ลำดับที่ถูก:

1. ระบุ invariant
2. ทำให้ correct
3. วัด
4. ลด critical section โดยไม่ทำลาย invariant
5. วัดซ้ำ

---

## Lab

~~~bash
dotnet run --project 06-synchronization-I/examples/Chapter06.csproj -- monitor-lock-counter
dotnet run --project 06-synchronization-I/examples/Chapter06.csproj -- modern-lock-counter
dotnet run --project 06-synchronization-I/examples/Chapter06.csproj -- interlocked
dotnet run --project 06-synchronization-I/examples/Chapter06.csproj -- semaphore
dotnet run --project 06-synchronization-I/examples/Chapter06.csproj -- cas
dotnet run --project 06-synchronization-I/examples/Chapter06.csproj -- self-test
~~~

---

## แบบฝึกหัด

1. อธิบายว่า counter++ ทำไมไม่ atomic
2. แยก critical section ของ inventory check+decrement
3. เขียน counter ด้วย System.Threading.Lock
4. เขียน counter ด้วย object lock
5. อธิบายว่าทำไมสอง version ด้านบนไม่ควรถูกเหมารวมเมื่อใช้ Monitor
6. เขียน SemaphoreSlim capacity 3 พร้อม try/finally
7. ใช้ Interlocked.Increment ทำ counter
8. ใช้ CompareExchange ทำ one-winner flag
9. วิจารณ์การ lock ทุก iteration ใน Activity 02
10. ออกแบบ local reduction แบบ Case Study
11. อธิบาย mutex vs counting semaphore
12. อธิบาย safety vs performance
13. หา bug จาก code ที่ Wait โดยไม่ได้ lock object
14. หา bug จาก code ที่ใช้ if แทน while รอบ Monitor.Wait
15. อธิบายว่า PulseAll ทำไมไม่ส่ง “token” แบบ semaphore

---

## Explain-It-Back

อธิบายให้ได้โดยไม่เปิดโน้ต:

~~~text
shared invariant
→ critical section
→ choose primitive
→ acquire
→ modify state
→ release
→ waiting/pulse if condition synchronization is required
~~~
