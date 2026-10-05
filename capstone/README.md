# Capstone — C# OS Behavior Laboratory

## Goal

รวม Chapters 01–11 เป็น C# console laboratory เดียว โดยทุก module ต้องมี:

~~~text
Prediction
→ Run
→ Observation
→ Explanation
→ Invariant
→ Failure Mode
→ Verification
~~~

---

## Required Modules

1. Syscall observation
2. Process inspector
3. Child process lifecycle
4. Thread IDs / scheduler observation
5. Race condition
6. Lock / Monitor / Interlocked / Semaphore
7. Producer–Consumer
8. Deadlock graph / bounded livelock
9. Scheduling simulator
10. Address translation / TLB simulator
11. Virtual memory / mmap / mprotect / page replacement

---

## Required Final Menu

~~~text
1  Process Info
2  Child Process
3  Thread Demo
4  Race Demo
5  System.Threading.Lock Demo
6  Monitor Producer Consumer
7  Deadlock Graph
8  Livelock Demo
9  Scheduling
10 Address Translation
11 TLB
12 Demand Paging
13 Page Protection
14 Page Replacement
15 Memory-Mapped File / COW
~~~

---

## Correctness Requirements

### Threads

ห้ามใช้ Sleep เป็น completion synchronization

### Producer–Consumer

ต้องมี:

- bounded capacity
- Count invariant
- while around Wait
- producer completion state
- clean consumer termination

### Deadlock/Livelock

ห้ามใช้ intentional infinite hang เป็นหลักฐานหลัก

ต้องมี timeout/bounded experiment

### Scheduling

ต้องมี golden test สำหรับ:

- FCFS
- RR
- MLFQ quantum semantics

### Memory

ต้องแยก:

- host page size
- simulator page size
- TLB miss
- page fault
- protection fault
- file COW
- fork COW concept

---

## Report

ทุก module:

1. Goal
2. Prediction
3. C# Code
4. Command
5. Observed Output
6. OS Explanation
7. Invariant
8. Common Bug
9. Fix
10. Connection to Another Chapter

---

## Graduation Test

อธิบาย end-to-end:

~~~text
C# Program
→ .NET Runtime
→ Linux Process/Thread
→ Scheduler
→ CPU
→ Synchronization
→ Virtual Address
→ TLB/Page Table/MMU
→ Possible Page Fault
→ Kernel Resolution/Signal
~~~

พร้อมบอกว่าอะไรเป็น:

- C# abstraction
- runtime behavior
- kernel behavior
- hardware behavior
