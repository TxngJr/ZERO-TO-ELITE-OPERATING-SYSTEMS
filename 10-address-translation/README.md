# Chapter 10 — Address Translation with C#

## Goals

เข้าใจ Virtual Address, Physical Address, MMU, Page, Frame, VPN, Offset, Page Table, PTE, Multi-Level Paging และ TLB

## C# Runtime Page Size

~~~csharp
int pageSize = Environment.SystemPageSize;
~~~

ไม่ hardcode 4096 แม้ x86-64 Linux มักใช้ 4 KiB base pages

## Virtual Address Split

~~~csharp
ulong vpn = virtualAddress / (ulong)pageSize;
ulong offset = virtualAddress % (ulong)pageSize;
~~~

ถ้า page size = 4096 = 2^12:

~~~text
VA
= VPN bits + 12-bit offset
~~~

## Unmanaged Address in C#

managed objects สามารถถูก GC ย้ายได้ จึงไม่ใช้ object address เป็น mental model หลัก

Lab ใช้:

~~~csharp
IntPtr memory = Marshal.AllocHGlobal(pageSize);
~~~

แล้วดู address ที่คืนมา

## MMU Mental Model

~~~text
CPU VA
↓
TLB
↓ miss
Page Table Walk
↓
PTE
↓
PFN + Offset
↓
Physical Address
~~~

C# ไม่ทำ translation เอง นี่เป็น hardware + OS responsibility

## TLB Miss vs Page Fault

- TLB miss: translation cache ไม่มี entry
- page fault: access ต้องให้ kernel จัดการ mapping/protection condition

สองอย่างไม่ใช่คำเดียวกัน

## C# Page Table Simulator

~~~bash
dotnet run --project 10-address-translation/examples/Chapter10.csproj -- table
~~~

## Split Address

~~~bash
dotnet run --project 10-address-translation/examples/Chapter10.csproj -- split 0x12345678
~~~

## Linux Observation

~~~bash
cat /proc/PID/maps
pmap -x PID
getconf PAGESIZE
~~~

## C# Exercises

1. เขียน method SplitAddress(ulong va)
2. สร้าง record Pte ที่มี PFN, Writable, Executable
3. เพิ่ม Read permission
4. เพิ่ม translation result enum: Success, Unmapped, ProtectionFault
5. ทำ TLB simulator ด้วย Dictionary + LRU list
6. คำนวณ hit/miss สำหรับ reference sequence
7. เปรียบเทียบ Environment.SystemPageSize กับ getconf PAGESIZE

## Quiz

- VA เท่ากับ PA หรือไม่?
- page กับ frame ต่างกันอย่างไร?
- TLB เป็น cache ของอะไร?
- TLB miss = page fault หรือไม่?
- managed object address ควรถูก assume ว่าคงที่หรือไม่?
