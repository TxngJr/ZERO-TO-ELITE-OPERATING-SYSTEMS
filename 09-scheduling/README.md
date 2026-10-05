# Chapter 09 — CPU Scheduling

## 1. Goals

หลังบทนี้ต้องสามารถ:

- อธิบาย scheduler, dispatcher, ready queue, preemption และ time quantum
- คำนวณ Arrival, Burst, Completion, Turnaround, Waiting และ Response Time
- วาด Gantt Chart
- วิเคราะห์ FCFS
- วิเคราะห์ SJF และ SRTF
- วิเคราะห์ Priority Scheduling ทั้ง preemptive/non-preemptive
- วิเคราะห์ Round Robin
- เข้าใจ MLFQ
- อธิบาย starvation และ aging/priority boost
- ใช้ simulator เพื่อ verify การคำนวณ
- แยก textbook scheduler จาก Linux scheduler จริง
- สังเกต scheduling class, nice value และ CPU assignment บน Fedora

---

# Part I — Why Scheduling Exists

## 2. Scheduling Problem

สมมติระบบมี runnable tasks จำนวนมาก แต่ logical CPU แต่ละตัว execute instruction stream ได้จำกัด

~~~text
Ready tasks
P1 P2 P3 P4 P5
      |
      v
  Scheduler
      |
      v
    CPU
~~~

scheduler ต้องตัดสินใจ:

~~~text
Who runs?
On which CPU?
For how long?
When should it be preempted?
~~~

บน multicore system ปัญหายังรวม:

- CPU placement
- load balancing
- affinity
- cache locality
- NUMA ในระบบที่เกี่ยวข้อง

บทนี้เริ่มจาก single-CPU textbook model เพื่อเรียน algorithm ก่อน

---

## 3. Scheduler vs Dispatcher

### Scheduler

เลือก runnable task ที่ควรได้ CPU

### Dispatcher

กลไกที่ทำให้ task ที่เลือกเริ่ม/กลับมารันจริง เช่น context-switch path

concept:

~~~text
Ready Queue
    |
 scheduler selects
    v
Chosen Task
    |
 dispatcher/context switch
    v
CPU execution
~~~

---

## 4. Preemptive vs Non-Preemptive

### Non-Preemptive

เมื่อ task ได้ CPU จะรันจน:

- burst จบ
- block
- exit
- voluntarily yield ตาม model

### Preemptive

OS สามารถหยุด task เพื่อให้ task อื่นรัน

จำเป็นต่อ responsiveness และ priority policies จำนวนมาก

---

# Part II — Scheduling Metrics

## 5. Terms

สำหรับ process P:

- Arrival Time (AT) — เวลาที่เข้าระบบ/ready model
- Burst Time (BT) — CPU time ที่ workload ต้องใช้ในโจทย์
- First Start Time (ST) — ครั้งแรกที่ได้ CPU
- Completion Time (CT) — เวลาที่จบ
- Turnaround Time (TAT)
- Waiting Time (WT)
- Response Time (RT)

สูตร:

~~~text
TAT = CT - AT

WT = TAT - BT

RT = first_start - AT
~~~

ใน model ที่มี CPU burst เดียวและไม่มี I/O burst แทรก สูตร WT นี้ใช้ตรง ๆ

---

## 6. Other Metrics

### Throughput

~~~text
completed jobs / unit time
~~~

### CPU Utilization

~~~text
busy CPU time / total observed time
~~~

### Fairness

ไม่ได้มีสูตรเดียว universal

ต้องถามว่า algorithm แจก service ตาม policy ที่ต้องการหรือไม่

---

# Part III — FCFS

## 7. First-Come, First-Served

เลือกตาม arrival order

ตัวอย่าง:

| Process | AT | BT |
|---|---:|---:|
| P1 | 0 | 5 |
| P2 | 1 | 2 |
| P3 | 2 | 1 |

Gantt:

~~~text
0         5     7   8
|   P1    | P2  |P3 |
~~~

completion:

~~~text
P1 CT=5
P2 CT=7
P3 CT=8
~~~

waiting:

~~~text
P1 WT=0
P2 WT=7-1-2=4
P3 WT=8-2-1=5
~~~

---

## 8. Convoy Effect

ถ้า long CPU-bound job มาก่อน short jobs:

~~~text
LLLLLLLLLLLL S S S S
~~~

short jobs รอนาน

นี่คือข้อเสียสำคัญของ FCFS

---

# Part IV — SJF / SRTF

## 9. Shortest Job First

non-preemptive:

