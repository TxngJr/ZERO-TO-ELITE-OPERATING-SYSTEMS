# Lab 09 — Scheduling Algorithms and Linux Observation

## Goal

แก้ scheduling ด้วยมือก่อน แล้วใช้ simulator verify

## A. Default Workload

~~~text
P1 AT=0 BT=8 PR=2
P2 AT=1 BT=4 PR=1
P3 AT=2 BT=2 PR=3
P4 AT=3 BT=1 PR=2
~~~

ก่อนรัน simulator ให้ทำ table และ Gantt ด้วยมือ

---

## B. FCFS

~~~bash
python3 scheduler_sim.py --algo fcfs
~~~

เปรียบเทียบ:

- timeline
- WT
- RT

---

## C. SJF

~~~bash
python3 scheduler_sim.py --algo sjf
~~~

ถาม:

- ตอน t=0 ทำไม P1 ยังถูกเลือก ทั้งที่มี jobs ที่ burst สั้นกว่าในอนาคต
- SJF เลือก future arrival ได้หรือไม่

---

## D. SRTF

~~~bash
python3 scheduler_sim.py --algo srtf
~~~

จดทุก preemption

อธิบายว่า remaining time เปลี่ยน decision อย่างไร

---

## E. Priority

~~~bash
python3 scheduler_sim.py --algo priority
python3 scheduler_sim.py --algo priority-preemptive
~~~

ใน simulator:

~~~text
smaller PR = higher priority
~~~

เปรียบเทียบ response time

---

## F. Round Robin

~~~bash
python3 scheduler_sim.py --algo rr --quantum 1
python3 scheduler_sim.py --algo rr --quantum 2
python3 scheduler_sim.py --algo rr --quantum 4
python3 scheduler_sim.py --algo rr --quantum 20
~~~

สร้างตาราง:

~~~text
q | avg WT | avg RT | timeline segments
~~~

อธิบาย trade-off

---

## G. MLFQ

~~~bash
python3 scheduler_sim.py --algo mlfq
python3 scheduler_sim.py --algo mlfq --boost 10
~~~

default queues:

~~~text
Q0 q=2
Q1 q=4
Q2 q=8
~~~

วิเคราะห์:

- P ใดถูก demote
- short job อยู่ queue ไหน
- boost เปลี่ยนอะไร

---

## H. Custom JSON

~~~bash
python3 scheduler_sim.py --algo rr --quantum 2 --input workload.json
~~~

แก้ workload.json แล้วสร้างกรณี:

1. convoy
2. SRTF preemption
3. priority starvation-like workload
4. CPU-bound MLFQ job

---

## I. Linux Observation

~~~bash
uname -r
chrt -m
ps -eo pid,cls,rtprio,pri,ni,psr,stat,comm | head -30
~~~

ทดลอง nice โดยไม่ใช้ root:

~~~bash
nice -n 10 sleep 20 &
PID=$!
ps -o pid,cls,pri,ni,psr,stat,comm -p "$PID"
wait "$PID"
~~~

---

## J. CPU Affinity

~~~bash
taskset -pc $$
~~~

ตอบ:

- allowed CPU list คืออะไร
- มันเท่ากับ priority หรือไม่
- scheduler ยังเลือก CPU ภายใน allowed set ได้หรือไม่

---

## K. Explain

เขียน 1 หน้า:

~~~text
ทำไม textbook scheduling simulator จึงจำเป็นต่อการเรียน
แต่ไม่ควรถูกเรียกว่า implementation ของ Linux scheduler จริง
~~~
