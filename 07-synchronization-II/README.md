# Chapter 07 — Synchronization Part II

## เป้าหมาย

หลังบทนี้ต้องสามารถ:

- แก้ Producer–Consumer ด้วย bounded buffer
- ใช้ lock + Monitor.Wait + Monitor.PulseAll
- อธิบาย state predicate
- อธิบาย single-slot handoff จาก Activity 03
- ใช้ ReaderWriterLockSlim
- วิเคราะห์ Dining Philosophers
- ออกแบบ global lock ordering
- แยก deadlock freedom ออกจาก fairness

---

## 1. Producer–Consumer

bounded buffer มี shared state:

~~~text
buffer
front
back
count
producer completion state
~~~

invariant:

~~~text
0 <= count <= capacity
front/back อยู่ใน ring-buffer range
dequeue เฉพาะเมื่อ count > 0
enqueue เฉพาะเมื่อ count < capacity
~~~

---

## 2. Ring Buffer

enqueue:

~~~text
buffer[back] = item
back = (back + 1) mod capacity
count++
~~~

dequeue:

~~~text
item = buffer[front]
front = (front + 1) mod capacity
count--
~~~

operation เหล่านี้แตะ shared invariant หลาย fields จึงต้อง synchronize เป็น critical section เดียว

---

## 3. Monitor Pattern

producer:

~~~csharp
lock (bufferLock)
{
    while (count == capacity)
        Monitor.Wait(bufferLock);

    // enqueue

    Monitor.PulseAll(bufferLock);
}
~~~

consumer:

~~~csharp
lock (bufferLock)
{
    while (count == 0 && producersStillRunning)
        Monitor.Wait(bufferLock);

    // dequeue or terminate

    Monitor.PulseAll(bufferLock);
}
~~~

---

## 4. ทำไมต้อง while

Thread ที่ถูก PulseAll:

1. ถูกย้ายจาก waiting queue ไปพร้อมแข่งขัน
2. ยังไม่ได้ lock ทันที
3. thread อื่นอาจเปลี่ยน state ก่อน
4. เมื่อได้ lock กลับจึงต้องตรวจ predicate ใหม่

ดังนั้น:

~~~text
wake-up
does not mean
predicate is guaranteed true
~~~

---

## 5. Activity 03 as Single-Slot Buffer

Activity 03 ใช้:

~~~text
hasValue = false
→ writer may publish

hasValue = true
→ one reader may consume
~~~

หลาย readers ไม่ได้แปลว่าทุก reader ต้องเห็นทุก message

reader ที่ได้ monitor ก่อนจะ consume แล้วเปลี่ยน hasValue=false

---

## 6. Thread-Safe Buffer จากไฟล์เรียน

ไฟล์ที่เรียนเพิ่ม state:

- ProducerFinished
- TotalProducer
- consumersReady
- canExit

termination predicate สำคัญ:

~~~text
buffer empty
AND
all producers finished
→ consumer can stop consuming
~~~

Sleep 5/7/16 ms ใน assignment เป็นข้อกำหนดของโจทย์ ไม่ควรเปลี่ยนเมื่อต้องส่งงานนั้น

---

## 7. Pulse vs PulseAll

Pulse:
- signal waiter หนึ่งรายตาม monitor waiting semantics

PulseAll:
- signal waiters ทั้งหมด

ทั้งคู่ไม่เก็บ token ถ้าไม่มี waiter แบบ semaphore counter

และ signal ไม่ได้มอบ lock ให้ waiter ทันที

---

## 8. Readers–Writers

resource หนึ่ง:

~~~text
many readers allowed together
one writer exclusive
~~~

C#:

~~~csharp
ReaderWriterLockSlim
~~~

read:

~~~csharp
rw.EnterReadLock();
try
{
    // read
}
finally
{
    rw.ExitReadLock();
}
~~~

write:

~~~csharp
rw.EnterWriteLock();
try
{
    // write
}
finally
{
    rw.ExitWriteLock();
}
~~~

---

## 9. Fairness

correct synchronization ไม่รับประกัน strict FIFO fairness

ต้องแยก:

~~~text
Safety
Liveness
Fairness
Performance
~~~

ReaderWriterLockSlim behavior ไม่ควรถูกอธิบายว่าเป็น strict reader-priority หรือ strict writer-priority contract

---

## 10. Dining Philosophers

model:

- philosopher = thread
- fork = lockable resource
- eating = operation ต้องถือสอง resources

naive:

~~~text
ทุกคน lock left
แล้วรอ right
~~~

สามารถ circular wait

---

## 11. Global Lock Ordering

กำหนด:

~~~text
F0 < F1 < F2 < F3 < F4
~~~

ทุก thread ต้อง acquire resource จากลำดับน้อยไปมาก

จึงไม่สามารถสร้าง circular wait ที่ย้อน order ได้

---

## 12. Deadlock-Free ไม่เท่ากับ Starvation-Free

lock ordering ช่วย circular wait

แต่ thread หนึ่งยังอาจได้ resource ช้ามากเพราะ scheduling/contention/fairness policy

---

## Lab

~~~bash
dotnet run --project 07-synchronization-II/examples/Chapter07.csproj -- producer-consumer
dotnet run --project 07-synchronization-II/examples/Chapter07.csproj -- readers-writers
dotnet run --project 07-synchronization-II/examples/Chapter07.csproj -- dining
~~~

---

## แบบฝึกหัด

1. เขียน ring-buffer invariants
2. หา critical section ใน enqueue
3. หา critical section ใน dequeue
4. ทำไม Count ต้องแก้ภายใต้ lock เดียวกับ Front/Back
5. ทำไม Wait ใช้ while
6. PulseAll ต่างจาก semaphore Release อย่างไร
7. อธิบาย Activity 03 แบบ one-reader handoff
8. อธิบาย Activity 03 แบบ 3 readers ว่าทำไมไม่ broadcast
9. เพิ่ม producer เป็น 3
10. เพิ่ม consumer เป็น 5
11. ออกแบบ producer-finished predicate
12. ReaderWriterLockSlim เหมาะกับ workload แบบใด
13. สร้าง writer-heavy workload
14. วาด dining deadlock
15. แก้ด้วย resource ordering
16. deadlock-free vs fairness
17. หา bug ถ้าใช้ if รอบ Monitor.Wait
18. หา bug ถ้า PulseAll ถูกเรียกนอก lock
19. หา bug ถ้า Count ถูกอ่านนอก synchronization protocol
20. อธิบาย consumersReady/canExit handshake ในไฟล์เรียน

---

## Explain-It-Back

~~~text
shared state
→ invariant
→ predicate
→ lock
→ while Wait
→ modify state
→ Pulse/PulseAll
→ re-check
~~~
