# Chapter 10 — Address Translation

## เป้าหมาย

หลังบทนี้ต้องเข้าใจ:

- Virtual Address vs Physical Address
- Page vs Frame
- VPN / PFN / Offset
- MMU
- Page Table / PTE
- permissions
- multi-level page tables
- 4-level / 5-level concept
- TLB hit/miss
- TLB replacement simulation
- context-switch translation effects
- huge-page trade-offs

---

## 1. Virtual Address

program instruction ใช้ virtual address ภายใต้ process address space

same VA ในสอง process:

~~~text
ไม่จำเป็นต้อง map physical frame เดียวกัน
~~~

และ shared mappings สามารถตั้งใจ map backing เดียวกันได้

---

## 2. Physical Address

หลัง translation hardware memory system ใช้ physical-address information

user process ปกติไม่ต้องรู้ PFN จริงเพื่อใช้ memory

---

## 3. Page / Frame

virtual space แบ่งเป็น pages

physical memory model แบ่งเป็น frames

~~~text
VPN → PFN
~~~

offset ภายใน page ไม่เปลี่ยนระหว่าง translation

---

## 4. Host Page Size vs Simulator Page Size

คอร์สแยกสอง concept ชัดเจน

### Host

~~~csharp
Environment.SystemPageSize
~~~

คือ page size ที่ OS runtime รายงาน

### Simulator

~~~text
SimulatedPageSize = 4096
~~~

ใช้สำหรับโจทย์ page-table table ที่กำหนด VPN/PFN ไว้

ห้ามใช้ host page size กับ hardcoded 4 KiB test addresses โดยไม่ประกาศ assumption

---

## 5. Address Split

ทั่วไป:

~~~text
VPN = VA / page_size
offset = VA mod page_size
~~~

ถ้า page_size = 4096 = 2^12:

~~~text
VPN = VA >> 12
offset = VA & 0xFFF
~~~

---

## 6. MMU

simplified:

~~~text
CPU creates VA
↓
TLB lookup
↓ miss
page-table walk
↓
permission/PTE check
↓
PFN + offset
↓
physical access path
~~~

OS สร้าง/configure mapping

hardware MMU ใช้ mapping ระหว่าง normal memory access

---

## 7. PTE

teaching PTE มี:

- PFN
- readable
- writable
- executable

real architecture มี metadata เพิ่ม เช่น:

- present/valid
- user/supervisor
- accessed
- dirty
- execute-disable

exact bit encoding เป็น architecture-specific

---

## 8. Protection

page table ทำสองหน้าที่สำคัญ:

~~~text
translation
+
protection
~~~

ตัวอย่าง concept:

~~~text
code: read + execute
data: read + write
~~~

policy จริงขึ้นกับ mapping/runtime/kernel

---

## 9. Multi-Level Page Tables

flat page table สำหรับ huge sparse virtual space เปลือง memory

multi-level structure allocate lower levels ตามที่ต้องใช้

concept:

~~~text
VA indices
→ top level
→ next level
→ ...
→ PTE
→ frame
~~~

---

## 10. x86-64 4-Level / 5-Level

modern x86-64 Linux สามารถรองรับ 4-level หรือ 5-level paging ตาม hardware/kernel configuration

ดังนั้นไม่ควรจำ:

~~~text
x86-64 = exactly 4 levels forever
~~~

5-level เพิ่ม virtual/physical addressing capacity

---

## 11. Canonical Address Concept

64-bit pointer width ไม่ได้หมายความว่า hardware ใช้ arbitrary 64 address bits ทุก bit

x86-64 valid virtual addresses ต้องเป็น canonical ตาม active paging mode

รายละเอียด bit layout อยู่ advanced architecture track

---

## 12. TLB

TLB = translation cache

hit:
translation พร้อมใช้

miss:
ต้องหา translation ผ่าน page-table walk path

สำคัญ:

~~~text
TLB miss
≠
page fault
~~~

mapping สามารถ valid/present แต่ไม่อยู่ใน TLB

---

## 13. LRU TLB Simulator

Chapter 10 มี Tlb class:

- capacity
- Lookup
- Insert
- LRU eviction
- hit/miss counters

ใช้เพื่อเข้าใจ translation caching ไม่ได้ claim ว่า CPU TLB จริงใช้ exact implementation นี้

---

## 14. Context Switch

เปลี่ยน address space อาจกระทบ translation context

แต่ modern hardware/OS มี tagging mechanisms เช่น PCID-like concepts

ดังนั้น:

~~~text
context switch
≠
full TLB flush always
~~~

---

## 15. Huge Pages

larger pages:

ข้อดี:
- TLB reach มากขึ้น
- page-table entries น้อยลง

ข้อเสีย:
- fragmentation
- allocation constraints
- mapping granularity ใหญ่ขึ้น

---

## Lab

~~~bash
dotnet run --project 10-address-translation/examples/Chapter10.csproj
dotnet run --project 10-address-translation/examples/Chapter10.csproj -- split 0x12345678
dotnet run --project 10-address-translation/examples/Chapter10.csproj -- table
dotnet run --project 10-address-translation/examples/Chapter10.csproj -- tlb
dotnet run --project 10-address-translation/examples/Chapter10.csproj -- self-test
~~~

Linux:

~~~bash
getconf PAGESIZE
cat /proc/PID/maps
pmap -x PID
~~~

---

## แบบฝึกหัด

1. VA vs PA
2. page vs frame
3. VPN vs PFN
4. 4 KiB offset bits
5. split 0x12345
6. translate VPN→PFN
7. write protection
8. execute protection
9. PTE มีหน้าที่อะไร
10. multi-level ทำไมช่วย sparse space
11. TLB hit
12. TLB miss
13. TLB miss vs page fault
14. LRU TLB trace
15. TLB capacity 2 sequence A B A C B
16. context switch/TLB
17. 4-level vs 5-level
18. canonical address concept
19. huge pages trade-off
20. แยก host page size จาก simulated page size

---

## Explain-It-Back

~~~text
VA
→ VPN + offset
→ TLB
→ possible page-table walk
→ PTE permission
→ PFN
→ PA
~~~
