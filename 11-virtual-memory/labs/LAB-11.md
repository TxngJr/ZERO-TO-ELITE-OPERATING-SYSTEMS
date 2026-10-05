# Lab 11 — Virtual Memory in C#

~~~bash
dotnet run --project 11-virtual-memory/examples/Chapter11.csproj -- faults
dotnet run --project 11-virtual-memory/examples/Chapter11.csproj -- mmap
dotnet run --project 11-virtual-memory/examples/Chapter11.csproj -- cow
dotnet run --project 11-virtual-memory/examples/Chapter11.csproj -- replacement
~~~

## Observe Linux

~~~bash
free -h
vmstat 1 5
swapon --show
zramctl
~~~

## C# Assignment

สร้าง VirtualMemoryLab class ที่มี methods:

- ObserveFaults
- FileMapping
- CopyOnWriteMapping
- Fifo
- Lru
- Optimal
- Clock

ทุก implementation ใช้ C#
