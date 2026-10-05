# Scheduling Calculation Set

## Set A — FCFS

| PID | AT | BT |
|---|---:|---:|
| P1 | 0 | 7 |
| P2 | 2 | 4 |
| P3 | 4 | 1 |
| P4 | 5 | 4 |

หา:

- Gantt Chart
- CT
- TAT
- WT
- RT
- average WT/TAT/RT

---

## Set B — SJF vs SRTF

| PID | AT | BT |
|---|---:|---:|
| A | 0 | 8 |
| B | 1 | 4 |
| C | 2 | 2 |
| D | 3 | 1 |

ทำทั้ง:

- SJF
- SRTF

อธิบายทุกจุดที่ SRTF preempt

---

## Set C — Priority

lower number = higher priority

| PID | AT | BT | PR |
|---|---:|---:|---:|
| P1 | 0 | 6 | 3 |
| P2 | 1 | 3 | 1 |
| P3 | 2 | 4 | 2 |
| P4 | 4 | 2 | 1 |

ทำ:

- non-preemptive
- preemptive

จากนั้นสร้าง workload เพิ่มเองให้ low-priority task รอนานมาก

---

## Set D — Round Robin

all arrive at 0

~~~text
P1 BT=9
P2 BT=5
P3 BT=3
P4 BT=1
~~~

คำนวณ:

- q=1
- q=3
- q=20

เปรียบเทียบ RT และจำนวน dispatch segments

---

## Set E — MLFQ

rules:

~~~text
Q0 q=2
Q1 q=4
Q2 q=8
new job -> Q0
quantum exhausted -> demote
higher queue preempts lower
boost all ready jobs every 20 time units
~~~

workload:

~~~text
CPU_A AT=0 BT=18
SHORT_B AT=3 BT=1
SHORT_C AT=7 BT=2
CPU_D AT=9 BT=12
~~~

trace timeline ด้วยมือก่อนใช้ simulator
