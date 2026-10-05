# Chapter 09 — CPU Scheduling

## เป้าหมาย

หลังบทนี้ต้อง:

- อธิบาย Scheduler / Dispatcher / Ready Queue
- แยก preemptive vs non-preemptive
- คำนวณ CT/TAT/WT/RT
- วาด Gantt Chart
- วิเคราะห์ FCFS, SJF, SRTF, Priority, RR, MLFQ
- เข้าใจ starvation/aging/priority boost
- เข้าใจ simulator assumptions
- แยก textbook scheduler จาก Linux scheduler จริง

---

## 1. Scheduling Problem

เมื่อ runnable tasks มากกว่า CPU execution slots:

~~~text
Ready tasks
↓
Scheduler
↓
CPU
~~~

คำถาม:

- ใครรัน
- บน CPU ไหน
- นานเท่าไร
- เมื่อไรถูก preempt
- fairness/latency/throughput trade-off คืออะไร

---

## 2. Scheduler vs Dispatcher

Scheduler:
เลือก task

Dispatcher:
ทำให้ task ที่เลือกได้ execution context บน CPU

---

## 3. Preemptive vs Non-Preemptive

Non-preemptive:
current task รันต่อจน burst/model event จบ

Preemptive:
scheduler สามารถหยุด current task ตาม policy

---

## 4. Simulator Assumptions

สูตรในบทนี้อิง model:

~~~text
single CPU
integer time units
one CPU burst per process
no I/O bursts
context-switch cost = 0
lower numeric priority = higher priority
deterministic tie breakers
~~~

ถ้า model เปลี่ยน สูตรและ trace interpretation ต้องปรับให้ตรง

---

## 5. Metrics

AT = Arrival Time  
BT = Burst Time  
CT = Completion Time  
TAT = Turnaround Time  
WT = Waiting Time  
RT = Response Time

ใน one-CPU-burst model:

~~~text
TAT = CT - AT
WT  = TAT - BT
RT  = first_start - AT
~~~

WT ไม่เท่ากับ first_start-arrival ใน preemptive workload ทั่วไป

---

## 6. FCFS

First-Come, First-Served

เลือก arrival order

ข้อเสีย:

~~~text
long job first
→ short jobs wait
→ convoy effect
~~~

---

## 7. SJF

Shortest Job First

เลือก burst สั้นสุดจาก jobs ที่ arrive แล้ว

classic property:
ถ้ารู้ burst lengths และใช้ assumptions ของ model จะช่วย average waiting time

ปัญหา:
future CPU burst ไม่ได้รู้สมบูรณ์ในระบบจริง

---

## 8. SRTF

Shortest Remaining Time First

preemptive version ของ shortest-job idea

decision ใช้ remaining time ไม่ใช่ original burst

long task อาจ starvation ถ้ามี short tasks เข้ามาตลอด

---

## 9. Priority Scheduling

คอร์ส simulator:

~~~text
smaller priority number
=
higher priority
~~~

นี่เป็น convention ของ simulator ไม่ใช่ universal rule

strict priority สามารถ starvation

aging:
รอนานขึ้น → effective priority ดีขึ้น

---

## 10. Round Robin

ready queue แบบ FIFO rotation

task ได้ quantum

~~~text
run min(quantum, remaining)
↓
new arrivals join queue
↓
unfinished task returns to tail
~~~

quantum เล็ก:
- responsive
- dispatch/context-switch pressure สูงขึ้น

quantum ใหญ่:
- behavior เข้าใกล้ FCFS

---

## 11. MLFQ — Explicit Policy ของ Simulator

queues:

~~~text
Q0 quantum=2
Q1 quantum=4
Q2 quantum=8
~~~

rules:

1. new task เข้า Q0
2. scheduler เลือก highest non-empty queue
3. task รันต่อจน:
   - finish
   - quantum หมด
   - higher-priority queue มีงาน
4. ใช้ quantum หมด → demote
5. higher queue arrival → preempt lower queue
6. ทุก 20 time units → boost runnable tasks ไป Q0

### Correctness detail

task ไม่ถูก requeue ทุก 1 tick ถ้ายังมี quantum เหลือและไม่มี higher-priority work

self-test ตรวจ specifically:

~~~text
P1 ที่ t=0 ใน Q0
ต้องได้ 0–2
ไม่ใช่ 0–1 แล้วสลับทันที
~~~

---

## 12. Linux Scheduler Reality

textbook algorithms มีไว้เรียน policy trade-offs

Linux normal scheduling ไม่ได้เลือก SJF/RR แบบ simulator นี้ตรง ๆ

Linux มี scheduling classes/policies เช่น:

- normal/fair scheduling
- SCHED_BATCH
- SCHED_IDLE
- SCHED_FIFO
- SCHED_RR
- SCHED_DEADLINE

modern Linux fair scheduler ใช้ EEVDF concepts เช่น:

- virtual runtime
- lag
- eligibility
- virtual deadline

CFS เป็นพื้นฐานทางประวัติศาสตร์ที่สำคัญ แต่ไม่ควรสรุปว่า modern Linux = CFS อย่างเดียว

---

## 13. Fedora Observation

~~~bash
uname -r
chrt -m
ps -eo pid,cls,rtprio,pri,ni,psr,stat,comm | head -30
taskset -pc $$
~~~

nice:
เป็น input ต่อ fair-scheduling weight/priority behavior ไม่ใช่ลำดับรันตายตัว

affinity:
กำหนด allowed CPUs ไม่ใช่ scheduling priority

---

## Lab

~~~bash
dotnet run --project 09-scheduling/examples/Chapter09.csproj -- fcfs
dotnet run --project 09-scheduling/examples/Chapter09.csproj -- sjf
dotnet run --project 09-scheduling/examples/Chapter09.csproj -- srtf
dotnet run --project 09-scheduling/examples/Chapter09.csproj -- priority
dotnet run --project 09-scheduling/examples/Chapter09.csproj -- priority-preemptive
dotnet run --project 09-scheduling/examples/Chapter09.csproj -- rr 2
dotnet run --project 09-scheduling/examples/Chapter09.csproj -- mlfq
dotnet run --project 09-scheduling/examples/Chapter09.csproj -- self-test
~~~

---

## แบบฝึกหัด

1. CT/TAT/WT/RT definitions
2. คำนวณ FCFS
3. convoy effect
4. SJF เลือก job ที่ยังไม่ arrive ได้ไหม
5. SRTF trace
6. Priority preemption
7. starvation case
8. aging design
9. RR q=1
10. RR q=4
11. RR q ใหญ่มาก
12. compare response time
13. trace MLFQ Q0/Q1/Q2
14. higher-priority arrival preempts อย่างไร
15. boost ช่วยอะไร
16. แก้ simulator เพิ่ม context-switch cost
17. แก้ simulator เพิ่ม I/O burst model
18. nice vs priority
19. affinity vs priority
20. textbook MLFQ vs Linux EEVDF

---

## Explain-It-Back

สำหรับทุก algorithm ตอบ:

~~~text
decision rule
preemptive?
best characteristic
main weakness
starvation risk
metrics affected
~~~
