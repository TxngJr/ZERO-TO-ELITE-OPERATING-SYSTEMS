# Full Core Coverage Audit

## Audit Scope

Audit นี้ตรวจ **Core Curriculum ที่กำหนด 1–11**

ไม่ใช่การอ้างว่าครอบคลุม Operating Systems ทุกสาขาในโลก

---

## Chapter 01 — OS Introduction

- OS / Kernel ✅
- User / Kernel Space ✅
- Hardware–OS–Application ✅
- Privilege ✅
- System Calls ✅
- Boot overview ✅
- Fedora observation ✅
- /proc / strace ✅

## Chapter 02 — Process I

- Program vs Process ✅
- PID / PPID ✅
- Process States ✅
- PCB abstraction ✅
- task_struct relation ✅
- CPU Context ✅
- Address-space overview ✅
- /proc/PID ✅

## Chapter 03 — Process II

- Context Switch ✅
- Mode vs Process Switch ✅
- fork ✅
- exec ✅
- wait/waitpid ✅
- Zombie ✅
- Reparenting ✅
- Copy-on-Write intro ✅

## Chapter 04 — Concurrency I

- Process vs Thread ✅
- pthread_create/join ✅
- Concurrency vs Parallelism ✅
- Interleaving ✅
- Race Condition ✅
- Critical Section ✅
- Atomicity intro ✅
- Linux TID observation ✅

## Chapter 05 — Concurrency II

- Multithreading lifecycle ✅
- Detached Threads ✅
- Shared Mutable State ✅
- Read–Modify–Write ✅
- Check–Then–Act ✅
- Data Race vs Race Condition ✅
- Thread Safety ✅
- Reentrancy ✅
- Memory Visibility ✅
- Acquire/Release intro ✅

## Chapter 06 — Synchronization I

- Mutex ✅
- Lock concept ✅
- Semaphore ✅
- Binary/Counting Semaphore ✅
- Atomic RMW ✅
- CAS ✅
- Spin vs Block ✅
- Safety/Liveness intro ✅

## Chapter 07 — Synchronization II

- Producer–Consumer ✅
- Bounded Buffer ✅
- Readers–Writers ✅
- Dining Philosophers ✅
- rwlock ✅
- Fairness concerns ✅
- Resource Ordering ✅

## Chapter 08 — Synchronization III

- Monitor ✅
- Condition Variable ✅
- Signal/Broadcast ✅
- Deadlock ✅
- Coffman Conditions ✅
- Resource/Wait-For Graph ✅
- Prevention/Avoidance/Detection/Recovery ✅
- Starvation ✅
- Livelock ✅
- Priority Inversion/Inheritance ✅

## Chapter 09 — Scheduling

- Scheduler/Dispatcher ✅
- FCFS ✅
- SJF ✅
- SRTF ✅
- Priority ✅
- Round Robin ✅
- MLFQ ✅
- TAT/WT/RT ✅
- Convoy/Starvation/Aging ✅
- Linux scheduler distinction ✅
- Simulator ✅

## Chapter 10 — Address Translation

- Virtual vs Physical Address ✅
- MMU ✅
- Paging ✅
- Page / Frame ✅
- VPN / Offset ✅
- Page Table / PTE ✅
- Multi-Level Page Tables ✅
- TLB ✅
- TLB vs Page Fault ✅
- Context Switch / Translation State ✅
- /proc maps / pmap ✅

## Chapter 11 — Virtual Memory

- VM abstraction ✅
- Demand Paging ✅
- Page Fault ✅
- Minor/Major Fault ✅
- FIFO/OPT/LRU/Clock ✅
- Belady's Anomaly ✅
- Locality ✅
- Working Set ✅
- Thrashing ✅
- Swap / zram concept ✅
- mmap ✅
- MAP_PRIVATE / MAP_SHARED ✅
- Copy-on-Write ✅
- mprotect ✅
- SIGSEGV relation ✅

---

## Cross-Course Requirements

| Requirement | Status |
|---|---:|
| Thai-first explanations | ✅ |
| English technical terms retained | ✅ |
| Fedora commands | ✅ |
| C labs | ✅ |
| Python simulators | ✅ |
| ASCII mental models | ✅ |
| Exercises | ✅ |
| Quizzes | ✅ |
| Explain-It-Back | ✅ |
| Misconceptions | ✅ |
| Debugging/Observation | ✅ |
| Midterm review | ✅ |
| Batch reviews | ✅ |
| Final integration | ✅ |
| Final review | ✅ |
| Capstone | ✅ |
| Automated CI | ✅ |

---

## What Is Intentionally Outside Core 1–11

Advanced Track candidates:

- Signals deep dive
- Pipes / FIFOs / shared-memory IPC
- Sockets
- VFS / filesystems
- page cache deep dive
- block I/O
- device drivers
- interrupts/exceptions deep dive
- kernel modules
- namespaces / cgroups
- containers
- NUMA
- SLUB
- futex internals
- RCU
- lock-free reclamation
- io_uring
- real-time Linux
- security/capabilities/SELinux
- virtualization
- kernel build/debugging
- writing a small OS/kernel

ดังนั้น repository นี้สามารถกล่าวได้ว่า:

~~~text
Core Scope 01–11: covered
Entire field of Operating Systems: intentionally not claimed complete
~~~