เมื่อ CPU ว่าง เลือก arrived job ที่ burst สั้นที่สุด

ข้อดีทางทฤษฎี:

ถ้ารู้ burst lengths ล่วงหน้า SJF ลด average waiting time ใน model classic ได้ดี

ปัญหา:

OS ไม่รู้ future CPU burst แบบสมบูรณ์ในชีวิตจริง

---

## 10. Shortest Remaining Time First

SRTF = preemptive form ของ SJF

ทุก scheduling decision เลือก task ที่ remaining time สั้นสุด

ตัวอย่าง:

~~~text
t=0 P1 burst=8 starts
t=1 P2 burst=2 arrives
P2 remaining 2 < P1 remaining 7
→ preempt P1
~~~

SRTF ช่วย short jobs แต่ long jobs อาจ starvation ถ้ามี short jobs มาเรื่อย ๆ

---

# Part V — Priority Scheduling

## 11. Priority

กำหนด priority ให้ task

ใน simulator นี้:

~~~text
smaller number = higher priority
~~~

นี่เป็น convention ของ simulator ไม่ใช่กฎ universal ของทุก OS/API

---

## 12. Non-Preemptive Priority

เลือก highest-priority available job เมื่อ CPU ว่าง

job ปัจจุบันไม่ถูกแยกกลาง burst

## 13. Preemptive Priority

ถ้า higher-priority job มา สามารถ preempt current job

---

## 14. Starvation and Aging

strict priority:

~~~text
high priority jobs keep arriving
↓
low priority job waits indefinitely
~~~

Aging:

~~~text
wait longer
↓
effective priority improves
~~~

หรือ MLFQ อาจใช้ periodic priority boost

---

# Part VI — Round Robin

## 15. Round Robin

ready queue แบบ circular FIFO

แต่ละ task ได้ quantum q

~~~text
P1 q
P2 q
P3 q
P1 q
...
~~~

ถ้า task จบก่อน quantum หมด CPU ไป task ถัดไป

---

## 16. Quantum Trade-Off

### q เล็กมาก

ข้อดี:

- response ดีขึ้นใน workload interactive

ข้อเสีย:

- context-switch overhead สูงขึ้น

### q ใหญ่มาก

Round Robin เข้าใกล้ FCFS

ดังนั้น quantum เป็น trade-off ไม่ใช่ "ยิ่งเล็กยิ่งดี"

---

## 17. Example RR

processes arrival 0 ทั้งหมด:

~~~text
P1 BT=5
P2 BT=3
P3 BT=1
q=2
~~~

Gantt:

~~~text
0  2  4 5  7 8 9
|P1|P2|P3|P1|P2|P1|
~~~

ให้คำนวณ CT/TAT/WT/RT ด้วยตนเองแล้ว verify ด้วย simulator

---

# Part VII — MLFQ

## 18. Multi-Level Feedback Queue

MLFQ ใช้หลาย priority queues

ตัวอย่าง:

~~~text
Q0 quantum=2  highest
Q1 quantum=4
Q2 quantum=8  lowest
~~~

typical teaching rules:

1. new job เข้า Q0
2. scheduler เลือก queue สูงสุดที่ไม่ว่าง
3. ถ้าใช้ quantum หมดโดยยังไม่จบ → demote
4. higher-priority arrival สามารถ preempt lower queue
5. periodic priority boost ป้องกัน starvation

---

## 19. Why MLFQ Is Interesting

เราอยากได้ behavior คล้าย:

- short/interactive jobs → response เร็ว
- long CPU-bound jobs → ยัง progress
- ไม่ต้องรู้ burst length ล่วงหน้า

MLFQ พยายาม infer behavior จาก execution history

---

## 20. MLFQ Is a Family, Not One Exact Algorithm

ตำราแต่ละเล่มอาจกำหนด:

- จำนวน queues
- quantum
- demotion rule
- allotment
- boost period
- I/O behavior

ต่างกัน

ดังนั้นเวลาแก้โจทย์ต้องอ่าน rules ที่โจทย์กำหนด

simulator ใน repo ใช้ explicit simplified rules และไม่อ้างว่าเป็น Linux scheduler

---

# Part VIII — Simulator

## 21. Run

~~~bash
cd 09-scheduling
python3 scheduler_sim.py --algo fcfs
python3 scheduler_sim.py --algo sjf
python3 scheduler_sim.py --algo srtf
python3 scheduler_sim.py --algo priority
python3 scheduler_sim.py --algo priority-preemptive
python3 scheduler_sim.py --algo rr --quantum 2
python3 scheduler_sim.py --algo mlfq
~~~

