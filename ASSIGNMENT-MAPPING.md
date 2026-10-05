# Mapping: ไฟล์ที่เรียนจริง → Zero to Elite OS

เอกสารนี้แยกชัดว่าอะไร “มาจากไฟล์ที่เรียน” และอะไรเป็น “เนื้อหา OS/.NET ที่คอร์สเติมเพื่ออธิบายให้ครบ”

---

## 1. Program.cs.Activity 02 1 — Sequential Baseline

สิ่งที่ไฟล์สอนโดยตรง:

- shared field sum
- plus()
- minus()
- ทำงานแบบ sequential
- ใช้ Stopwatch วัดเวลา
- ไม่มี Thread

ตำแหน่งในคอร์ส:

- Chapter 04 — baseline ก่อนสร้าง concurrency
- Chapter 06 — control experiment เพื่อเทียบ synchronization overhead

mental model:

~~~text
sequential execution
→ ไม่มี concurrent access ต่อ sum
→ ไม่ต้อง synchronize sum ในโปรแกรมนี้
~~~

ข้อควรระวัง:

ผลเวลาเพียง 1 รอบไม่เพียงพอสรุป performance เพราะ JIT, scheduler และ background work ทำให้เวลาแกว่งได้

---

## 2. Program.cs.Activity 02 2 — Threads + System.Threading.Lock

สิ่งที่ไฟล์สอนโดยตรง:

- Thread
- ThreadStart
- Start()
- Join()
- System.Threading.Lock
- C# lock statement
- Stopwatch

รูปแบบจากงาน:

~~~csharp
private static Lock _lock = new Lock();

lock (_lock)
{
    sum += i;
}
~~~

ตำแหน่งในคอร์ส:

- Chapter 04 — Thread lifecycle
- Chapter 06 — mutual exclusion
- Chapter 06 — modern System.Threading.Lock

### กฎสำคัญ

System.Threading.Lock และ Monitor object ต้องไม่ถูกสอนเป็น mechanism เดียวกัน

ใน C# รุ่นใหม่:

~~~csharp
Lock modernLock = new();

lock (modernLock)
{
    // compiler uses Lock.EnterScope()
}
~~~

แต่ Producer–Consumer ที่ใช้ condition synchronization ใช้:

~~~csharp
object monitorLock = new();

lock (monitorLock)
{
    Monitor.Wait(monitorLock);
}
~~~

ดังนั้นคอร์สแยก:

~~~text
System.Threading.Lock
→ general mutual exclusion

object + lock + Monitor.Wait/Pulse/PulseAll
→ monitor condition synchronization
~~~

ห้ามเปลี่ยน monitor object เป็น System.Threading.Lock แล้วสมมติว่า Monitor.Wait จะทำงานเหมือนเดิม

---

## 3. Program.cs.Activity 03 1 — One Reader / One Writer Handoff

สิ่งที่ไฟล์สอน:

- shared string x
- exitflag
- hasValue
- dedicated lockObj
- Monitor.Wait
- Monitor.PulseAll
- writer รอจนค่าก่อนถูก consume
- reader รอจนมีค่าใหม่

state-machine:

~~~text
hasValue = false
writer may publish

hasValue = true
reader may consume
writer waits
~~~

นี่คือ condition synchronization ไม่ใช่แค่ mutual exclusion

ตำแหน่งในคอร์ส:

- Chapter 05 — shared state + state predicate
- Chapter 06 — Monitor semantics
- Chapter 07 — Producer–Consumer foundation

---

## 4. Program.cs.Activity 03 2 — Three Readers / One Writer

แม้มี Reader 3 threads แต่ shared state มี hasValue เพียงหนึ่งค่า

เมื่อ reader หนึ่งได้ lock และอ่าน:

~~~csharp
hasValue = false;
Monitor.PulseAll(lockObj);
~~~

ดังนั้น input หนึ่งค่าไม่ได้หมายความว่า reader ทั้งสามต้องได้รับค่านั้นทุกคน

