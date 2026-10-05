# Chapter 10 — Address Translation

## 1. Goals

หลังบทนี้ต้องสามารถ:

- แยก Virtual Address และ Physical Address
- อธิบายว่าทำไม process ไม่ควรใช้ physical address โดยตรง
- เข้าใจ MMU
- แยก Page กับ Frame
- แยก Virtual Page Number (VPN) กับ Page Offset
- คำนวณ offset bits จาก page size
- แปล address ผ่าน page table แบบจำลอง
- เข้าใจ Page Table Entry (PTE)
- อธิบาย Multi-Level Page Table
- อธิบาย TLB hit / miss
- เข้าใจความสัมพันธ์ Context Switch ↔ address space ↔ TLB
- ใช้ /proc/PID/maps, pmap และ getconf บน Fedora
- เข้าใจข้อจำกัดของ /proc/PID/pagemap และเหตุผลที่ user ปกติไม่ควรพยายามอ่าน PFN ตรง ๆ

---

# Part I — Why Address Translation Exists

## 2. ปัญหาถ้าโปรแกรมใช้ Physical Address โดยตรง

สมมติ Process A และ B เขียน physical RAM ตรง ๆ:

~~~text
Process A ---> RAM address 0x1000
Process B ---> RAM address 0x1000
~~~

จะเกิดปัญหา:

- isolation แย่
- relocation ยาก
- memory allocation ซับซ้อน
- process หนึ่งอาจทำลายอีก process
- process ต้องรู้ layout ของ RAM จริง

OS + hardware จึงสร้าง abstraction:

~~~text
Process sees Virtual Address Space
              |
              v
             MMU
              |
              v
        Physical Memory
~~~

---

## 3. Virtual Address

Virtual Address (VA) คือ address ที่ CPU instruction ของ process ใช้อ้างถึงภายใต้ address space ของ process

สำคัญ:

~~~text
VA 0x400000 ของ Process A
≠
VA 0x400000 ของ Process B ต้อง map ไป physical frame เดียวกัน
~~~

แต่บาง mappings อาจ intentionally share physical backing เช่น shared libraries/shared memory

---

## 4. Physical Address

Physical Address คือ address ใน physical memory address space ที่ memory subsystem ใช้หลัง translation

process user-space ปกติไม่ควรต้องรู้ physical address

---

## 5. Logical vs Virtual Address

ตำราบางเล่มใช้คำ:

~~~text
Logical Address
Virtual Address
~~~

ใกล้เคียงกันใน paging discussion

แต่ architecture บางยุคมี segmentation/logical-address terminology แยกชัด

ในคอร์สนี้ Chapter 10 ใช้คำว่า **Virtual Address** เป็นหลักเพื่อสอดคล้อง Linux/x86-64 modern mental model

---

# Part II — MMU

## 6. Memory Management Unit

MMU เป็น hardware logic ที่ช่วย translate virtual address และ enforce permissions

mental model:

~~~text
CPU generates VA
      |
      v
+-------------+
| MMU / TLB   |
+-------------+
      |
      | translation
      v
Physical Address
      |
      v
RAM / cache hierarchy
~~~

OS มีหน้าที่สร้าง/แก้ page tables และกำหนด mapping/policy

hardware ใช้ translation structures ระหว่าง execution

---

## 7. Address Translation ไม่ได้เกิดผ่าน Kernel Function Call ทุก Load

อย่าเข้าใจว่า:

~~~text
CPU load
→ syscall
→ kernel translate
→ RAM
~~~

ผิด

normal mapped memory access ใช้ hardware MMU/TLB โดยตรง

kernel เข้ามาเมื่อ:

- mapping ต้องถูกสร้าง/แก้
- page fault
- protection fault
- memory management event

---

# Part III — Paging

## 8. Page และ Frame

Virtual address space แบ่งเป็น **pages**

Physical memory model แบ่งเป็น **frames**

~~~text
Virtual Pages              Physical Frames

VPN 0 -----VPN 1 ------+---------> PFN 9
VPN 2 ---------------> PFN 2
VPN 3 -----/
~~~

mapping ไม่จำเป็นต้อง contiguous ทาง physical แม้ virtual pages จะติดกัน

---

## 9. Base Page Size

บน Linux/x86-64 commonly used base page size คือ 4096 bytes (4 KiB) แต่ code ต้อง query runtime value แทนการ hardcode

ตรวจ:

~~~bash
getconf PAGESIZE
~~~

หรือ C:

~~~c
sysconf(_SC_PAGESIZE)
~~~

---

## 10. Virtual Address Split

