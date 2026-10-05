# Final Review — C# Edition

## Explain Without Notes

1. OS / Kernel
2. C#/.NET runtime layer
3. System Call
4. Process / Thread
5. Context Switch
6. Process.Start / WaitForExit
7. Thread.Start / Join
8. Race Condition
9. lock / Monitor
10. Volatile / Interlocked
11. SemaphoreSlim
12. Producer–Consumer
13. ReaderWriterLockSlim
14. Deadlock / Starvation / Livelock
15. Scheduling algorithms
16. VA / PA / MMU / TLB
17. Page Fault
18. Page Replacement
19. MemoryMappedFile
20. CopyOnWrite

## Programming Final

เขียน C# program ที่:

- สร้าง producer 2 threads
- consumer 3 threads
- bounded buffer
- lock + Monitor.Wait/PulseAll
- termination condition
- Start/Join ครบ

จากนั้นอธิบาย Linux thread/process behavior ที่เกี่ยวข้อง

## Calculation Final

- Scheduling CT/TAT/WT/RT
- VPN/offset
- page replacement faults

## Observation Final

ใช้:

~~~bash
strace
ps -L
/proc/PID/maps
pmap
vmstat
~~~
