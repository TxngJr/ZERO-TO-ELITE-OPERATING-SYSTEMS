# Batch 03 Review — Chapters 07–09

## Scope

- Chapter 07 — Synchronization Part II
- Chapter 08 — Synchronization Part III
- Chapter 09 — Scheduling

---

## 1. Dependency Map

~~~text
Mutex / Semaphore / Atomic
        ↓
Classic synchronization problems
        ↓
Condition variables / Monitor
        ↓
Multiple-resource dependencies
        ↓
Deadlock / Starvation / Livelock
        ↓
Scheduler chooses runnable work
        ↓
FCFS / SJF / SRTF / Priority / RR / MLFQ
        ↓
Modern Linux scheduling concepts
~~~

---

## 2. Coverage Matrix

| Topic | Theory | Diagram/Trace | Code | Lab | Exercises | Quiz |
|---|---:|---:|---:|---:|---:|---:|
| Producer–Consumer | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Readers–Writers | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Dining Philosophers | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Monitor | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Condition Variable | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Deadlock | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Coffman Conditions | ✅ | ✅ | — | ✅ | ✅ | ✅ |
| Resource/Wait-For Graph | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Starvation | ✅ | ✅ | — | ✅ | ✅ | ✅ |
| Livelock | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Priority Inversion | ✅ | ✅ | — | ✅ | ✅ | ✅ |
| FCFS | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| SJF/SRTF | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Priority Scheduling | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Round Robin | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| MLFQ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Scheduling Metrics | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Linux EEVDF/CFS context | ✅ | ✅ | — | ✅ | ✅ | ✅ |

---

## 3. Correctness Audit

### Condition Variables

ทุก wait pattern ใช้:

~~~text
lock
while !predicate:
    cond_wait
operate
signal
unlock
~~~

ไม่ใช้ if เพื่อ assume wakeup = predicate true

### Dining Philosophers

demo ไม่ intentionally deadlock เครื่อง

ใช้ global resource ordering เพื่อทำลาย circular wait

### Deadlock Lab

ใช้ graph detector เพื่อสอน cycle โดยไม่สร้าง infinite blocked program

### Livelock Lab

bounded demo จบได้เสมอ และระบุชัดว่า true livelock อาจไม่จบ

### Scheduler

simulator ระบุ assumptions:

- single CPU
- integer time
- one CPU burst
- context-switch cost zero
- lower numeric priority = higher priority

MLFQ ถูกระบุว่าเป็น teaching model ไม่ใช่ Linux implementation

---

## 4. Formula Gate

ต้องจำจากความเข้าใจ:

~~~text
TAT = CT - AT
WT  = TAT - BT
RT  = first_start - AT
~~~

และต้องรู้ว่าทำไม:

~~~text
WT != first_start - arrival
~~~

สำหรับ preemptive workload ทั่วไป

---

## 5. Scheduling Comparison Gate

| Algorithm | Preemptive? | Main rule | Major weakness |
|---|---|---|---|
| FCFS | No | arrival order | convoy |
| SJF | No | shortest burst | future burst unknown/starvation |
| SRTF | Yes | shortest remaining | long-job starvation |
| Priority | either | highest priority | starvation |
| RR | Yes | time quantum | quantum trade-off |
| MLFQ | Yes-style | feedback queues | policy complexity |

---

## 6. Linux Reality Gate

ต้องอธิบายได้ว่า:

- textbook scheduling เป็น model
- Linux มี scheduling classes/policies
- normal fair scheduling สมัยใหม่กำลังใช้ EEVDF concepts
- CFS เป็น historical/foundational predecessor ที่ยังสำคัญต่อความเข้าใจ
- multicore scheduling มี placement/load-balancing problems เพิ่ม
- nice ไม่ใช่ "ลำดับรันตายตัว"
- affinity ไม่ใช่ priority

---

## 7. Practical Gate

### Build C labs

~~~bash
make -C 07-synchronization-II clean all
make -C 08-synchronization-III clean all
~~~

### Scheduling smoke tests

~~~bash
python3 09-scheduling/scheduler_sim.py --algo fcfs
python3 09-scheduling/scheduler_sim.py --algo sjf
python3 09-scheduling/scheduler_sim.py --algo srtf
python3 09-scheduling/scheduler_sim.py --algo priority
python3 09-scheduling/scheduler_sim.py --algo priority-preemptive
python3 09-scheduling/scheduler_sim.py --algo rr --quantum 2
python3 09-scheduling/scheduler_sim.py --algo mlfq
~~~

---

## 8. Explain-It-Back Gate

ตอบโดยไม่เปิดโน้ต:

1. Producer–Consumer ต้องมี predicates อะไร
2. Condition Variable ทำไมต้องใช้ while
3. Readers–Writers trade-off คืออะไร
4. Dining Philosophers deadlock เพราะอะไร
5. Coffman Conditions 4 ข้อ
6. Deadlock vs Starvation vs Livelock
7. Priority inversion คืออะไร
8. FCFS convoy effect
9. SJF vs SRTF
10. Priority starvation
11. RR quantum trade-off
12. MLFQ feedback/boost
13. WT/TAT/RT
14. Linux scheduler ทำไมไม่เท่ากับ textbook RR/SJF

ผ่านทั้งหมดแล้วจึงเข้าสู่ Address Translation และ Virtual Memory
