# Lab 06 — Mutex, Semaphore, Atomics and CAS

## Goal

แก้ race/invariant problems ด้วย primitive ที่ต่างกัน และอธิบายเหตุผลที่เลือกได้

## Build

~~~bash
cd 06-synchronization-I
make clean
make
~~~

## A. Mutex Counter

~~~bash
./bin/mutex-counter
~~~

ตอบ:

1. critical section อยู่ตรงไหน
2. invariant คืออะไร
3. ถ้าย้าย unlock ขึ้นมาก่อน ++counter จะเกิดอะไร
4. mutex มี ownership อย่างไร

## B. Atomic Counter

~~~bash
./bin/atomic-counter
~~~

เปรียบเทียบ algorithm กับ mutex-counter

คำถาม:

- ทำไม fetch_add เหมาะกับ counter นี้
- ทำไม relaxed ใช้ได้ใน demo นี้
- ถ้าต้อง publish object อื่นพร้อม counter ยังตอบเหมือนเดิมได้หรือไม่

## C. Counting Semaphore

~~~bash
./bin/semaphore-limit
~~~

ตรวจ output ว่า active เกิน 2 หรือไม่

อธิบาย:

~~~text
capacity=2
sem_wait consumes permit
sem_post returns permit
~~~

## D. CAS Stock

~~~bash
for i in {1..5}; do ./bin/cas-stock; done
~~~

order ของ buyer ที่ชนะอาจเปลี่ยน แต่:

~~~text
successful purchases = 1
final stock = 0
~~~

ควรเป็น invariant

## E. Measure Carefully

ถ้ามี perf:

~~~bash
perf stat ./bin/mutex-counter
perf stat ./bin/atomic-counter
~~~

อย่าสรุปว่า atomic "เร็วกว่าเสมอ"

repeat หลายครั้งและอธิบาย noise, contention และ workload differences

## F. Design Matrix

เลือก primitive:

| Scenario | Your choice | Invariant |
|---|---|---|
| shared request count | ? | exact count |
| 3 DB connections | ? | active <= 3 |
| linked-list insert/delete | ? | structure valid |
| one-time state 1 -> 0 | ? | only one winner |

ไม่มีคะแนนจาก "เลือกคำถูก" อย่างเดียว ต้องอธิบาย scope และ failure schedule

## Explain

สรุปเป็นคำตนเอง:

~~~text
Mutex protects a critical section.
Semaphore controls permits/resources.
Atomic RMW performs one indivisible memory update.
CAS performs a conditional state transition.
~~~

แล้วเขียนข้อจำกัดของแต่ละตัวอย่างน้อย 1 ข้อ
