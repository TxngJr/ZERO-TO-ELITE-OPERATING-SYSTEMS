# Chapter 04 — Concurrency Part I

## เป้าหมาย

หลังบทนี้ต้องเข้าใจ:

- Thread
- Start / Join / Sleep
- Concurrency vs Parallelism
- Interleaving
- Shared State
- Race Condition
- Critical Section
- Activity 02 sequential vs threaded baseline
- Case Study partition concept

---

## 1. Thread

Thread คือ execution flow ภายใน process

threads ใน process เดียวกันมัก share:

- heap
- static fields
- process address space
- open resources

แต่มี execution state ของตัวเอง:

- stack
- registers/context
- scheduling state

---

## 2. Start / Join

~~~csharp
Thread t = new Thread(Work);
t.Start();
t.Join();
~~~

Start:
ทำให้ thread เริ่ม execution lifecycle

Join:
caller รอ thread target จบ

Join ไม่ได้ “รวมค่าผลลัพธ์” ให้เอง

---

## 3. Thread.Sleep

Sleep ทำให้ thread ไม่ runnable ชั่วคราวตาม requested interval

ไม่ใช่ synchronization primitive สำหรับ correctness

ผิด:

~~~text
Sleep 100 ms
therefore another thread must have finished
~~~

ถูก:

~~~text
use Join / condition synchronization / other explicit protocol
~~~

---

## 4. Concurrency vs Parallelism

Concurrency:

หลาย execution flows มี progress overlap/interleave

Parallelism:

execute พร้อมกันจริงบนหลาย logical CPUs

single-core ก็มี concurrency ได้ผ่าน time sharing

---

## 5. Interleaving

สมมติ:

~~~text
A1 A2 A3
B1 B2 B3
~~~

possible:

~~~text
A1 B1 A2 B2 B3 A3
~~~

program correctness ต้องไม่พึ่ง interleaving ที่ “เราหวังว่าจะเกิด”

---

## 6. Activity 02-1 — Sequential Baseline

ไฟล์เรียนทำ plus() แล้ว minus() ต่อกัน

ไม่มี concurrent access ต่อ sum

จึงเป็น control baseline ที่ดีสำหรับเทียบกับ threaded version

---

## 7. Activity 02-2 — Threads + Lock

ไฟล์ใช้:

- Thread P
- Thread M
- Start
- Join
- Lock
- Stopwatch

สิ่งที่ต้องวิเคราะห์:

- correctness
- lock contention
- lock acquisition count
- scheduler overhead
- JIT/benchmark noise

การเพิ่ม threads ไม่รับประกันว่าเร็วขึ้น

---

## 8. Race Condition

ถ้าสอง thread ทำ:

~~~csharp
counter++;
~~~

logical decomposition:

~~~text
read
compute
write
~~~

interleaving สามารถ lost update

---

## 9. Critical Section

critical section ไม่ใช่ “ทุก code ที่มี thread”

มันคือ region ที่ต้องควบคุม concurrent access เพื่อรักษา invariant

---

## 10. Case Study — Partition + Local Reduction

ไฟล์ Case Study แบ่งช่วงงานให้หลาย threads

แต่ละ worker:

~~~text
compute localResult
↓
lock only once
↓
result += localResult
~~~

นี่ลด global contention เทียบกับ lock ทุก inner-loop update

สิ่งที่ source ไม่บอก เช่น implementation Calculate1 ต้องไม่เดา

---

## 11. Linux Thread Observation

~~~bash
dotnet run --project 04-concurrency-I/examples/Chapter04.csproj -- observe
~~~

อีก terminal:

~~~bash
ps -L -p PID -o pid,tid,psr,stat,comm
ls /proc/PID/task
~~~

PSR บอก CPU ที่ tool รายงานสำหรับ task ณ observation ไม่ได้แปลว่า thread ถูก pin ถาวร

---

## แบบฝึกหัด

1. Process vs Thread
2. Start ทำอะไร
3. Join ทำอะไร
4. Sleep ใช้แทน Join ได้หรือไม่
5. Concurrency vs Parallelism
6. single-core มี concurrency ได้ไหม
7. counter++ ทำไม race ได้
8. วาด lost-update interleaving
9. ทำ sequential Activity 02 baseline
10. ทำ threaded version
11. รัน 20 รอบและเก็บเวลา
12. อธิบายว่าทำไม timing แกว่ง
13. สร้าง 10 threads แล้วดู Linux TIDs
14. เปรียบเทียบ ManagedThreadId กับ TID
15. ออกแบบ local reduction แบบ Case Study

---

## Explain-It-Back

~~~text
Thread creation
→ scheduler interleaving
→ shared state
→ possible race
→ identify critical section
~~~
