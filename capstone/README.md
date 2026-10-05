# Capstone — Mini OS Behavior Laboratory

## Goal

สร้าง laboratory repository section ที่พิสูจน์ concepts จาก Chapters 01–11 แบบ end-to-end

ไม่ใช่การเขียน kernel ใหม่ แต่เป็นการพิสูจน์ behavior ของ OS จริงจาก user space

---

## Required Modules

~~~text
capstone-work/
├── 01-system-call/
├── 02-process/
├── 03-fork-exec-wait/
├── 04-threads/
├── 05-race/
├── 06-synchronization/
├── 07-classic-sync/
├── 08-deadlock/
├── 09-scheduling/
├── 10-address-translation/
├── 11-virtual-memory/
└── REPORT.md
~~~

---

## Module 01 — System Call

ต้อง:

- เขียน Hello C
- trace ด้วย strace
- แยก library call vs syscall
- อธิบาย user/kernel transition

Evidence:

- source
- command
- selected output
- explanation

---

## Module 02 — Process

ต้องสังเกต:

- PID
- PPID
- process state
- /proc/PID/status
- /proc/PID/maps
- /proc/PID/fd

---

## Module 03 — fork / exec / wait

ต้อง:

- create child
- exec external program
- wait status
- explain zombie window
- explain COW connection

---

## Module 04 — Threads

ต้อง:

- create multiple pthreads
- show PID/TIDs
- ps -L
- explain stack/shared address space

---

## Module 05 — Race

ต้องสร้าง logical lost update แบบ defined behavior ตามแนว Chapter 04

รายงาน:

- invariant
- interleaving
- observed failure

---

## Module 06 — Synchronization

แก้ race ด้วยอย่างน้อย:

- mutex
- atomic RMW

จากนั้นเปรียบเทียบ:

- correctness
- complexity
- performance assumptions

---

## Module 07 — Classic Synchronization

เลือกอย่างน้อย 2:

- Producer–Consumer
- Readers–Writers
- Dining Philosophers

ต้องเขียน invariant และ waiting conditions

---

## Module 08 — Deadlock

ห้ามสร้าง infinite hang เป็นหลักฐานเพียงอย่างเดียว

ใช้:

- wait-for graph
- cycle detector
- lock-order reasoning

อธิบาย Coffman Conditions

---

## Module 09 — Scheduling

สร้าง workload อย่างน้อย 5 processes

ทำ:

- FCFS
- SRTF
- RR
- MLFQ

รายงาน:

- timeline
- TAT
- WT
- RT

---

## Module 10 — Address Translation

ต้อง:

- query page size
- split VA → VPN + offset
- inspect /proc/PID/maps
- run page-table simulator
- explain TLB miss vs page fault

---

## Module 11 — Virtual Memory

ต้อง:

- observe page faults
- demonstrate COW
- demonstrate mmap file
- demonstrate protection fault safely
- compare FIFO/LRU/Clock

---

## REPORT.md Required Sections

~~~text
1. Environment
2. Kernel Version
3. CPU Topology
4. Experiments
5. Predictions
6. Observations
7. Explanations
8. Failed Predictions
9. Corrections
10. Final Mental Model
~~~

---

## Capstone Rule

ทุก experiment ต้องมี:

~~~text
Prediction
Command / Code
Observed Result
Why It Happened
What Would Break the Model
Connection to Another Chapter
~~~

---

## Graduation Test

อธิบาย scenario เดียว:

~~~text
launch program
→ process
→ scheduler
→ thread
→ synchronized shared state
→ virtual memory access
→ TLB/page table
→ possible page fault
→ resume
→ exit/reap
~~~

ถ้าอธิบายได้พร้อม evidence จาก labs แสดงว่า Core Course ถูก integrate จริง
