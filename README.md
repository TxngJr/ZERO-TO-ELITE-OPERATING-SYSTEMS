# Zero to Elite Operating Systems

หลักสูตร Operating Systems แบบลงมือทำบน **Fedora Linux / x86-64** จาก Absolute Zero ไปสู่ระดับ Systems Engineer

> Primary lab environment: Acer Aspire 7 A715-43G, AMD Ryzen 7 5825U, Fedora Linux

## Learning model

~~~text
Why → Concept → Mental Model → Linux → Code → Run → Observe → Debug → Explain
~~~

## Core roadmap

1. ✅ Course Overview & OS Introduction
2. ✅ Process & Context Switch Part I
3. ✅ Process & Context Switch Part II
4. ✅ Concurrency Part I
5. ✅ Concurrency Part II
6. ✅ Synchronization Part I
7. ✅ Synchronization Part II
8. ✅ Synchronization Part III
9. ✅ Scheduling
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

## Batch 3

- [Chapter 07 — Synchronization Part II](./07-synchronization-II/README.md)
- [Chapter 08 — Synchronization Part III](./08-synchronization-III/README.md)
- [Chapter 09 — Scheduling](./09-scheduling/README.md)
- [Scheduling Exercises](./09-scheduling/EXERCISES.md)
- [Scheduling References](./09-scheduling/REFERENCES.md)
- [Batch 03 Review](./BATCH-03-REVIEW.md)

## Setup

~~~bash
git clone https://github.com/TxngJr/ZERO-TO-ELITE-OPERATING-SYSTEMS.git
cd ZERO-TO-ELITE-OPERATING-SYSTEMS
~~~

อ่าน [SETUP-FEDORA.md](./SETUP-FEDORA.md)

## Current knowledge chain

~~~text
Hardware
  ↓
Kernel / User Space
  ↓
System Calls
  ↓
Process / Context Switch
  ↓
fork / exec / wait
  ↓
Threads / Concurrency
  ↓
Race Conditions
  ↓
Mutex / Semaphore / Atomics
  ↓
Producer–Consumer / Readers–Writers / Dining Philosophers
  ↓
Deadlock / Starvation / Livelock
  ↓
CPU Scheduling
~~~

## Current practical programs

หลักสูตรตอนนี้มี labs สำหรับ:

- syscall observation
- process/procfs inspection
- fork/exec/wait/zombie
- POSIX threads
- race conditions
- release/acquire atomics
- mutex/semaphore/CAS
- producer–consumer
- readers–writers
- dining philosophers
- monitor-like queue
- wait-for cycle detection
- bounded livelock
- CPU scheduling simulation

## Scheduling Simulator

~~~bash
python3 09-scheduling/scheduler_sim.py --algo fcfs
python3 09-scheduling/scheduler_sim.py --algo srtf
python3 09-scheduling/scheduler_sim.py --algo rr --quantum 2
python3 09-scheduling/scheduler_sim.py --algo mlfq
~~~

รองรับ:

~~~text
FCFS
SJF
SRTF
Priority
Preemptive Priority
Round Robin
MLFQ
~~~

## Next Batch

Batch 4 จะปิด Core Curriculum:

- Chapter 10 — Address Translation
- Chapter 11 — Virtual Memory
- Final Integration
- Final Review
- Capstone
- Full Course Coverage Audit
