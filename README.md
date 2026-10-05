# Zero to Elite Operating Systems

หลักสูตร Operating Systems แบบลงมือทำบน **Fedora Linux / x86-64** จาก Absolute Zero ไปสู่ระดับ Systems Engineer

> Primary lab environment: Acer Aspire 7 A715-43G, AMD Ryzen 7 5825U, Fedora Linux

## Learning model

~~~text
Why → Concept → Mental Model → Linux → Code → Run → Observe → Debug → Explain
~~~

เราไม่เรียน OS ด้วย definition อย่างเดียว แต่สังเกต kernel/process/thread behavior และเขียน C experiments จริง

## Core roadmap

1. ✅ Course Overview & OS Introduction
2. ✅ Process & Context Switch Part I
3. ✅ Process & Context Switch Part II
4. ✅ Concurrency Part I
5. ✅ Concurrency Part II
6. ✅ Synchronization Part I
7. ⏳ Synchronization Part II
8. ⏳ Synchronization Part III
9. ⏳ Scheduling
10. ⏳ Address Translation
11. ⏳ Virtual Memory

## Batch 1

- [Chapter 01 — OS Introduction](./01-os-introduction/README.md)
- [Chapter 02 — Process & Context Switch I](./02-process-context-I/README.md)
- [Chapter 03 — Process & Context Switch II](./03-process-context-II/README.md)
- [Batch 01 Review](./BATCH-01-REVIEW.md)

## Batch 2

- [Chapter 04 — Concurrency Part I](./04-concurrency-I/README.md)
- [Chapter 05 — Concurrency Part II](./05-concurrency-II/README.md)
- [Midterm Review — Chapters 01–05](./MIDTERM-REVIEW-01-05.md)
- [Chapter 06 — Synchronization Part I](./06-synchronization-I/README.md)
- [Batch 02 Review](./BATCH-02-REVIEW.md)

## Setup

เริ่มจาก:

~~~bash
git clone https://github.com/TxngJr/ZERO-TO-ELITE-OPERATING-SYSTEMS.git
cd ZERO-TO-ELITE-OPERATING-SYSTEMS
~~~

อ่าน:

- [Fedora Setup](./SETUP-FEDORA.md)

ติดตั้ง lab tools ตามไฟล์ setup แล้วเรียนตามลำดับ

## Build

แต่ละ chapter ที่มี C examples ใช้:

~~~bash
cd 04-concurrency-I
make
~~~

หลัก compiler flags:

~~~text
-Wall -Wextra -Wpedantic -std=c17
~~~

thread examples เพิ่ม:

~~~text
-pthread
~~~

## Current mastery target

หลัง Chapter 06 ต้องอธิบายเส้นทางนี้ได้:

~~~text
Program
  ↓
Process
  ↓
CPU Context / Context Switch
  ↓
fork / exec / wait
  ↓
Thread
  ↓
Concurrency / Interleaving
  ↓
Race Condition
  ↓
Invariant / Critical Section
  ↓
Mutex / Semaphore / Atomic / CAS
~~~

และต้องเข้าใจความแตกต่างสำคัญ:

~~~text
Concurrency ≠ Parallelism
Race Condition ≠ Data Race
Atomic variable ≠ Atomic transaction
System Call ≠ Process Context Switch
Semaphore ≠ Mutex
volatile ≠ Thread Synchronization
~~~

## Next Batch

Batch 3:

- Chapter 07 — Synchronization Part II: Producer–Consumer, Readers–Writers, Dining Philosophers
- Chapter 08 — Synchronization Part III: Monitor, Condition Variable, Deadlock, Starvation, Livelock
- Chapter 09 — Scheduling: FCFS, SJF/SRTF, Priority, RR, MLFQ and metrics
