# Lab 07 — Classic Synchronization Problems

## Build

~~~bash
cd 07-synchronization-II
make clean
make
~~~

## A. Producer–Consumer

~~~bash
./bin/producer-consumer
~~~

ตรวจ invariant:

~~~text
0 <= count <= 4
final count = 0
~~~

ตอบ:

- not_empty signal หลัง operation ใด
- not_full signal หลัง operation ใด
- ทำไม wait ต้องอยู่ใน while
- ถ้าเอา mutex ออกจะเสีย invariant ใด

## B. Readers–Writers

~~~bash
./bin/readers-writers
~~~

สังเกตว่า readers สามารถ overlap ภายใต้ read lock แต่ writer ต้อง exclusive

ตอบ:

- final value ทำไมเป็น 8
- rwlock รับประกัน FIFO fairness หรือไม่
- workload แบบไหน rwlock อาจไม่คุ้ม

## C. Dining Philosophers

~~~bash
./bin/dining-philosophers
~~~

วาด resource order:

~~~text
F0 < F1 < F2 < F3 < F4
~~~

แต่ละ philosopher ต้อง acquire ตาม order นี้

อธิบายว่าทำไม directed wait cycle ถูกป้องกัน

## D. Design Drill

ออกแบบด้วย pseudo-code:

1. queue capacity 16
2. cache ที่ read 95%, write 5%
3. operation ที่ต้อง lock resources A,C,D พร้อมกัน

ทุกข้อเขียน:

- invariant
- waiting condition
- primitive
- lock order
- fairness concern
