# Lab 08 — Monitor, Deadlock Graph, Livelock

~~~bash
dotnet run --project 08-synchronization-III/examples/Chapter08.csproj -- monitor
dotnet run --project 08-synchronization-III/examples/Chapter08.csproj -- graph
dotnet run --project 08-synchronization-III/examples/Chapter08.csproj -- livelock
~~~

## Exercise

สร้าง class BoundedBuffer ใน C# ที่ encapsulate:

- Queue
- Capacity
- lock object
- Put
- Get
- Monitor.Wait
- PulseAll

caller ห้ามแตะ Queue โดยตรง
