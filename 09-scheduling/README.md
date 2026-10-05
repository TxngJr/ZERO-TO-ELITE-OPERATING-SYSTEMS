# Chapter 09 — Scheduling with a C# Simulator

## Goals

เข้าใจ FCFS, SJF, SRTF, Priority, Round Robin, MLFQ และ metrics

## Metrics

~~~text
TAT = CT - AT
WT  = TAT - BT
RT  = FirstStart - AT
~~~

## Run C# Simulator

~~~bash
dotnet run --project 09-scheduling/examples/Chapter09.csproj -- fcfs
dotnet run --project 09-scheduling/examples/Chapter09.csproj -- sjf
dotnet run --project 09-scheduling/examples/Chapter09.csproj -- srtf
dotnet run --project 09-scheduling/examples/Chapter09.csproj -- priority
dotnet run --project 09-scheduling/examples/Chapter09.csproj -- priority-preemptive
dotnet run --project 09-scheduling/examples/Chapter09.csproj -- rr 2
dotnet run --project 09-scheduling/examples/Chapter09.csproj -- mlfq
~~~

## FCFS

เลือกตาม arrival order

ข้อเสียหลัก: convoy effect

## SJF

เลือก burst สั้นสุดจากงานที่ arrive แล้ว

## SRTF

เลือก remaining time สั้นสุดและ preempt ได้

## Priority

simulator นี้ใช้เลขน้อย = priority สูง

ต้องระวัง starvation

## Round Robin

ใช้ queue + quantum

quantum เล็ก:
- response ดีขึ้นได้
- context switching มากขึ้น

quantum ใหญ่:
- เข้าใกล้ FCFS

## MLFQ

C# simulator ใช้ 3 queues:

~~~text
Q0 q=2
Q1 q=4
Q2 q=8
~~~

เป็น teaching model ไม่ใช่ Linux scheduler implementation

## Linux Observation

~~~bash
uname -r
chrt -m
ps -eo pid,cls,rtprio,pri,ni,psr,stat,comm | head
taskset -pc $$
~~~

## C# Exercises

1. เปลี่ยน Workload array เป็นโจทย์ของตนเอง
2. เพิ่ม context-switch cost
3. เพิ่ม tie-breaker ที่กำหนดเอง
4. เพิ่ม aging ให้ priority scheduling
5. เพิ่ม priority boost ให้ MLFQ
6. เขียน method print Gantt Chart
7. เขียน unit-style self-check สำหรับ CT/TAT/WT/RT
8. เปรียบเทียบ RR quantum 1,2,4,20

## Quiz

- WT = FirstStart - Arrival เสมอหรือไม่?
- SRTF preemptive หรือไม่?
- RR ต้องมี quantum หรือไม่?
- strict priority starvation ได้หรือไม่?
- MLFQ มี implementation เดียวตายตัวหรือไม่?
