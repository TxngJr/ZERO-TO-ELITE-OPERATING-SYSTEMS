# Batch 03 Review — Chapters 07–09

## Must Master

- Producer–Consumer
- Monitor.Wait/PulseAll
- Readers–Writers
- Dining Philosophers
- Deadlock / Starvation / Livelock
- Wait-For Graph
- Priority Inversion
- FCFS / SJF / SRTF / Priority / RR / MLFQ
- CT / TAT / WT / RT
- Linux scheduler vs textbook simulator

## Correctness Gates

### Monitor

Wait อยู่ใน while และ predicate ถูกตรวจซ้ำ

### Livelock

demo ต้อง bounded และทุก Barrier participant ผ่านจำนวน phases เท่ากัน

### MLFQ

Q0=2, Q1=4, Q2=8

task ไม่ถูก requeue ทุก 1 tick ถ้ายังไม่หมด quantum และไม่มี higher-priority work

## C# Gate

~~~bash
dotnet run --project 07-synchronization-II/examples/Chapter07.csproj -- producer-consumer
dotnet run --project 08-synchronization-III/examples/Chapter08.csproj -- self-test
dotnet run --project 08-synchronization-III/examples/Chapter08.csproj -- livelock
dotnet run --project 09-scheduling/examples/Chapter09.csproj -- self-test
~~~

## Explain

1. PulseAll ทำไมไม่ guarantee predicate
2. deadlock vs starvation vs livelock
3. MLFQ quantum/preemption
4. textbook RR/MLFQ ทำไมไม่ใช่ Linux normal fair scheduler
