# Study Order

## Phase 0 — Align with Class Material

1. README.md
2. ASSIGNMENT-MAPPING.md
3. SOURCE-ALIGNED-EXERCISES.md
4. COURSE-GUIDE.md
5. SETUP-FEDORA.md
6. CSHARP-OS-MAPPING.md

---

## Phase 1 — OS and Processes

1. Chapter 01
2. Chapter 02
3. Chapter 03
4. BATCH-01-REVIEW.md

Gate:

~~~text
OS / Kernel / Syscall
Process / PID / State
Context Switch
fork / exec / wait / zombie
C# Process mapping
~~~

---

## Phase 2 — Threads / Midterm

1. Chapter 04
2. Chapter 05
3. MIDTERM-REVIEW-01-05.md

ทำ Source-Aligned Exercises Parts A–C

Gate:

~~~text
Thread Start/Join
Concurrency
Race
Shared State
Atomicity / Visibility / Ordering
Activity 02 / 03 explanation
~~~

---

## Phase 3 — Synchronization

1. Chapter 06
2. Chapter 07
3. Chapter 08
4. BATCH-02-REVIEW.md
5. BATCH-03-REVIEW.md

ทำ Source-Aligned Exercises Parts D–F

Gate:

~~~text
System.Threading.Lock
object + Monitor
SemaphoreSlim
Interlocked
Producer–Consumer
Deadlock / Starvation / Livelock
~~~

---

## Phase 4 — Scheduling

1. Chapter 09
2. Scheduling exercises
3. run scheduler self-test

Gate:

~~~text
FCFS/SJF/SRTF/Priority/RR/MLFQ
CT/TAT/WT/RT
textbook vs Linux scheduler
~~~

---

## Phase 5 — Memory

1. Chapter 10
2. Chapter 11
3. BATCH-04-REVIEW.md

Gate:

~~~text
VA/PA
MMU/TLB/Page Table
Demand Paging
Page Fault
mmap/mprotect
Replacement
COW
~~~

---

## Phase 6 — Final Integration

1. FINAL-INTEGRATION.md
2. FINAL-REVIEW.md
3. CORRECTNESS-GATES.md
4. capstone/README.md
5. FULL-COVERAGE-AUDIT.md

---

# Rule

อย่าไปบทถัดไปเพียงเพราะอ่านจบ

ไปต่อเมื่อ:

~~~text
อธิบายเองได้
+
trace ได้
+
เขียน C# ได้
+
หา bug ได้
+
รัน lab ได้
+
Linux observation ได้
+
self-test/invariant ผ่าน
~~~
