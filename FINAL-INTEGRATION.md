# Final Integration — From Command to CPU to Memory

เป้าหมายของไฟล์นี้คือเชื่อม Chapters 01–11 เป็น model เดียว

---

# Scenario

ผู้ใช้พิมพ์:

~~~bash
./my-program
~~~

## Stage 1 — Shell and Program Execution

~~~text
Shell
↓
process creation path
↓
fork-like semantics
↓
child execution context
↓
exec
↓
new program image
~~~

เชื่อมกับ Chapters 01–03

---

## Stage 2 — Process State

kernel ต้อง track:

- process/thread identity
- runnable/blocking state
- CPU context
- virtual address space
- open resources
- credentials
- scheduling metadata

textbook PCB เป็น abstraction ของข้อมูลเหล่านี้

---

## Stage 3 — Scheduler

~~~text
ready/runnable
↓
scheduler decision
↓
dispatcher/context switch
↓
CPU executes task
~~~

เชื่อม Chapter 09

---

## Stage 4 — Threads

~~~text
Process
├── Thread A
├── Thread B
└── Thread C
~~~

แต่ละ thread มี execution context และ stack ของตน แต่ share process resources ตาม model

---

## Stage 5 — Concurrency

~~~text
A reads shared state
B reads shared state
A writes
B writes
~~~

ถ้า algorithm ไม่ synchronize:

- race
- broken invariant

---

## Stage 6 — Synchronization

program อาจใช้:

- mutex
- semaphore
- atomic RMW
- CAS
- condition variable
- rwlock

เพื่อสร้าง:

- mutual exclusion
- ordering
- visibility
- resource limits

---

## Stage 7 — Multi-Resource Problems

~~~text
T1 holds A waits B
T2 holds B waits A
~~~

เกิด deadlock ได้

ต้องคิด:

- lock ordering
- Coffman Conditions
- starvation
- livelock
- priority inversion

---

## Stage 8 — Memory Access

CPU instruction access pointer:

~~~text
Virtual Address
↓
VPN + offset
↓
TLB
↓
possible page-table walk
↓
PTE
↓
Physical Frame + offset
~~~

เชื่อม Chapter 10

---

## Stage 9 — Page Fault

ถ้า mapping valid แต่ page ยังไม่ populated:

~~~text
access
↓
page fault
↓
kernel allocates/resolves
↓
page table updated
↓
instruction retries
~~~

ถ้า invalid/protected:

~~~text
fault
↓
kernel cannot resolve
↓
signal such as SIGSEGV
~~~

---

## Stage 10 — fork and COW Revisited

~~~text
parent mappings
↓
fork
↓
child mappings initially share backing where possible
↓
write
↓
COW fault
↓
private copy
~~~

Chapter 03 concept จึงเชื่อมตรงกับ Chapter 11

---

## Stage 11 — Blocking and Rescheduling

ถ้า thread:

- waits on condition variable
- waits for I/O
- sleeps
- blocks on lock

มันอาจออกจาก runnable execution

~~~text
Synchronization
↔
Process State
↔
Scheduler
~~~

---

## Stage 12 — Exit

~~~text
exit
↓
kernel releases process resources
↓
termination state
↓
parent wait/reap
~~~

กลับไป Chapter 03

---

# Complete Mental Model

~~~text
User command
↓
Shell
↓
System Calls
↓
Process creation
↓
fork / exec
↓
Virtual address space
↓
Runnable task
↓
Scheduler
↓
CPU execution
↓
Threads
↓
Concurrency
↓
Synchronization
↓
Memory access
↓
TLB / Page Table / MMU
↓
Possible Page Fault
↓
Kernel resolution
↓
Execution continues
↓
Exit
↓
wait / reap
~~~

---

# Final Explain-It-Back

อธิบาย flow ด้านบนโดยห้ามใช้คำว่า:

~~~text
"OS ทำให้เอง"
~~~

โดยไม่ขยายว่า subsystem ไหนทำอะไร

ต้องระบุให้ได้:

- CPU
- scheduler
- kernel
- user-space runtime
- MMU
- TLB
- page table
- synchronization primitive
- process/thread state
