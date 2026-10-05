# Chapter 11 — Virtual Memory with C#

## Goals

เข้าใจ Virtual Memory, Demand Paging, Page Fault, Minor/Major Fault, Replacement, Locality, Working Set, Thrashing, Swap, Memory Mapping และ Copy-on-Write

## Virtual Memory Is Not Swap

~~~text
Virtual Memory
=
address-space abstraction
+
translation
+
protection
+
mapping
+
demand population
+
sharing/private semantics
~~~

Swap เป็นเพียง backing/reclaim mechanism หนึ่งส่วน

## Demand Paging from C#

Lab ใช้ unmanaged memory:

~~~csharp
IntPtr memory = Marshal.AllocHGlobal(bytes);
~~~

แล้ว touch หนึ่ง byte ต่อ page:

~~~csharp
Marshal.WriteByte(memory, offset, 1);
~~~

พร้อมอ่าน Linux fault counters ผ่าน P/Invoke getrusage

run:

~~~bash
dotnet run --project 11-virtual-memory/examples/Chapter11.csproj -- faults
~~~

จำนวน fault ไม่จำเป็นต้องเท่าจำนวน pages แบบ 1:1 เพราะ runtime/kernel optimizations และ huge-page behavior

## Minor vs Major Fault

minor: resolve ได้โดยไม่ต้องอ่าน backing data จาก storage แบบ blocking I/O ใน accounting model

major: ต้องมี backing I/O มากขึ้น

คำว่า minor/major ไม่ใช่ระดับความร้ายแรงของ bug

## MemoryMappedFile

C# abstraction:

~~~csharp
MemoryMappedFile
MemoryMappedViewAccessor
~~~

run:

~~~bash
dotnet run --project 11-virtual-memory/examples/Chapter11.csproj -- mmap
~~~

## Copy-on-Write

C# demo ใช้:

~~~csharp
MemoryMappedFileAccess.CopyOnWrite
~~~

เพื่อเห็น private-write semantics:

~~~bash
dotnet run --project 11-virtual-memory/examples/Chapter11.csproj -- cow
~~~

mapped view เปลี่ยนได้ แต่ file backing เดิมไม่ถูกเขียนตาม

นี่ช่วยสร้าง mental model สำหรับ fork COW แม้ Process.Start ใน C# ไม่ได้ expose fork address-space duplication

## Page Replacement in C#

~~~bash
dotnet run --project 11-virtual-memory/examples/Chapter11.csproj -- replacement
~~~

รองรับ:

- FIFO
- LRU
- OPT
- CLOCK
- Belady anomaly demo

## Locality

Temporal locality = สิ่งที่เพิ่งใช้มีแนวโน้มใช้ซ้ำ

Spatial locality = address ใกล้กันมีแนวโน้มถูกใช้ใกล้เวลาเดียวกัน

## Working Set and Thrashing

ถ้า active working sets ใหญ่เกิน effective memory:

~~~text
reclaim
→ faults
→ reload
→ reclaim
→ useful work ลด
~~~

ไม่สร้าง intentional thrashing บน laptop หลัก

## Linux Observation

~~~bash
free -h
vmstat 1 5
swapon --show
zramctl
cat /proc/$$/maps
~~~

## C# Exercises

1. เปลี่ยน region จาก 32 MiB เป็น 64 MiB แล้ววัด fault counters
2. touch ทุก 2 pages แล้วเปรียบเทียบ
3. สร้าง MemoryMappedFile ReadWrite และ CopyOnWrite เปรียบเทียบ
4. เขียน FIFO/LRU/OPT/CLOCK ด้วย class แยก
5. เขียน function รับ reference string จาก args
6. แสดง Belady anomaly ด้วย C#
7. สร้าง simulator working set แบบ sliding window
8. อธิบาย TLB miss, cache miss และ page fault ว่าไม่ใช่ event เดียวกัน

## Quiz

- Virtual Memory = Swap หรือไม่?
- every page fault = error หรือไม่?
- every page fault = disk I/O หรือไม่?
- FIFO มี Belady anomaly ได้หรือไม่?
- CopyOnWrite แก้ backing file เดิมหรือไม่?
