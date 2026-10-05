# Final Review — Chapters 01–11

## 1. Core Knowledge Map

~~~text
Hardware
↓
Kernel / User Space
↓
System Calls
↓
Process / CPU Context
↓
fork / exec / wait
↓
Threads
↓
Concurrency / Interleaving
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
Virtual Memory
~~~

---

## 2. Must Explain Without Notes

1. OS vs Kernel
2. User Space vs Kernel Space
3. System Call
4. Program vs Process
5. PID / PPID
6. PCB abstraction
7. Process State
8. CPU Context
9. Context Switch vs Mode Switch
10. fork / exec / wait
11. Zombie / reaping
12. Process vs Thread
13. Concurrency vs Parallelism
14. Race Condition vs Data Race
15. Critical Section
16. Mutex
17. Semaphore
18. Atomic RMW / CAS
19. Producer–Consumer
20. Readers–Writers
21. Dining Philosophers
22. Monitor / Condition Variable
23. Deadlock / Coffman Conditions
24. Starvation
25. Livelock
26. Priority Inversion
27. FCFS / SJF / SRTF
28. Priority / RR / MLFQ
29. TAT / WT / RT
30. Virtual vs Physical Address
31. Page / Frame
32. MMU
33. Page Table / PTE
34. TLB
35. Demand Paging
36. Minor / Major Fault
37. FIFO / OPT / LRU / Clock
38. Belady's Anomaly
39. Working Set / Thrashing
40. Swap / zram concept
41. mmap
42. Copy-on-Write
43. mprotect / SIGSEGV relationship

---

## 3. Cross-Chapter Questions

### Q1

ทำไม system call ไม่เท่ากับ process context switch

### Q2

fork เกี่ยวข้องกับ virtual memory อย่างไร

### Q3

thread synchronization เกี่ยวข้องกับ scheduler อย่างไร

### Q4

condition variable wait ทำให้ thread state เปลี่ยนอย่างไร

### Q5

deadlock กับ scheduling starvation ต่างกันอย่างไร

### Q6

TLB miss ต่างจาก page fault อย่างไร

### Q7

page fault ที่ valid ทำไมไม่ถือว่า application error

### Q8

ทำไม same virtual address ใน parent/child หลัง fork ไม่ได้แปลว่าจะเห็นค่าเดียวกันหลัง write

### Q9

ทำไม mutex correctness เกี่ยวกับ memory visibility ไม่ใช่แค่ "กันสองคนเข้า"

### Q10

ทำไม textbook scheduler simulator ไม่ใช่ Linux scheduler implementation

---

## 4. Calculation Section

ต้องทำได้:

### Scheduling

~~~text
TAT = CT - AT
WT  = TAT - BT
RT  = first_start - AT
~~~

ทำ Gantt Chart ของ:

- FCFS
- SJF
- SRTF
- Priority
- RR

### Address Translation

ถ้า page size = 4 KiB:

~~~text
offset bits = 12
VPN = VA >> 12
offset = VA & 0xFFF
PA = (PFN << 12) | offset
~~~

### Paging

คำนวณ:

- จำนวน pages จาก region size
- FIFO faults
- LRU faults
- Belady example

---

## 5. Linux Observation Gate

ต้องรู้ว่าจะใช้ tool ไหน:

| Question | Tool |
|---|---|
| kernel version | uname -r |
| CPU topology | lscpu |
| process tree | pstree |
| process state | ps |
| process mappings | /proc/PID/maps |
| open FDs | /proc/PID/fd |
| system calls | strace |
| threads | ps -L |
| scheduler class | ps / chrt |
| CPU affinity | taskset |
| memory summary | free / vmstat |
| swap | swapon |
| mappings summary | pmap |
| debug crash | gdb |

---

## 6. Final Practice Scenarios

### Scenario A — Shell to Exit

อธิบาย:

~~~text
shell
→ fork-like creation
→ exec
→ scheduler
→ CPU
→ memory accesses
→ exit
→ wait
~~~

### Scenario B — Concurrent Counter

อธิบาย:

~~~text
threads
→ interleaving
→ lost update
→ invariant
→ mutex or atomic RMW
~~~

### Scenario C — Producer Consumer

อธิบาย:

~~~text
buffer empty/full
→ mutex
→ condition variable
→ block
→ wake
→ recheck predicate
~~~

### Scenario D — Deadlock

~~~text
T1 holds A waits B
T2 holds B waits A
~~~

ระบุ Coffman Conditions และแก้ด้วย lock ordering

### Scenario E — First Memory Touch

~~~text
mmap
→ VA mapping
→ first access
→ page fault
→ kernel resolution
→ retry
~~~

---

## 7. Final Exam Style

แนะนำ 100 คะแนน:

- 20 — OS/Process
- 20 — Concurrency/Synchronization
- 20 — Deadlock/Scheduling
- 20 — Address Translation
- 20 — Virtual Memory

ห้ามวัดเฉพาะ definition

ควรมี:

- trace
- calculate
- explain output
- fix broken algorithm
- design
- Linux observation

---

## 8. Final Mastery Gate

ผ่าน Core เมื่อสามารถ:

~~~text
Explain
+
Trace
+
Calculate
+
Code
+
Observe
+
Debug
+
Design
~~~

ได้ตลอด chain Chapters 01–11
