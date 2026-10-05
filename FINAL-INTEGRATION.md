# Final Integration — C# → .NET → Linux → Hardware

## Complete Chain

~~~text
C# source
↓
.NET compilation / CoreCLR
↓
Linux process
↓
managed threads mapped to OS execution threads
↓
scheduler
↓
CPU
↓
shared state + synchronization
↓
virtual address
↓
TLB / page table / MMU
↓
physical memory
↓
possible page fault
↓
kernel resolution / signal
~~~

---

## 1. Process Creation

C#:

~~~text
Process.Start
~~~

เป็น high-level runtime abstraction

OS concept:

~~~text
process creation
fork-like semantics
exec/image replacement concepts
wait/reap lifecycle
~~~

ห้ามเท่ากันแบบ 1:1 กับ syscall sequence ตายตัว

---

## 2. Threads

~~~text
Thread.Start
→ thread becomes part of execution lifecycle
→ scheduler decides actual run timing
~~~

Thread.Join:

~~~text
caller blocks until target terminates
~~~

Sleep:

~~~text
timed blocking
not a correctness protocol
~~~

---

## 3. Shared State

threads ใน process เดียวกันเห็น static fields/shared heap objects ได้

จึงเกิด:

~~~text
interleaving
→ race
→ broken invariant
~~~

ถ้า synchronization ไม่พอ

---

## 4. Two C# Locking Families Used in This Course

### System.Threading.Lock

ตรงกับ Activity 02:

~~~text
mutual exclusion
~~~

### object + Monitor

ตรงกับ Activity 03 / Thread-Safe Buffer:

~~~text
mutual exclusion
+
condition waiting
+
Pulse/PulseAll
~~~

ห้ามเอา Monitor.Wait ไปอธิบายว่าเป็น behavior ของ System.Threading.Lock โดยอัตโนมัติ

---

## 5. Blocking Connects Synchronization to Scheduling

เมื่อ thread:

- Wait
- Join
- Sleep
- waits for semaphore
- blocks on lock

มันไม่สามารถใช้ CPU ทำ useful application work ในช่วงนั้น

scheduler จึงเลือก runnable task อื่น

~~~text
Synchronization
↔
Thread State
↔
Scheduler
~~~

---

## 6. Memory Access

application pointer/reference ultimatelyเกี่ยวกับ virtual memory

concept:

~~~text
VA
→ TLB
→ page-table walk if needed
→ PTE
→ PFN + offset
→ physical access
~~~

TLB miss ไม่เท่ากับ page fault

---

## 7. Demand Paging

~~~text
mmap virtual region
→ first touch
→ page fault
→ kernel validates mapping
→ populate page
→ retry instruction
~~~

page fault จึงเป็น normal mechanism ได้

---

## 8. Protection

~~~text
mprotect read-only
→ write attempt
→ hardware protection fault
→ kernel handler
→ invalid access
→ signal path
~~~

Chapter 11 แยก failure ไป child process

---

## 9. Copy-on-Write

สอง concept ต้องแยก:

~~~text
fork COW
vs
file-backed private mapping COW
~~~

principle คล้าย:

~~~text
share backing
→ private write
→ divergence
~~~

แต่ lifecycle/source ของ mapping ต่างกัน

---

## Final Explain-It-Back

ห้ามตอบเพียง:

~~~text
.NET ทำให้เอง
OS ทำให้เอง
CPU ทำให้เอง
~~~

ต้องระบุ:

- runtime responsibility
- kernel responsibility
- scheduler responsibility
- synchronization protocol
- MMU/TLB responsibility
- process/thread state
- assumptions ของ model
