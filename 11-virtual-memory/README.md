# Chapter 11 — Virtual Memory

## 1. Goals

หลังบทนี้ต้องสามารถ:

- อธิบาย Virtual Memory โดยไม่สับสนกับ Swap
- เข้าใจ Demand Paging
- อธิบาย Page Fault flow
- แยก valid fault, protection fault และ invalid access
- เข้าใจ Minor vs Major Page Fault
- วิเคราะห์ FIFO, Optimal, LRU และ Clock/Second-Chance
- อธิบาย Belady's Anomaly
- เข้าใจ Temporal/Spatial Locality
- เข้าใจ Working Set และ Thrashing
- เข้าใจ Swap และ compressed swap devices แบบ concept
- ใช้ mmap/munmap
- แยก anonymous vs file-backed mapping
- เข้าใจ MAP_PRIVATE / MAP_SHARED
- เชื่อม Copy-on-Write กลับไป fork
- ใช้ mprotect เพื่อเข้าใจ page permissions
- อธิบาย SIGSEGV relationship กับ invalid/protection memory access
- ใช้ free, vmstat, swapon, /proc/PID/maps, /proc/PID/status และ getrusage

---

## 2. Virtual Memory ไม่เท่ากับ Swap

ประโยคผิด:

~~~text
Virtual Memory = RAM + Swap
~~~

model ที่ดีกว่า:

~~~text
Virtual Memory
=
per-process virtual address-space abstraction
+
address translation
+
protection
+
mapping
+
demand population
+
sharing/private mappings
+
optional backing by files/swap/etc.
~~~

Swap เป็นเพียงกลไก backing/reclaim หนึ่งส่วนของ memory management

---

## 3. Why Virtual Memory Matters

Virtual memory ช่วย:

- isolation
- address-space abstraction
- sparse allocation
- sharing
- file mapping
- Copy-on-Write
- permissions
- demand paging
- efficient process creation

สอง process สามารถใช้ virtual address เดียวกันแต่เห็นข้อมูลคนละ physical backing

---

# Demand Paging

## 4. Allocate Does Not Necessarily Mean Physical Page Now

application ขอ virtual range:

~~~text
mmap/malloc
   ↓
virtual mapping exists
   ↓
physical backing may be populated lazily
   ↓
first access
   ↓
page fault
   ↓
kernel resolves
~~~

exact behavior ขึ้นกับ mapping type, flags, allocator, overcommit, huge pages และ kernel implementation

---

## 5. Page Fault

Page Fault เป็น CPU exception ที่เกิดเมื่อ memory access ต้องให้ OS จัดการ translation/protection-related condition

~~~text
CPU issues memory access
        ↓
MMU checks translation
        ↓
cannot complete normally
        ↓
page-fault exception
        ↓
kernel fault handler
        ↓
valid resolvable?
   /                  \
 yes                  no
 |                     |
map/populate/fix      signal/error path
 |
resume instruction
~~~

สำคัญ:

~~~text
Page Fault ≠ program bug เสมอ
~~~

demand paging ใช้ page faults เป็น normal mechanism

---

## 6. Valid Fault

ตัวอย่าง:

- virtual mapping valid
- page ยังไม่มี physical backing populated
- kernel จัด page
- update page table
- instruction retry

application อาจไม่รู้เลยว่า fault เกิด

---

## 7. Invalid / Protection Access

ตัวอย่าง:

- access unmapped address
- write ไป read-only page
- execute page ที่ execute permission ถูกห้าม

kernel อาจส่ง signal เช่น SIGSEGV ตาม situation

~~~text
page-fault exception
→ kernel decision
→ may resume OR may signal
~~~

---

# Minor vs Major Fault

## 8. Minor Page Fault

โดยทั่วไป fault ที่ resolve ได้โดยไม่ต้องอ่าน page data จาก backing storage แบบ blocking I/O

ตัวอย่างอาจรวม:

- anonymous demand-zero page
- Copy-on-Write
- resident/cache-related mapping cases

exact accounting semantics เป็น kernel detail

---

## 9. Major Page Fault

