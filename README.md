# Zero to Elite Operating Systems

หลักสูตร Operating Systems แบบลงมือทำบน **Fedora Linux / x86-64** จาก Absolute Zero → Core OS Foundations → Systems-Level Reasoning

> Primary lab environment: Acer Aspire 7 A715-43G, AMD Ryzen 7 5825U, Fedora Linux

## Core Course Status

1. ✅ Course Overview & OS Introduction
2. ✅ Process & Context Switch Part I
3. ✅ Process & Context Switch Part II
4. ✅ Concurrency Part I
5. ✅ Concurrency Part II
6. ✅ Synchronization Part I
7. ✅ Synchronization Part II
8. ✅ Synchronization Part III
9. ✅ Scheduling
10. ✅ Address Translation
11. ✅ Virtual Memory

**Core Scope 01–11 is complete.**

คำว่า complete หมายถึงครบตาม scope 11 บทนี้ ไม่ได้หมายความว่า Operating Systems ทั้งสาขามีเพียง 11 บท

---

## Start Here

1. [Course Guide](./COURSE-GUIDE.md)
2. [Fedora Setup](./SETUP-FEDORA.md)
3. [Study Order](./STUDY-ORDER.md)

---

## Batch 1 — OS and Processes

- [Chapter 01 — OS Introduction](./01-os-introduction/README.md)
- [Chapter 02 — Process & Context Switch I](./02-process-context-I/README.md)
- [Chapter 03 — Process & Context Switch II](./03-process-context-II/README.md)
- [Batch 01 Review](./BATCH-01-REVIEW.md)

## Batch 2 — Concurrency

- [Chapter 04 — Concurrency I](./04-concurrency-I/README.md)
- [Chapter 05 — Concurrency II](./05-concurrency-II/README.md)
- [Midterm Review — Chapters 01–05](./MIDTERM-REVIEW-01-05.md)
- [Chapter 06 — Synchronization I](./06-synchronization-I/README.md)
- [Batch 02 Review](./BATCH-02-REVIEW.md)

## Batch 3 — Advanced Synchronization and Scheduling

- [Chapter 07 — Synchronization II](./07-synchronization-II/README.md)
- [Chapter 08 — Synchronization III](./08-synchronization-III/README.md)
- [Chapter 09 — Scheduling](./09-scheduling/README.md)
- [Scheduling Exercises](./09-scheduling/EXERCISES.md)
- [Batch 03 Review](./BATCH-03-REVIEW.md)

## Batch 4 — Memory

- [Chapter 10 — Address Translation](./10-address-translation/README.md)
- [Chapter 11 — Virtual Memory](./11-virtual-memory/README.md)
- [Batch 04 Review](./BATCH-04-REVIEW.md)

---

## Final Integration

- [Final Integration](./FINAL-INTEGRATION.md)
- [Final Review](./FINAL-REVIEW.md)
- [Capstone](./capstone/README.md)
- [Full Core Coverage Audit](./FULL-COVERAGE-AUDIT.md)

---

## Learning Model

~~~text
Why
↓
Concept
↓
Mental Model
↓
Linux
↓
Code
↓
Run
↓
Observe
↓
Debug
↓
Explain
~~~

---

## Whole-Course Build

~~~bash
git clone https://github.com/TxngJr/ZERO-TO-ELITE-OPERATING-SYSTEMS.git
cd ZERO-TO-ELITE-OPERATING-SYSTEMS

make core-build
make python-check
make smoke
~~~

cleanup:

~~~bash
make clean
~~~

---

## Core Toolchain

~~~text
C17
GCC
POSIX Threads
Linux /proc
strace
gdb
perf
ps / pstree / top
vmstat / free / pmap
Python 3 simulators
GitHub Actions Fedora CI
~~~

---

## Simulators

Scheduling:

~~~bash
python3 09-scheduling/scheduler_sim.py --algo fcfs
python3 09-scheduling/scheduler_sim.py --algo srtf
python3 09-scheduling/scheduler_sim.py --algo rr --quantum 2
python3 09-scheduling/scheduler_sim.py --algo mlfq
~~~

Address translation:

~~~bash
python3 10-address-translation/page_table_sim.py
~~~

Page replacement:

~~~bash
python3 11-virtual-memory/page_replacement_sim.py
~~~

---

## End-to-End Knowledge Chain

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
Threads
↓
Concurrency
↓
Synchronization
↓
Deadlock / Starvation / Livelock
↓
Scheduling
↓
Virtual Address
↓
MMU / TLB / Page Table
↓
Demand Paging / Page Fault
↓
Virtual Memory / COW / mmap / Protection
~~~

---

## After Core Course

Advanced Track can continue with:

~~~text
Signals
IPC
Pipes
Shared Memory
Sockets
VFS / Filesystems
Page Cache
Block I/O
Device Drivers
Kernel Modules
Namespaces / cgroups
Containers
NUMA
SLUB
Futex
RCU
Lock-Free Programming
io_uring
Real-Time Linux
Security / SELinux / Capabilities
Virtualization
Kernel Compilation
Kernel Debugging
Building a Small OS
~~~