ถ้า page size = 4096 bytes:

~~~text
4096 = 2^12
~~~

ดังนั้น offset ใช้ 12 bits

~~~text
Virtual Address
+------------------------+------------+
| Virtual Page Number    | Offset     |
+------------------------+------------+
                          12 bits
~~~

สูตร:

~~~text
offset = VA mod page_size
VPN    = VA / page_size
~~~

หรือถ้า page size เป็น power of two:

~~~text
offset = VA & (page_size - 1)
VPN    = VA >> offset_bits
~~~

---

## 11. Example

VA:

~~~text
0x0000000012345678
~~~

page size:

~~~text
0x1000
~~~

offset:

~~~text
0x678
~~~

VPN:

~~~text
0x12345
~~~

หาก page table บอก:

~~~text
VPN 0x12345 → PFN 0xABC
~~~

physical address:

~~~text
PA = PFN * page_size + offset
   = 0xABC000 + 0x678
   = 0xABC678
~~~

---

# Part IV — Page Table

## 12. Page Table Concept

Page table เก็บ translation metadata:

~~~text
VPN → PTE → PFN + flags
~~~

ตัวอย่าง conceptual:

| VPN | Present | Writable | Executable | PFN |
|---:|---|---|---|---:|
| 0 | yes | no | yes | 12 |
| 1 | yes | yes | no | 4 |
| 2 | no | — | — | — |

exact bit names/encoding ขึ้นกับ architecture

---

## 13. PTE Properties

แนวคิดที่พบบ่อย:

- present / valid
- writable
- user/supervisor access
- execute-disable / executable policy
- accessed/reference indication
- dirty indication
- physical frame information

อย่าท่องว่า flags ทุก architecture เหมือนกัน

---

## 14. Protection

Page Table ไม่ได้มีไว้หา physical frame อย่างเดียว

มันยังช่วย enforce:

~~~text
Read
Write
Execute
User / Supervisor
~~~

เช่น code mapping อาจ:

~~~text
r-x
~~~

data:

~~~text
rw-
~~~

stack:

~~~text
rw-
~~~

ดูได้จาก:

~~~bash
cat /proc/$$/maps
~~~

---

# Part V — Why Multi-Level Page Tables

## 15. Flat Page Table Problem

สมมติ virtual address space ใหญ่มาก แต่ process ใช้จริงเพียงบางส่วน

ถ้ามี giant flat table ที่มี entry สำหรับทุก virtual page จะเปลือง memory

multi-level structure ช่วย allocate lower-level tables เฉพาะพื้นที่ที่ต้องใช้

---

## 16. Tree Mental Model

~~~text
Virtual Address
     |
     v
Level 1 index
     |
     v
Level 2 table
     |
     v
Level 3 table
     |
     v
Level 4 table
     |
     v
PTE
     |
     v
Physical Frame
~~~

ระดับจริงขึ้นกับ architecture/configuration

---

## 17. x86-64 Reality

x86-64 Linux รองรับทั้ง 4-level และ 5-level paging บน hardware/kernel ที่เหมาะสม

ดังนั้นห้ามเขียนว่า:

~~~text
x86-64 = 4 levels เสมอ
~~~

หรือ:

~~~text
ทุกเครื่องใช้ 5 levels
~~~

kernel สามารถ build รองรับ 5-level และทำงานบน hardware 4-level โดย fold level ที่ไม่ใช้ได้

สำหรับการเรียนใน Lab เราใช้ generic multi-level model ก่อน

---

## 18. Canonical Address Concept

x86-64 ไม่ได้ใช้ 64 address bits ทั้งหมดเป็น arbitrary virtual address bits ในทุก mode

valid virtual addresses ต้องอยู่ใน canonical form ตาม active paging mode

อย่าพยายามเดา usable user-space width จากคำว่า "64-bit" อย่างเดียว

---

# Part VI — TLB

## 19. Translation Lookaside Buffer

ถ้าทุก memory access ต้องเดิน page-table hierarchy จาก RAM หลายครั้ง performance จะแย่มาก

CPU จึงมี TLB cache สำหรับ translations

~~~text
Virtual Address
      |
      v
   TLB lookup
   /        \
 hit        miss
 |           |
 v           v
PFN      page-table walk
              |
              v
          fill TLB
~~~

---

## 20. TLB Hit

translation ที่ต้องการอยู่ใน TLB

ข้อดี:

- ลด page-table walk overhead

## 21. TLB Miss

ไม่พบ translation ใน TLB

hardware/kernel interaction exact ขึ้นกับ architecture

บน x86 page-table walk ปกติทำโดย hardware page walker