fault ที่ต้องทำ I/O เพื่อเอาข้อมูล page จาก storage/backing store ใน accounting model

major fault มักแพงกว่า แต่ performance ต้องวัดจริง

---

## 10. Observe with getrusage

~~~bash
cd 11-virtual-memory
make
./bin/page-fault-demo
~~~

โปรแกรม:

1. ดู minor/major counters
2. mmap anonymous region
3. touch one byte per page
4. ดู counters อีกครั้ง

จำนวน faults อาจไม่เท่าจำนวน pages 1:1 เพราะ:

- Transparent Huge Pages
- kernel optimizations
- runtime activity
- accounting details

เป้าหมายคือสังเกต trend ไม่ใช่ท่อง exact number

---

# Page Replacement

## 11. Why Replacement Exists

เมื่อ physical frames จำกัด:

~~~text
choose victim page
↓
preserve/write back if needed
↓
reuse frame
~~~

ตำราใช้ page-reference string เพื่อเรียน policy

---

## 12. FIFO

First-In, First-Out:

~~~text
victim = page ที่เข้ามานานที่สุด
~~~

ง่าย แต่ไม่ใช้ locality โดยตรง

---

## 13. Optimal

เลือก page ที่จะถูกใช้อีกไกลที่สุดในอนาคต

ข้อดี:

- theoretical minimum faults สำหรับ model/reference string

ข้อเสีย:

- ต้องรู้อนาคต
- ใช้จริงตรง ๆ ไม่ได้

จึงใช้เป็น benchmark

---

## 14. LRU

Least Recently Used:

~~~text
victim = page ที่ไม่ได้ใช้นานที่สุดในอดีต
~~~

อาศัย temporal locality

exact true LRU อาจมี overhead สูง ระบบจริงจึงมักใช้ approximations/related algorithms

---

## 15. Clock / Second Chance

pages อยู่ในวงกลมพร้อม reference/use bit concept

~~~text
if reference=0:
    evict
else:
    clear bit
    advance hand
~~~

เป็น approximation ที่ practical กว่า exact LRU ในหลาย designs

---

## 16. Page Replacement Simulator

~~~bash
python3 page_replacement_sim.py
~~~

รองรับ:

~~~text
FIFO
OPT
LRU
CLOCK
~~~

และ demo Belady anomaly

---

# Belady's Anomaly

## 17. Counterintuitive Result

สำหรับ FIFO บาง reference strings:

~~~text
เพิ่มจำนวน frames
→ page faults กลับเพิ่ม
~~~

classic reference:

~~~text
1 2 3 4 1 2 5 1 2 3 4 5
~~~

stack algorithms เช่น idealized LRU/OPT ไม่มี anomaly แบบเดียวกันใน classic model

---

# Locality

## 18. Temporal Locality

สิ่งที่เพิ่งใช้มีโอกาสถูกใช้อีก:

- loop variables
- hot code
- frequently accessed objects

## 19. Spatial Locality

ถ้า access address หนึ่ง มีโอกาส access address ใกล้กัน:

- array traversal
- sequential instructions

paging/cache systems ได้ประโยชน์จาก locality

---

## 20. Locality Is Multi-Layered

performance difference อาจมาจาก:

- CPU cache
- TLB
- page locality
- prefetching
- memory controller

ดังนั้นอย่าอ้าง page faults อย่างเดียวโดยไม่วัด

---

# Working Set and Thrashing

## 21. Working Set

Working Set คือกลุ่ม pages ที่ process ใช้งาน active ในช่วงเวลา/หน้าต่างหนึ่ง

ถ้า memory รองรับ working sets ของ active processes ได้ fault rate มัก manageable

---

## 22. Thrashing

~~~text
working set > available effective memory
        ↓
frequent replacement
        ↓
frequent faults/reclaim
        ↓
low useful progress
~~~

ไม่ควรตั้งใจทำเครื่องหลักให้ thrash เพื่อเรียน

Lab ใช้ simulation/paper reasoning แทน

---

# Swap

## 23. What Swap Is