default workload:

~~~text
P1 AT=0 BT=8 Priority=2
P2 AT=1 BT=4 Priority=1
P3 AT=2 BT=2 Priority=3
P4 AT=3 BT=1 Priority=2
~~~

---

## 22. Custom Workload

สร้าง JSON:

~~~json
[
  {"pid": "A", "arrival": 0, "burst": 5, "priority": 2},
  {"pid": "B", "arrival": 1, "burst": 3, "priority": 1},
  {"pid": "C", "arrival": 4, "burst": 2, "priority": 3}
]
~~~

run:

~~~bash
python3 scheduler_sim.py --algo rr --quantum 2 --input workload.json
~~~

---

## 23. Simulator Assumptions

เพื่อให้เรียนง่าย:

- single CPU
- integer time units
- one CPU burst per job
- no I/O bursts
- context-switch time = 0
- lower priority number = higher priority
- tie breakers deterministic
- MLFQ is a teaching implementation

ดังนั้นห้ามเอาค่าจาก simulator ไปอ้างว่าเป็น exact Linux behavior

---

# Part IX — Linux Scheduling

## 24. Textbook Algorithms vs Linux

Linux ไม่ได้เลือกระหว่าง:

~~~text
FCFS vs SJF vs RR
~~~

สำหรับ process ทุกตัวแบบโจทย์มหาวิทยาลัยเดียวกัน

Linux มี scheduling classes/policies และ multicore scheduling machinery

สิ่งที่เรียนในบทนี้เป็น foundations ที่ช่วยเข้าใจ trade-offs ของ scheduler จริง

---

## 25. Modern Linux Fair Scheduling

ในอดีต Linux fair scheduling อธิบายผ่าน CFS (Completely Fair Scheduler) และ virtual runtime เป็นหลัก

kernel สมัยใหม่เริ่มเปลี่ยน fair scheduling ไปใช้แนวคิด **EEVDF — Earliest Eligible Virtual Deadline First** ตั้งแต่ Linux 6.6

EEVDF ใช้แนวคิดเช่น:

- virtual runtime
- lag
- eligibility
- virtual deadline

เพื่อเลือก fair-class tasks และปรับ latency/fairness

ดังนั้นคอร์สนี้ไม่พูดว่า:

~~~text
"Linux ปัจจุบันใช้ CFS อย่างเดียว"
~~~

ให้ตรวจ kernel version และ documentation ของ kernel ที่ใช้อยู่เสมอ

---

## 26. Scheduling Classes / Policies Preview

Linux มี policies เช่น:

- SCHED_OTHER / normal fair scheduling
- SCHED_BATCH
- SCHED_IDLE
- SCHED_FIFO
- SCHED_RR
- SCHED_DEADLINE

real-time/deadline policies มีผลต่อระบบสูงและอาจต้อง privilege

ในคอร์สนี้เน้น observation ไม่เปลี่ยนเครื่องหลักเป็น RT configuration

---

## 27. Fedora Observation Lab

ตรวจ kernel:

~~~bash
uname -r
~~~

ดู scheduling policy ที่เครื่องรองรับ:

~~~bash
chrt -m
~~~

ดู processes:

~~~bash
ps -eo pid,cls,rtprio,pri,ni,psr,stat,comm | head -30
~~~

fields:

- CLS = scheduling class
- RTPRIO = real-time priority ถ้ามี
- PRI = priority representation ของ tool
- NI = nice
- PSR = CPU ที่ report

---

## 28. Nice Value

normal users สามารถทดลองเพิ่ม nice value ของ process ตัวเอง:

~~~bash
nice -n 10 sleep 30 &
ps -o pid,ni,pri,cls,comm -p $!
wait $!
~~~

nice เป็น input ต่อ fair scheduling priority/weight ไม่ใช่ "task จะได้ CPU ลำดับ 10"

---

## 29. CPU Affinity Preview

ดู CPU count:

~~~bash
nproc
lscpu
~~~

ดู affinity:

~~~bash
taskset -pc $$
~~~

affinity จำกัด CPU set ที่ task มีสิทธิ์รัน

อย่าสับสน:

~~~text
affinity = allowed CPUs
priority = who should run
~~~

---

## 30. Multicore Complexity

scheduler จริงต้องตัดสินใจมากกว่า "เลือก task ถัดไป":

~~~text
Which task?
Which CPU?
Wakeup placement?
Migrate or preserve cache locality?
Balance load?
SMT/core topology?
Power/performance?
NUMA?
~~~

ดังนั้น textbook Gantt Chart เป็น foundation ไม่ใช่ complete scheduler implementation

