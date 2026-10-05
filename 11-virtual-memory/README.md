# Chapter 11 — Virtual Memory

## เป้าหมาย

หลังบทนี้ต้องเข้าใจ:

- Virtual Memory ไม่เท่ากับ Swap
- Demand Paging
- Page Fault
- Minor / Major Fault
- Anonymous mmap
- Page Protection / mprotect
- SIGSEGV path
- FIFO / OPT / LRU / Clock
- Belady's Anomaly
- Locality
- Working Set / Thrashing
- Swap / zram
- File Mapping
- File-backed Copy-on-Write
- fork COW concept

---

## 1. Virtual Memory

Virtual Memory เป็น abstraction ใหญ่:

~~~text
per-process virtual address space
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
+
optional backing
~~~

Swap เป็นเพียงส่วนหนึ่งของ memory-management system

---

## 2. Demand Paging

mapping สามารถถูกสร้างก่อน physical page ถูก populate

~~~text
mmap virtual region
↓
first access
↓
page fault
↓
kernel resolves
↓
page table updated
↓
instruction retries
~~~

page fault จึงไม่ใช่ bug เสมอ

---

## 3. ทำไม Lab ใช้ mmap ผ่าน C# P/Invoke

AllocHGlobal เป็น allocator abstraction

เพื่อสอน VM โดยตรงกว่า Chapter 11 ใช้ C# source เรียก Linux:

- mmap
- munmap
- mprotect
- getrusage

นี่ทำให้เห็น:

~~~text
C# source
→ native interop
→ Linux VM primitive
~~~

โดยไม่เปลี่ยนภาษาหลักของคอร์ส

---

## 4. Anonymous mmap Lab

Lab map 32 MiB:

~~~text
PROT_READ | PROT_WRITE
MAP_PRIVATE | MAP_ANONYMOUS
~~~

วัด getrusage:

- before mmap
- after mmap
- after touching one byte per page

expected trend:

minor-fault count เพิ่มเมื่อ pages ถูก demand-populated

ไม่ควร expect exact 1 fault = 1 page เพราะ kernel/runtime/huge-page/accounting effects มีได้

---

## 5. Minor vs Major Fault

Minor:

fault ถูก resolve โดยไม่ต้องทำ blocking backing-store I/O ตาม accounting model

Major:

fault ต้องใช้ I/O เพื่อนำ page data เข้ามา

คำว่า minor/major ไม่ใช่ bug severity

---

## 6. Valid vs Invalid Fault

valid demand fault:

~~~text
mapping valid
→ kernel resolves
→ retry
~~~

invalid/protection fault:

~~~text
access violates mapping/permission
→ kernel cannot resolve as valid access
→ signal/error path
~~~

---

## 7. mprotect / Protection Demo

child process:

~~~text
mmap RW page
↓
write succeeds
↓
mprotect R
↓
attempt write
↓
hardware protection fault
↓
kernel signal path
↓
child cannot complete normally
~~~

parent process ยังรอดและตรวจ child exit

ใช้ child แยกเพื่อไม่ให้ main teaching process ตาย

---

## 8. SIGSEGV

Segmentation fault message ใน Linux user space ไม่ได้แปลว่าระบบใช้ old-style segmentation แทน paging

modern path สามารถเป็น:

~~~text
memory access
→ page/protection exception
→ kernel fault handler
→ invalid/unresolvable
→ SIGSEGV
~~~

---

## 9. Page Replacement

เมื่อ frames จำกัด system ต้องเลือก victim ตาม policy/model

### FIFO

evict oldest arrival

### OPT

evict page whose next use is farthest in future

OPT ใช้เป็น theoretical benchmark เพราะรู้อนาคต

### LRU

evict least recently used

### CLOCK

ใช้ reference-bit approximation + clock hand

---

## 10. Belady's Anomaly

classic FIFO sequence:

~~~text
1 2 3 4 1 2 5 1 2 3 4 5
~~~