Swap เป็น storage-backed space ที่ kernel สามารถใช้เป็น backing สำหรับ memory บางชนิดตาม policy

ตรวจเครื่องจริง:

~~~bash
swapon --show
free -h
cat /proc/swaps
~~~

---

## 24. zram

บาง Linux/Fedora configurations อาจใช้ zram ซึ่งเป็น compressed block device ใน RAM

ตรวจ:

~~~bash
zramctl
swapon --show
~~~

อย่า assume ว่าทุก Fedora version/install ใช้ configuration เหมือนกัน

---

## 25. Swap Is Not Extra RAM

swap/compressed backing มี trade-offs:

- latency
- I/O cost
- CPU compression cost
- reclaim behavior

จึงไม่ควรอธิบายว่า swap เพิ่ม RAM แบบไม่มีค่าใช้จ่าย

---

# mmap

## 26. mmap Mental Model

~~~text
Virtual Range
    |
    +--> anonymous backing
    |
    +--> file-backed mapping
~~~

API:

~~~c
mmap(...)
munmap(...)
~~~

---

## 27. Anonymous Mapping

ไม่มี regular file เป็น source content

ใช้ได้กับ:

- allocator internals
- large regions
- arenas/stacks
- shared/private anonymous memory ตาม flags

---

## 28. File-Backed Mapping

~~~text
file
 |
 v
virtual mapping
 |
CPU loads/stores via memory instructions
~~~

kernel/page cache/filesystem ดูแล backing semantics

---

## 29. MAP_PRIVATE vs MAP_SHARED

MAP_PRIVATE:

- private modification semantics
- มักเกี่ยวกับ COW

MAP_SHARED:

- modifications สามารถ reflect ไป shared backing ตาม semantics

ทั้งคู่ยังต้อง synchronization ถ้าหลาย execution flows แก้ shared logical data

---

## 30. File Mapping Demo

~~~bash
./bin/mmap-file
~~~

โปรแกรม:

- สร้าง temporary file
- resize
- MAP_SHARED
- เขียนผ่าน mapping
- msync
- อ่านกลับ
- cleanup

---

# Copy-on-Write

## 31. Return to fork()

ก่อน fork:

~~~text
Parent VA page -> physical frame F
~~~

หลัง fork conceptually:

~~~text
Parent mapping --\
                  > same physical data while shareable
Child mapping  --/
~~~

เมื่อมี write:

~~~text
write/protection fault
↓
kernel allocates private copy
↓
writer mapping points to new backing
↓
other process keeps old content
~~~

---

## 32. COW Demo

~~~bash
./bin/cow-demo
~~~

สังเกต:

- parent/child สามารถ print VA เดียวกัน
- child เปลี่ยน private value
- parent ยังเห็นค่าเดิม

same VA ไม่ได้แปลว่า physical backing ต้องเหมือนกันหลัง write

---

# Memory Protection

## 33. mprotect

~~~c
mprotect(...)
~~~

ใช้เปลี่ยน page permissions

demo:

~~~text
RW
↓
R--
↓
child attempts write
↓
protection fault
↓
SIGSEGV
~~~

---

## 34. Controlled Protection Demo

~~~bash
./bin/protection-demo
~~~

parent ยังคงอยู่และรายงานว่า child ถูก signal อะไร

---

# Segmentation Fault

## 35. SIGSEGV Is a Signal

~~~text
invalid/protected memory access
↓
CPU exception/page-fault mechanism
↓
kernel cannot resolve access
↓
kernel sends SIGSEGV
↓
default action terminates process
~~~

คำว่า Segmentation Fault ใน Linux ไม่ได้หมายความว่า paging ไม่เกี่ยวข้อง

---

# Linux Observation

## 36. Memory Summary

~~~bash
free -h
vmstat 1 5
swapon --show
zramctl
~~~

snapshot เดียวไม่พิสูจน์ thrashing

---

## 37. Process Memory

~~~bash
cat /proc/$$/status
cat /proc/$$/maps
pmap -x $$
~~~

fields อาจรวม:

- VmSize
- VmRSS
- VmData
- VmStk

รายละเอียดขึ้นกับ kernel/tool version