TLB miss **ไม่เท่ากับ page fault**

นี่สำคัญมาก

---

## 22. TLB Miss vs Page Fault

### TLB Miss

translation ไม่อยู่ใน translation cache

แต่ mapping อาจ valid และ present

### Page Fault

memory access ต้องให้ OS xử理 exception เพราะ mapping/protection/current residency state ต้องถูกจัดการ

ดังนั้น:

~~~text
TLB miss ≠ page fault
~~~

---

# Part VII — Context Switch and TLB

## 23. Address Space Switch

Process A และ B มี page-table context ต่างกันได้

~~~text
Process A VA 0x1000 → PFN 10
Process B VA 0x1000 → PFN 44
~~~

context switch ข้าม address spaces ต้องทำให้ CPU translation context สอดคล้องกับ process ใหม่

---

## 24. ไม่ควรจำว่า Context Switch = Flush TLB ทั้งหมดเสมอ

modern CPUs/OS มี mechanisms เช่น address-space tags/PCID-like techniques เพื่อรักษา translations บางส่วนข้าม switches

ดังนั้น:

~~~text
context switch may affect translation state
≠
always full TLB flush
~~~

exact behavior ขึ้นกับ architecture/kernel/hardware

---

# Part VIII — Linux Observation

## 25. /proc/PID/maps

run:

~~~bash
cat /proc/$$/maps
~~~

ตัวอย่าง permission field:

~~~text
r-xp
rw-p
r--p
~~~

ตัวอักษรหลัก:

- r read
- w write
- x execute
- p private
- s shared

---

## 26. pmap

~~~bash
pmap $$
pmap -x $$
~~~

ถ้ายังไม่มี:

~~~bash
sudo dnf install procps-ng
~~~

pmap อ่านข้อมูล mappings และแสดง view ที่ใช้งานง่ายขึ้น

---

## 27. Address Observation Program

build:

~~~bash
cd 10-address-translation
make
./bin/address-demo
~~~

โปรแกรมแสดง:

- runtime page size
- address ของ function
- global/static
- heap
- stack
- VPN
- offset

รันซ้ำเพื่อสังเกต ASLR

---

## 28. Address Split Calculator

~~~bash
./bin/address-split 0x12345678
~~~

โปรแกรมใช้ page size จากระบบ แล้วคำนวณ:

- decimal/hex VA
- page size
- VPN
- offset

---

# Part IX — Translation Simulator

## 29. ทำไมต้องมี Simulator

user-space process ปกติไม่สามารถถาม physical PFN อย่างเสรีและ portable ได้

แทนที่จะพยายาม bypass protection เราจะเรียน translation ด้วย simulator ที่ explicit

~~~bash
python3 page_table_sim.py
~~~

มันสร้าง page table จำลองและแปล:

~~~text
VA → VPN + offset → PFN + offset → PA
~~~

พร้อมกรณี unmapped page

---

## 30. /proc/PID/pagemap Warning

Linux มี pagemap interface สำหรับตรวจ page-table-related information

แต่ physical frame numbers ถูกจำกัดสำหรับ unprivileged users ใน modern kernels เพราะ security concerns

ดังนั้น Lab นี้:

- ไม่ใช้ sudo เพื่อดึง PFN
- ไม่พยายาม bypass restriction
- ใช้ /proc/PID/maps สำหรับ mapping view
- ใช้ simulator เพื่อเข้าใจ VPN→PFN

นี่เป็นทั้ง security lesson และ OS lesson

---

# Part X — Huge Pages Preview

## 31. Why Larger Pages Exist

larger pages:

- ลดจำนวน page-table entries
- เพิ่ม TLB coverage

แต่:

- fragmentation trade-offs
- allocation complexity
- less fine-grained mapping

Linux มี huge-page mechanisms เช่น explicit HugeTLB และ Transparent Huge Pages

รายละเอียดลึกอยู่ Advanced Memory Track

---

# Part XI — Calculations

## 32. Offset Bits

ถ้า page size:

~~~text
1 KiB  = 2^10 → offset 10 bits
4 KiB  = 2^12 → offset 12 bits
16 KiB = 2^14 → offset 14 bits
2 MiB  = 2^21 → offset 21 bits
~~~

---

## 33. Number of Pages

virtual region size:

~~~text
16 MiB
~~~

page size:

~~~text
4 KiB
~~~

จำนวน pages:

~~~text
16 MiB / 4 KiB
= 2^24 / 2^12
= 2^12
= 4096 pages
~~~

---

## 34. Translation Exercise

Given:

~~~text
Page size = 4 KiB
VA = 0x00012ABC
VPN 0x12 maps to PFN 0x9F
~~~

ต้องระวัง: แยก VPN ให้ถูกจาก 12-bit offset

~~~text
VPN = VA >> 12
offset = VA & 0xFFF
~~~

แล้ว:

~~~text
PA = (PFN << 12) | offset
~~~

---

# Part XII — Common Misconceptions

### "Virtual memory คือ swap"

ผิด Virtual Address Space/translation เป็น abstraction ใหญ่กว่านั้น

### "VA คือ address ใน RAM จริง"

ผิด ต้อง translate

### "TLB miss = page fault"

ผิด

### "Page table อยู่ใน TLB"

ผิด TLB เป็น cache ของ translation information

### "MMU เป็น software"

ผิด core translation enforcement เป็น hardware subsystem; OS configures structures/policy

### "ทุก x86-64 ใช้ page table 4 levels"

ผิด modern architecture/kernel รองรับ 5-level ด้วย

### "user process ต้องรู้ PFN เพื่อใช้ memory"

ผิด

---

## 35. Performance Notes

translation cost เกี่ยวกับ:

- TLB locality
- page-table walks
- page size
- working set
- context/address-space switches
- cache hierarchy

นี่เป็นเหตุผลที่ data layout และ locality มีผลต่อ performance แม้ algorithm complexity เท่ากัน

---

## 36. Security Notes

page permissions เป็นเสาหลักของ:

- process isolation
- W^X policy concepts
- NX/execute disable
- user/kernel protection

physical-address disclosure ยังอาจเป็น security-sensitive information ดังนั้น Linux จำกัด PFN visibility ใน pagemap สำหรับ unprivileged users

---

## 37. Exercises

1. Virtual Address คืออะไร
2. Physical Address คืออะไร
3. MMU ทำอะไร
4. Page vs Frame
5. VPN vs PFN
6. 4 KiB page ใช้ offset กี่ bits
7. 2 MiB page ใช้ offset กี่ bits
8. VA 0x12345 กับ 4 KiB page มี offset เท่าไร
9. page table มีไว้ทำอะไร 2 หน้าที่
10. PTE permissions สำคัญอย่างไร
11. ทำไม flat page table เปลือง
12. multi-level ช่วยอย่างไร
13. TLB คืออะไร
14. TLB hit vs miss
15. TLB miss vs page fault
16. Context switch เกี่ยวกับ address space อย่างไร
17. ทำไมไม่ควร assume full TLB flush
18. /proc/PID/maps แสดงอะไร
19. ทำไม PFN ถูกจำกัด
20. region 64 MiB ใช้กี่ 4 KiB pages
21. เขียน translation formula
22. อธิบาย ASLR ผ่าน address-demo
23. canonical address หมายถึงอะไรระดับ concept
24. huge pages trade-off
25. วาด end-to-end VA→PA flow

---

## 38. Quiz

1. process ปกติใช้ virtual address หรือ physical address
2. MMU เกี่ยวกับ translation หรือไม่
3. 4096 bytes = 2^12 หรือไม่
4. offset เปลี่ยนเมื่อ translate page หรือไม่
5. PFN มาจาก page-table mapping หรือไม่
6. TLB cache translation หรือไม่
7. TLB miss ต้อง page fault ทุกครั้งหรือไม่
8. multi-level tables ลด memory waste สำหรับ sparse spaces หรือไม่
9. x86-64 รองรับ 5-level paging หรือไม่
10. context switch ต้อง flush TLB ทั้งหมดเสมอหรือไม่
11. /proc/PID/maps ให้ mapping ranges/permissions หรือไม่
12. user program ต้องใช้ physical PFN เพื่อ malloc หรือไม่

### Answers

1. virtual
2. ใช่
3. ใช่
4. ไม่
5. ใช่
6. ใช่
7. ไม่
8. ใช่
9. ใช่บน supporting hardware/kernel
10. ไม่
11. ใช่
12. ไม่

---

## 39. Explain-It-Back

อธิบาย chain โดยไม่เปิดโน้ต:

~~~text
CPU instruction
→ Virtual Address
→ VPN + Offset
→ TLB lookup
→ possible page-table walk
→ PTE permissions + PFN
→ Physical Address
→ memory hierarchy
~~~

จากนั้นอธิบาย:

- TLB miss ตรงไหน
- Page Fault จะเกิดเมื่อไร
- Kernel เกี่ยวเมื่อไร
- Context Switch เกี่ยวกับ translation state อย่างไร

ถ้าตอบได้ พร้อม Chapter 11