semantics จริง:

~~~text
one published value
→ one reader wins and consumes
→ hasValue becomes false
→ other readers re-check predicate and wait again
~~~

นี่เป็น single-slot producer-consumer/handoff มากกว่า broadcast messaging

ตำแหน่งในคอร์ส:

- Chapter 07 — single-slot buffer
- Chapter 08 — Monitor waiting queue / ready queue

---

## 5. Programmain.cs — Unsafe Buffer Baseline

baseline ไม่มี synchronization

ปัญหาที่ต้องหา:

1. Front, Back, Count เป็น shared mutable state
2. producer/consumer แก้ state พร้อมกันได้
3. ไม่มี full waiting
4. ไม่มี empty waiting
5. dequeue สามารถเกิดตอน logical buffer ว่าง
6. overwrite สามารถเกิดตอน logical buffer เต็ม
7. จำนวน dequeue ที่กำหนดไว้ไม่สัมพันธ์กับจำนวน item ที่ producer สร้าง
8. ไม่มี Join จึงไม่ควรใช้เป็น lifecycle design ที่ถูกต้อง

ไฟล์นี้ถูกใช้เป็น Find-the-Bug exercise ไม่ใช่ตัวอย่างที่ถูกต้อง

---

## 6. Program.cs — Thread Safe Buffer ที่แก้แล้ว

สิ่งที่ต้องรักษาตามไฟล์:

- ring buffer 10 slots
- Front, Back, Count
- dedicated BufferLock
- while (Count == TSBuffer.Length)
- while (Count == 0 && ProducerFinished < TotalProducer)
- Monitor.Wait
- Monitor.PulseAll
- producer completion count
- consumers exit เมื่อ producers ทุกตัวจบและ buffer ว่าง
- Start() / Join()
- Sleep 5 / 7 / 16 ms เป็นข้อกำหนดของ assignment และไม่ควรถูกแก้ในงานนั้น

invariants:

~~~text
0 <= Count <= TSBuffer.Length
Front และ Back อยู่ใน range ของ ring buffer
dequeue เฉพาะเมื่อ Count > 0
enqueue เฉพาะเมื่อ Count < capacity

consumer termination:
all producers finished
AND
Count == 0
~~~

consumersReady/canExit เป็น handshake เพิ่มเติมสำหรับ flow ที่ Main รอ consumer ทุกตัวถึง exit barrier ก่อนให้ผู้ใช้กดออก

---

## 7. Case study 02 — Partition + Local Reduction

ไฟล์สอน:

- data array ขนาดใหญ่
- 16 worker threads
- แบ่งช่วงด้วย startIndex/endIndex
- worker เก็บผลใน localResult
- lock เฉพาะตอน merge result

pattern:

~~~text
partition input
→ compute locally without global lock
→ merge once under lock
~~~

นี่ดีกว่า lock ทุก iteration ใน workload ที่ local computation แยกจากกันได้

ตำแหน่งในคอร์ส:

- Chapter 04 — parallel decomposition
- Chapter 06 — lock granularity
- Chapter 09 — scheduling/performance interpretation

สิ่งที่ไฟล์ที่ส่งมาไม่รองรับให้คอร์สแต่งเพิ่มเอง:

- implementation ภายใน CalculatingFunctions.CalClass.Calculate1
- specification ที่อธิบาย data length 11,000,001
- เหตุผลเชิง assignment ที่กำหนด 16 threads

คอร์สจึงระบุเรื่องเหล่านี้ว่า “unknown from supplied source” แทนการเดา

---

# Study Order ที่ตรงกับไฟล์เรียน

~~~text
Activity 02-1 sequential baseline
↓
Activity 02-2 Thread + Lock
↓
Activity 03-1 one-reader handoff
↓
Activity 03-2 multi-reader competition
↓
Unsafe Buffer baseline
↓
Thread-Safe Buffer
↓
Case Study local reduction
↓
OS theory ที่อธิบาย runtime behavior
~~~
