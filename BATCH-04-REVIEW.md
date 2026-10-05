# Batch 04 Review — Chapters 10–11

## Must Master

- VA / PA
- Page / Frame
- VPN / PFN / Offset
- MMU / TLB / Page Table
- PTE permissions
- multi-level paging
- 4-level / 5-level concept
- Demand Paging
- Minor / Major Fault
- mmap / munmap
- mprotect
- SIGSEGV path
- FIFO / LRU / OPT / CLOCK
- Belady
- Working Set / Thrashing
- file mapping
- COW

## Critical Distinctions

~~~text
host page size
≠
simulator page size

TLB miss
≠
page fault

cache miss
≠
page fault

file-backed COW
≠
fork COW demo
~~~

## C# Gate

~~~bash
dotnet run --project 10-address-translation/examples/Chapter10.csproj -- self-test
dotnet run --project 10-address-translation/examples/Chapter10.csproj -- tlb

dotnet run --project 11-virtual-memory/examples/Chapter11.csproj -- faults
dotnet run --project 11-virtual-memory/examples/Chapter11.csproj -- protection
dotnet run --project 11-virtual-memory/examples/Chapter11.csproj -- self-test
~~~

## Explain

1. Environment.SystemPageSize คือ host page size
2. Chapter 10 simulator ใช้ explicit 4096-byte teaching page
3. mmap first-touch เกี่ยวกับ demand paging อย่างไร
4. mprotect write failure เชื่อม MMU → kernel → signal อย่างไร
5. Belady anomaly
