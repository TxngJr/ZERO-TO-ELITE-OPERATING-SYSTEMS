# Study Order

## Phase 0 — Setup

1. README.md
2. COURSE-GUIDE.md
3. SETUP-FEDORA.md

---

## Phase 1 — OS and Processes

1. 01-os-introduction
2. 02-process-context-I
3. 03-process-context-II
4. BATCH-01-REVIEW.md

Checkpoint:

~~~text
OS
Kernel/User Space
System Call
Program/Process
PID/PPID
Process State
CPU Context
fork/exec/wait
Zombie
~~~

---

## Phase 2 — Concurrency and Basic Synchronization

1. 04-concurrency-I
2. 05-concurrency-II
3. MIDTERM-REVIEW-01-05.md
4. 06-synchronization-I
5. BATCH-02-REVIEW.md

Checkpoint:

~~~text
Thread
Concurrency
Parallelism
Interleaving
Race Condition
Data Race
Critical Section
Mutex
Semaphore
Atomic RMW
CAS
~~~

---

## Phase 3 — Advanced Synchronization and Scheduling

1. 07-synchronization-II
2. 08-synchronization-III
3. 09-scheduling
4. BATCH-03-REVIEW.md

Checkpoint:

~~~text
Producer–Consumer
Readers–Writers
Dining Philosophers
Monitor
Condition Variable
Deadlock
Starvation
Livelock
Priority Inversion
FCFS/SJF/SRTF/Priority/RR/MLFQ
Scheduling Metrics
~~~

---

## Phase 4 — Memory

1. 10-address-translation
2. 11-virtual-memory
3. BATCH-04-REVIEW.md

Checkpoint:

~~~text
VA/PA
MMU
Page/Frame
Page Table
TLB
Demand Paging
Page Fault
Replacement
Swap
mmap
COW
Protection
~~~

---

## Phase 5 — Integration

1. FINAL-INTEGRATION.md
2. FINAL-REVIEW.md
3. capstone/README.md
4. FULL-COVERAGE-AUDIT.md

---

## Recommended Rule

อย่าเรียนต่อเพียงเพราะอ่านบทก่อนจบ

ไปบทถัดไปเมื่อสามารถ:

~~~text
อธิบายโดยไม่เปิดโน้ต
+
รัน lab ได้
+
ทำนาย output หลักได้
+
แก้ exercise ระดับ trace/design ได้
~~~
