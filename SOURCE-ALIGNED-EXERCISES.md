# Source-Aligned Exercise Bank

โจทย์ชุดนี้ผูกกับไฟล์ที่เรียนจริงโดยตรง

---

# Part A — Activity 02 Sequential vs Threaded

## A1

ใน sequential version plus ทำงานก่อน minus

อธิบายว่าทำไม sum ไม่ถูกแก้พร้อมกันโดยสอง execution flows

## A2

คำนวณเชิงคณิตศาสตร์ว่า:

~~~text
plus:
1 ... 1,000,000

minus:
0 ... 999,999
~~~

expected final sum ควรเป็นเท่าไรหาก arithmetic ไม่ overflow

## A3

เพิ่ม Thread สองตัวแต่เอา lock ออก

ทำนาย failure mode ของ shared sum

## A4

อธิบายว่า sum += i เป็น read-modify-write อย่างไร

## A5

ทำไม lock ทุก iteration แม้ correct แต่อาจช้ากว่า sequential

## A6

System.Threading.Lock ใน Activity 02 ต่างจาก object monitor ใน Activity 03 อย่างไร

## A7

ทำไม Stopwatch หนึ่งรอบยังไม่ใช่ benchmark ที่น่าเชื่อถือ

## A8

ออกแบบการวัด 20 รอบและรายงาน median

---

# Part B — Activity 03 One Reader / One Writer

## B1

ระบุ shared variables:

- x
- exitflag
- hasValue

## B2

เขียน invariant ของ hasValue

## B3

ทำไม reader ใช้:

~~~text
while !hasValue and not exiting
→ Wait
~~~

แทน if

## B4

เมื่อ Wait ถูกเรียก lock ถูกถืออยู่หรือไม่

## B5

ระหว่าง Wait thread ยังถือ lockObj อยู่หรือไม่

## B6

เมื่อ Wait return thread ต้องได้ monitor กลับก่อนหรือไม่

## B7

ทำไม writer หลัง publish ต้องรอ hasValue กลับ false

## B8

ถ้าเอา PulseAll หลัง reader consume ออกจะเกิดอะไร

---

# Part C — Activity 03 Three Readers

## C1

input หนึ่งข้อความถูกอ่านโดย reader ทั้งสามหรือ reader หนึ่งตัว

อธิบายจาก hasValue protocol

## C2

PulseAll ปลุกทุก waiter แล้วทำไมไม่ใช่ทุก reader ที่ consume ค่าเดียวกัน

## C3

วาด waiting queue / ready queue หลัง PulseAll

## C4

ถ้าต้องการ broadcast ให้ reader ทุกตัวเห็นทุก message ต้อง redesign state อย่างไร

เสนอแนวคิด per-reader acknowledgement หรือ sequence number

## C5

อธิบาย exitflag termination protocol

---

# Part D — Unsafe Buffer

## D1

หา race บน Count

## D2

หา race บน Front

## D3

หา race บน Back

## D4

หา buffer-full bug

## D5

หา buffer-empty bug

## D6

นับจำนวน items ที่ producer สร้างจาก loop bounds

## D7

นับจำนวน dequeue attempts จาก consumer loop เดิม

## D8

อธิบายว่าทำไมตัวเลขสองฝั่งไม่สมดุล

## D9

ทำไมไม่มี Join เป็น lifecycle problem

## D10

เขียน list invariants ที่ version แก้ต้องรักษา

---

# Part E — Thread-Safe Buffer

## E1

อธิบาย:

~~~text
while Count == capacity
→ producer waits
~~~

## E2

อธิบาย:

~~~text
while Count == 0 AND producers remain
→ consumer waits
~~~

## E3

ทำไม consumer จบได้เมื่อ:

~~~text
Count == 0
AND
ProducerFinished == TotalProducer
~~~

## E4

ProducerDone ต้อง lock BufferLock เพราะอะไร

## E5

ทำไม ProducerDone ต้อง PulseAll

## E6

Front/Back modulo ทำอะไร

## E7

พิสูจน์ว่า Count ไม่ควรต่ำกว่า 0

## E8

พิสูจน์ว่า Count ไม่ควรเกิน capacity

## E9

Sleep 5/7/16 ใน assignment มีผลต่อ timing แต่ไม่ควรเป็น correctness mechanism เพราะอะไร

## E10

consumersReady/canExit มีไว้ทำอะไรนอกเหนือจาก producer-consumer correctness

---

# Part F — Case Study 02

## F1

ทำไมแต่ละ worker ใช้ localResult

## F2

ทำไม global result ถูก lock เฉพาะ merge

## F3

เปรียบเทียบ:

~~~text
lock every Calculate1 result
vs
localResult then one merge
~~~

## F4

แบ่ง 10,000,000 indices เป็น 16 ช่วงละ 625,000 แล้วตรวจว่าครบช่วงใด

## F5

ไฟล์ data มี length 11,000,001 แต่ source ที่ให้มาไม่ได้อธิบาย specification ของส่วนที่เกิน

ควรตอบอย่างไรโดยไม่เดา

## F6

เหตุผลที่ 16 threads อาจไม่เท่ากับจำนวน CPU cores คืออะไร

## F7

threads มากขึ้นรับประกันเร็วขึ้นหรือไม่

## F8

เสนอ experiment 1,2,4,8,16 threads โดยคง workload เดิม

---

# Part G — OS Integration

## G1

Thread.Start แล้ว thread รันทันทีแบบ deterministic หรือไม่

## G2

Thread.Join เกี่ยวกับ process/thread lifecycle อย่างไร

## G3

Monitor.Wait ทำให้ thread runnable อยู่ตลอดหรือไม่

## G4

blocked waiter มีผลต่อ scheduler อย่างไร

## G5

shared buffer อยู่ใน virtual address space ของ process ใด

## G6

หลาย threads ใน process เดียวกันเห็น static buffer เดียวกันได้อย่างไร

## G7

lock correctness เกี่ยวข้องกับ CPU scheduling แต่ไม่สามารถพึ่ง scheduling order อย่างไร

## G8

เชื่อม Activity 02 → Chapter 04 → Chapter 06

## G9

เชื่อม Activity 03 → Chapter 07 → Chapter 08

## G10

เชื่อม Case Study → Scheduling / performance analysis