---

# Part X — Calculation Strategy

## 31. วิธีแก้โจทย์ Scheduling

ทุกครั้ง:

### Step 1 — Table

~~~text
PID | AT | BT | Priority
~~~

### Step 2 — Algorithm Rules

เขียนก่อนว่า:

- preemptive?
- tie breaker?
- quantum?
- lower/higher number = higher priority?

### Step 3 — Timeline

วาด Gantt ทีละ scheduling event

### Step 4 — Completion

หา CT ของทุก process

### Step 5 — Metrics

~~~text
TAT = CT - AT
WT = TAT - BT
RT = first_start - AT
~~~

### Step 6 — Average

~~~text
Average WT = sum(WT) / n
Average TAT = sum(TAT) / n
Average RT = sum(RT) / n
~~~

### Step 7 — Verify

ใช้ simulator

---

## 32. Common Mistakes

### Waiting Time = Start - Arrival

ผิดสำหรับ preemptive algorithms

นั่นใกล้กับ Response Time เฉพาะ first start

### RR queue order ผิดเมื่อมี arrivals ระหว่าง quantum

ต้องกำหนด rule ชัดและ enqueue arrivals ตาม timeline

### SJF เลือก job ที่ยังไม่ arrive

ผิด

### SRTF ดู original burst แทน remaining

ผิด

### Priority convention ไม่อ่านโจทย์

ผิด บางโจทย์เลขมากสูงกว่า บางโจทย์เลขน้อยสูงกว่า

### MLFQ มีสูตรเดียว

ผิด ต้องอ่าน rules

---

## 33. Exercises

### Metrics

1. AT=2 BT=5 CT=10 หา TAT
2. จากข้อ 1 หา WT
3. first start=4 หา RT

### FCFS

4. P1(0,6), P2(1,2), P3(2,1): วาด Gantt
5. หา average WT

### SJF

6. P1(0,7), P2(0,2), P3(0,4): schedule
7. compare average WT กับ FCFS order P1,P2,P3

### SRTF

8. P1(0,8), P2(1,4), P3(2,2): trace ทุก arrival
9. หา response times

### Priority

10. ออกแบบ preemptive timeline 3 jobs
11. ยก starvation case
12. aging แก้อย่างไร

### RR

13. P1=5,P2=3,P3=1 q=2
14. ลอง q=1
15. ลอง q=10
16. compare response/context-switch count

### MLFQ

17. Q0=2,Q1=4,Q2=8 trace CPU-bound job burst=15
18. เพิ่ม short job arrival t=5
19. priority boost ช่วยอะไร
20. MLFQ ต่างจาก SJF อย่างไร

### Linux

21. chrt -m แสดงอะไร
22. NI คืออะไรระดับ concept
23. PSR แสดงอะไร
24. taskset affinity ต่างจาก priority อย่างไร
25. ทำไมไม่ควรเรียก Linux normal scheduler ว่า Round Robin

---

## 34. Quiz

1. TAT = CT - AT หรือไม่
2. WT = TAT - BT ใน one-burst model หรือไม่
3. RT ใช้ first start หรือ completion
4. FCFS preemptive หรือไม่ใน classic form
5. SRTF ใช้ remaining time หรือ original burst
6. RR ต้องมี quantum หรือไม่
7. q ใหญ่มากทำ RR เข้าใกล้ FCFS หรือไม่
8. strict priority starvation ได้หรือไม่
9. aging ช่วย starvation หรือไม่
10. MLFQ new job มักเริ่ม high queue ใน teaching model หรือไม่
11. Linux fair scheduling สมัยใหม่อธิบายด้วย EEVDF ได้หรือไม่
12. textbook simulator เท่ากับ Linux scheduler จริงหรือไม่

### Answers

1. ใช่
2. ใช่
3. first start
4. ไม่
5. remaining
6. ใช่
7. ใช่
8. ได้
9. ใช่
10. ใช่ใน model ที่กำหนด
11. ใช่สำหรับ modern fair scheduling transition
12. ไม่

---

## 35. Explain-It-Back

อธิบายโจทย์เดียวกันผ่าน:

~~~text
FCFS
SJF
SRTF
Priority
RR
MLFQ
~~~

สำหรับแต่ละ algorithm ให้ตอบ:

- decision rule
- preemptive?
- strength
- weakness
- starvation risk
- workload ที่ตอบโจทย์
- metric ที่มักดี/แย่

จากนั้นอธิบายว่าทำไม Linux scheduler จริงซับซ้อนกว่า Gantt Chart
