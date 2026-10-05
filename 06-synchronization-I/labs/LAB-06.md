# Lab 06 — C# Synchronization

~~~bash
dotnet run --project 06-synchronization-I/examples/Chapter06.csproj -- lock-counter
dotnet run --project 06-synchronization-I/examples/Chapter06.csproj -- interlocked
dotnet run --project 06-synchronization-I/examples/Chapter06.csproj -- semaphore
dotnet run --project 06-synchronization-I/examples/Chapter06.csproj -- cas
~~~

## Exercise Matrix

| Problem | C# primitive |
|---|---|
| complex shared state | lock / Monitor |
| resource capacity N | SemaphoreSlim |
| exact counter | Interlocked |
| state 1 → 0 winner | CompareExchange |

เขียนเหตุผล ไม่ตอบแค่ชื่อ primitive
