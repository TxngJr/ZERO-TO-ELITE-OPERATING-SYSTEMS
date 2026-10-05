# Batch 03 Review — C# Edition

## Chapters 07–09

ต้อง master:

- Producer–Consumer ด้วย lock + Monitor
- Readers–Writers ด้วย ReaderWriterLockSlim
- Dining Philosophers + lock ordering
- Deadlock / Starvation / Livelock
- Wait-For Graph ใน C#
- FCFS / SJF / SRTF / Priority / RR / MLFQ
- CT / TAT / WT / RT

## C# Gate

run:

~~~bash
dotnet run --project 07-synchronization-II/examples/Chapter07.csproj -- producer-consumer
dotnet run --project 08-synchronization-III/examples/Chapter08.csproj -- graph
dotnet run --project 09-scheduling/examples/Chapter09.csproj -- rr 2
~~~

## Explain

ทำไม Monitor.Wait ต้องอยู่ใน while และทำไม PulseAll ไม่รับประกันว่า predicate จะยัง true เมื่อ thread ได้ lock กลับมา
