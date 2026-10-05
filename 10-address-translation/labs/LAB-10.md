# Lab 10 — Address Translation in C#

~~~bash
dotnet run --project 10-address-translation/examples/Chapter10.csproj
dotnet run --project 10-address-translation/examples/Chapter10.csproj -- split 0x12345678
dotnet run --project 10-address-translation/examples/Chapter10.csproj -- table
~~~

ระหว่าง default mode sleep:

~~~bash
cat /proc/PID/maps
pmap -x PID
~~~

## C# Assignment

สร้าง class Tlb ที่:

- capacity = 4
- Lookup(VPN)
- Insert(VPN, PFN)
- LRU replacement
- hit/miss counters