---

## 38. Fault Counters

ถ้ามี GNU time:

~~~bash
/usr/bin/time -v ./bin/page-fault-demo
~~~

ดู:

- Major page faults
- Minor page faults
- Maximum resident set size

Fedora อาจติดตั้งเพิ่ม:

~~~bash
sudo dnf install time
~~~

---

# Performance

## 39. Memory Performance Is Multi-Layered

~~~text
CPU registers
↓
L1/L2/L3 caches
↓
TLB
↓
page tables
↓
RAM
↓
possible storage-backed fault path
~~~

ดังนั้น:

~~~text
TLB miss ≠ cache miss ≠ page fault
~~~

---

## 40. Page Size Trade-Off

larger page:

- fewer translations
- larger TLB reach
- fewer page-table entries

แต่:

- internal fragmentation
- coarser mapping/protection
- allocation constraints

---

# Common Misconceptions

## 41. Correct the Mental Model

- malloc = physical RAM immediately ❌
- every page fault = disk I/O ❌
- minor/major = bug severity ❌
- more frames always reduce FIFO faults ❌
- LRU knows future ❌
- Swap = Virtual Memory ❌
- same VA after fork means same private value forever ❌
- SIGSEGV means hardware RAM failure ❌

---

## 42. Exercises

1. Virtual Memory คืออะไร
2. Demand Paging คืออะไร
3. Page Fault เป็น bug เสมอหรือไม่
4. valid demand fault flow
5. invalid access flow
6. minor vs major fault
7. FIFO rule
8. Optimal rule
9. LRU rule
10. Clock rule
11. Belady anomaly
12. temporal locality
13. spatial locality
14. working set
15. thrashing
16. swap
17. zram concept
18. anonymous mapping
19. file-backed mapping
20. MAP_PRIVATE vs MAP_SHARED
21. Copy-on-Write
22. mprotect
23. SIGSEGV flow
24. TLB miss vs page fault vs cache miss
25. ทำไมไม่ควร intentionally thrash เครื่องหลัก
26. reference string 1,2,3,1,4,1,2 กับ 3 frames — FIFO
27. ทำข้อ 26 ด้วย LRU
28. วิเคราะห์ COW หลัง parent/child write
29. อธิบาย page-fault-demo counters
30. วาด end-to-end demand paging flow

---

## 43. Quiz

1. Virtual Memory = Swap หรือไม่
2. mmap anonymous region สามารถ demand-page ได้หรือไม่
3. page fault สามารถ resolve แล้ว retry instruction ได้หรือไม่
4. major fault โดยทั่วไปเกี่ยวกับ backing I/O มากกว่า minor หรือไม่
5. FIFO มี Belady anomaly ได้หรือไม่
6. Optimal ใช้จริงโดยรู้ future ได้หรือไม่
7. LRU อาศัย recent past หรือ future
8. thrashing ทำ useful progress ลดหรือไม่
9. MAP_PRIVATE เกี่ยวกับ private/COW semantics หรือไม่
10. parent/child หลัง COW write ต้องเห็นค่าเดียวกันหรือไม่
11. mprotect เปลี่ยน page permissions ได้หรือไม่
12. SIGSEGV สามารถตามหลัง protection fault ที่ resolve ไม่ได้หรือไม่

### Answers

1. ไม่
2. ได้
3. ได้
4. ใช่
5. ได้
6. ไม่
7. recent past
8. ใช่
9. ใช่
10. ไม่
11. ได้
12. ใช่

---

## 44. Explain-It-Back

อธิบาย:

~~~text
process accesses anonymous VA first time
→ TLB/page-table path
→ page not populated
→ page fault
→ kernel allocates/zeros/maps page
→ page table updated
→ instruction retries
→ normal subsequent access
~~~

จากนั้นอธิบาย:

~~~text
parent page
→ fork
→ COW sharing
→ child write
→ write/protection fault
→ private copy
→ parent and child values diverge
~~~

ถ้าอธิบายได้โดยไม่สับสน TLB miss กับ page fault ถือว่าจบ Core Memory Track
