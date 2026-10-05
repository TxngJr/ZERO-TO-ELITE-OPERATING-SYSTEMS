# Lab 09 — Scheduling in C#

## Default Workload

~~~text
P1 AT=0 BT=8 PR=2
P2 AT=1 BT=4 PR=1
P3 AT=2 BT=2 PR=3
P4 AT=3 BT=1 PR=2
~~~

ทำด้วยมือก่อน แล้ว run simulator

~~~bash
dotnet run --project 09-scheduling/examples/Chapter09.csproj -- fcfs
dotnet run --project 09-scheduling/examples/Chapter09.csproj -- srtf
dotnet run --project 09-scheduling/examples/Chapter09.csproj -- rr 2
dotnet run --project 09-scheduling/examples/Chapter09.csproj -- mlfq
~~~

## C# Assignment

สร้าง class Scheduler ที่มี methods:

- Fcfs
- Sjf
- Srtf
- Priority
- RoundRobin
- Mlfq

คืน Timeline และ Metrics โดยห้ามใช้ Python