ใน simulator:

~~~text
3 frames → 9 faults
4 frames → 10 faults
~~~

memory เพิ่มแต่ faults เพิ่ม

นี่คือ Belady's anomaly

---

## 11. Locality

Temporal locality:

สิ่งที่เพิ่งใช้มีแนวโน้มถูกใช้ซ้ำ

Spatial locality:

address ใกล้กันมีแนวโน้มถูกใช้ใกล้เวลาเดียวกัน

performance จริงมีหลาย layer:

~~~text
cache
TLB
page tables
RAM
storage-backed faults
~~~

ห้ามอธิบาย slowdown ทุกแบบว่าเป็น page fault

---

## 12. Working Set

working set = pages ที่ workload ใช้ active ในช่วงหน้าต่างหนึ่ง

ถ้า effective available memory ไม่พอ active working sets:

~~~text
reclaim
→ page needed again
→ fault/reload
→ reclaim
→ low useful progress
~~~

เกิด thrashing ได้

Lab ใช้ reasoning/simulation ไม่ตั้งใจทำ laptop thrash

---

## 13. Swap / zram

ตรวจ:

~~~bash
free -h
swapon --show
zramctl
vmstat 1 5
~~~

Swap ไม่ใช่ RAM ฟรี

trade-offs:

- storage latency
- I/O
- compression CPU cost
- reclaim overhead

configuration ต้องดูเครื่องจริง ไม่ assume ทุก Fedora เหมือนกัน

---

## 14. File Mapping

C# MemoryMappedFile ใช้สอน file-backed mapping

ReadWrite mode:
write mapping แล้ว flush/readback file

---

## 15. File-Backed Copy-on-Write

CopyOnWrite demo:

~~~text
file contains 10
↓
private COW view
↓
view writes 99
↓
view sees 99
file remains 10
~~~

นี่คือ file-backed private mapping COW

มันช่วยเข้าใจ COW principle แต่ไม่ใช่ demo fork address-space duplication โดยตรง

---

## 16. fork COW Concept

concept จาก Chapter 03:

~~~text
parent mapping
↓ fork
parent + child initially share backing where allowed
↓ write
COW fault
↓
writer gets private copy
~~~

C# Process.Start ไม่ expose fork memory-duplication model ดังนั้นคอร์สแยก conceptual fork COW ออกจาก MemoryMappedFile COW ให้ชัด

---

## Lab

~~~bash
dotnet run --project 11-virtual-memory/examples/Chapter11.csproj -- faults
dotnet run --project 11-virtual-memory/examples/Chapter11.csproj -- mmap
dotnet run --project 11-virtual-memory/examples/Chapter11.csproj -- cow
dotnet run --project 11-virtual-memory/examples/Chapter11.csproj -- protection
dotnet run --project 11-virtual-memory/examples/Chapter11.csproj -- replacement
dotnet run --project 11-virtual-memory/examples/Chapter11.csproj -- self-test
~~~

---

## แบบฝึกหัด

1. Virtual Memory vs Swap
2. Demand Paging
3. Page Fault เป็น bug เสมอไหม
4. minor vs major fault
5. anonymous mmap
6. first-touch behavior
7. mprotect
8. SIGSEGV path
9. FIFO trace
10. LRU trace
11. OPT trace
12. CLOCK trace
13. Belady anomaly
14. temporal locality
15. spatial locality
16. working set
17. thrashing
18. swap vs zram
19. MemoryMappedFile ReadWrite
20. file-backed COW vs fork COW
21. TLB miss vs page fault
22. cache miss vs page fault
23. ทำไมไม่ควร hardcode exact fault count
24. วิเคราะห์ vmstat si/so
25. ออกแบบ safe memory-pressure simulation

---

## Explain-It-Back

~~~text
VA access
→ translation
→ possible page fault
→ kernel validates mapping
→ populate/COW/protection decision
→ update mapping or signal
→ retry or terminate
~~~
