# Final Review — Chapters 01–11

## 1. End-to-End Knowledge Chain

~~~text
C# Application
↓
.NET Runtime
↓
Linux Process
↓
Threads
↓
Scheduler
↓
Concurrency
↓
Synchronization
↓
Virtual Address
↓
TLB / Page Table / MMU
↓
Physical Memory
↓
Possible Page Fault
↓
Kernel Resolution / Signal
~~~

---

# 2. Must Explain Without Notes

## OS / Process

1. OS vs Kernel
2. User vs Kernel Space
3. System Call
4. Program vs Process
5. PID/PPID/TID
6. PCB abstraction
7. Process State
8. Context Switch vs Mode Switch
9. fork
10. exec
11. wait
12. zombie

## Concurrency

13. Thread
14. Start / Join / Sleep
15. Concurrency vs Parallelism
16. Race Condition
17. Critical Section
18. Read-Modify-Write
19. Check-Then-Act
20. Atomicity / Visibility / Ordering

## Synchronization

21. System.Threading.Lock
22. object monitor lock
23. Monitor.Wait
24. Pulse / PulseAll
25. Interlocked
26. CompareExchange
27. SemaphoreSlim
28. Producer–Consumer
29. ReaderWriterLockSlim
30. Dining Philosophers
31. Deadlock
32. Coffman Conditions
33. Starvation
34. Livelock
35. Priority Inversion

## Scheduling

36. FCFS
37. SJF
38. SRTF
39. Priority
40. RR
41. MLFQ
42. CT/TAT/WT/RT
43. Linux fair scheduling vs textbook algorithms

## Memory

44. VA vs PA
45. Page vs Frame
46. VPN/PFN/Offset
47. MMU
48. PTE
49. TLB
50. TLB miss vs page fault
51. Demand Paging
52. Minor/Major fault
53. mmap
54. mprotect
55. SIGSEGV
56. FIFO/OPT/LRU/CLOCK
57. Belady
58. Locality
59. Working Set
60. Thrashing
61. Swap/zram
62. File mapping
63. File-backed COW vs fork COW

---

# 3. Source-Aligned Final

ต้องอธิบายไฟล์เรียน:

- Activity 02 sequential
- Activity 02 threaded + Lock
- Activity 03 one reader
- Activity 03 three readers
- unsafe buffer
- thread-safe buffer
- Case Study local reduction

ดู:

- ASSIGNMENT-MAPPING.md
- SOURCE-ALIGNED-EXERCISES.md
- SOURCE-ALIGNED-ANSWERS.md

---

# 4. Calculation Gate

## Scheduling

~~~text
TAT = CT - AT
WT  = TAT - BT
RT  = FirstStart - AT
~~~

ภายใต้ assumptions ของ Chapter 09

## Address Translation

4 KiB simulated page:

~~~text
VPN = VA >> 12
offset = VA & 0xFFF
PA = PFN * 4096 + offset
~~~

## Replacement

ต้อง trace FIFO / LRU / OPT / CLOCK ได้

---

# 5. Debugging Gate

ให้ broken code แล้วต้องหา:

- lost update
- check-then-act
- missing Join
- Wait outside lock
- if instead of while
- missing PulseAll
- missing producer termination state
- inconsistent lock order
- semaphore permit leak
- incorrect MLFQ quantum
- page-size assumption bug

---

# 6. Linux Observation Gate

~~~bash
strace
ps
ps -L
pstree
/proc/PID/status
/proc/PID/maps
/proc/PID/task
pmap
chrt
taskset
free
vmstat
swapon
zramctl
~~~

---

# 7. Programming Final

เขียน C# bounded buffer:

- producer 2
- consumer 3
- ring buffer
- Front/Back/Count
- object monitor
- while + Monitor.Wait
- PulseAll
- producer completion count
- Start / Join
- clean termination

จากนั้นเขียน invariants และอธิบาย scheduler interactions

---

# 8. Elite Gate

ไม่พอที่จะเขียน code ได้

ต้องตอบ:

~~~text
Why correct?
What invariant?
What can race?
What blocks?
What wakes it?
What is OS responsibility?
What is runtime responsibility?
What is hardware responsibility?
What would fail if assumption changes?
~~~
