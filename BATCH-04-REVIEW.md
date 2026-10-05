# Batch 04 Review — C# Edition

## Chapters 10–11

ต้อง master:

- VA / PA
- Page / Frame
- VPN / Offset
- MMU / TLB / Page Table
- Demand Paging
- Minor / Major Fault
- FIFO / LRU / OPT / CLOCK
- Belady
- MemoryMappedFile
- CopyOnWrite
- Working Set / Thrashing

## C# Gate

~~~bash
dotnet run --project 10-address-translation/examples/Chapter10.csproj -- table
dotnet run --project 11-virtual-memory/examples/Chapter11.csproj -- faults
dotnet run --project 11-virtual-memory/examples/Chapter11.csproj -- replacement
~~~

ต้องอธิบายว่าทำไม managed object address ไม่ควรถูกใช้เป็น physical-address mental model
