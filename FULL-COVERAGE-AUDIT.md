# Full Core Coverage Audit

## Definition of 100% in This Repository

100% หมายถึง:

~~~text
ครบ Core Scope Chapters 01–11
+
ตรงกับ supplied C# coursework
+
known correctness bugs ที่ audit พบถูกแก้
+
examples build
+
critical demos terminate
+
algorithm/invariant tests ผ่าน
~~~

ไม่ได้หมายความว่า field Operating Systems ทั้งหมดจบที่ 11 บท

---

## Source Alignment

Activity 02-1 ✅  
Activity 02-2 ✅  
Activity 03-1 ✅  
Activity 03-2 ✅  
Unsafe Buffer ✅  
Thread-Safe Buffer ✅  
Case Study 02 ✅

ดู ASSIGNMENT-MAPPING.md

---

## Chapter Coverage

01 OS / Kernel / Syscall / Runtime ✅  
02 Process / PID / State / PCB / CPU State ✅  
03 Context Switch / fork / exec / wait / zombie ✅  
04 Thread / Concurrency / Race / Critical Section ✅  
05 Shared State / RMW / Check-Then-Act / Memory Ordering ✅  
06 Lock / Monitor distinction / Semaphore / Interlocked / CAS ✅  
07 Producer–Consumer / Readers–Writers / Dining Philosophers ✅  
08 Deadlock / Starvation / Livelock / Priority Inversion ✅  
09 FCFS / SJF / SRTF / Priority / RR / MLFQ / Linux scheduler context ✅  
10 VA / PA / MMU / Page Table / TLB / Multi-Level Paging ✅  
11 Demand Paging / Page Fault / Replacement / mmap / COW / Protection ✅

---

## Correctness Fixes Included

- livelock Barrier accidental-deadlock bug fixed ✅
- MLFQ one-tick requeue bug fixed ✅
- hidden 4-KiB host-page assumption fixed ✅
- VM first-touch lab moved to mmap ✅
- mprotect child protection lab restored ✅
- background-thread demo fixed ✅
- SemaphoreSlim Release protected by finally ✅
- System.Threading.Lock vs Monitor semantics separated ✅
- file-backed COW vs fork COW separated ✅
- scheduling assumptions documented ✅
- CI upgraded from smoke-only to invariant/golden tests ✅

---

## Verification

~~~bash
./build.sh
./verify.sh
~~~

CI pins:

~~~text
Fedora 45
.NET 10 SDK feature line
~~~

---

## Remaining Topics Outside Core Scope

- Signals deep dive
- Pipes/FIFOs
- IPC/shared memory APIs
- sockets
- VFS/filesystems
- page cache deep dive
- block I/O
- device drivers
- interrupts architecture deep dive
- namespaces/cgroups
- containers
- NUMA
- allocator internals
- futex internals
- RCU
- lock-free reclamation
- io_uring
- real-time Linux
- SELinux/capabilities
- virtualization
- kernel build/debug
- writing a kernel/OS

ดังนั้นคำที่ถูกต้องคือ:

~~~text
Core 01–11 + supplied coursework alignment = complete target
Entire Operating Systems field = intentionally beyond this core
~~~
