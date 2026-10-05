# Chapter 02 — Process & Context Switch Part I

## 1. Goals

หลังบทนี้ต้องอธิบายได้ว่า:

- Program ต่างจาก Process อย่างไร
- Process ต้องมี state อะไรบ้าง
- PID และ PPID คืออะไร
- textbook PCB คือ abstraction อะไร
- Linux task_struct เกี่ยวข้องอย่างไรโดยไม่บอกว่า "PCB = struct เดียว"
- CPU state ที่ execution context ต้องรักษามีอะไร
- process address space มีภาพรวมอย่างไร
- textbook process states ต่างจาก Linux state codes อย่างไร
- ใช้ /proc/PID เพื่อตรวจ process จริงได้

---

## 2. Program ≠ Process

### Program

program บน disk เป็น passive object เช่น ELF executable

ตัวอย่าง:

~~~bash
ls -l /usr/bin/sleep
file /usr/bin/sleep
~~~

file นั้นไม่ได้ "กำลังรัน" เพียงเพราะมีอยู่

### Process

process คือ executing instance พร้อม execution context และ kernel-managed state

simplified:

~~~text
Executable file
     |
     | execute
     v
+----------------------+
| Process              |
| PID                  |
| virtual address space|
| registers/context    |
| open file descriptors|
| credentials          |
| scheduling state     |
| signals              |
| kernel metadata      |
+----------------------+
~~~

program เดียวสามารถมีหลาย processes ได้

ลอง:

~~~bash
sleep 1000 &
sleep 1000 &
pgrep -a sleep
~~~

ทั้งสองอาจ execute program เดียวกัน แต่มี PID คนละตัว

cleanup:

~~~bash
pkill -x sleep
~~~

---

## 3. ELF Preview

Linux executable ทั่วไปใช้ ELF format

ลอง:

~~~bash
file /bin/ls
readelf -h /bin/ls | less
~~~

ตอนนี้รู้เพียงว่า ELF เก็บข้อมูลที่ loader/kernel/runtime ใช้ในการสร้าง process image

เรื่อง sections, segments, dynamic linking และ loader จะเรียนลึกใน systems/ELF track

---

## 4. Process Address Space

simplified conceptual layout:

~~~text
High virtual addresses
+---------------------------+
| Stack                     |
+---------------------------+
| mmap / shared libraries   |
| ...                       |
+---------------------------+
| Heap                      |
+---------------------------+
| BSS                       |
+---------------------------+
| Initialized Data          |
+---------------------------+
| Code / executable mapping |
+---------------------------+
Low virtual addresses
~~~

นี่ไม่ใช่กฎว่าทุก process ต้องวางเหมือนรูปเป๊ะ

ASLR, loader, PIE, mappings และ architecture ทำให้ layout จริงแตกต่างได้

### Code/Text

machine instructions ที่ executable mapping ใช้

### Data

global/static variables ที่มี initial value

### BSS concept

global/static storage ที่ไม่ต้องเก็บ initialized bytes แบบเดียวกับ data section เช่น zero-initialized objects

### Heap

dynamic allocation เช่น malloc มองจาก programmer level

แต่ malloc implementation อาจได้ memory จาก brk/mmap และจัดการเอง จึงอย่าจำว่า "malloc = syscall ทุกครั้ง"

### Stack

ใช้กับ call frames, local automatic variables และ control data ตาม ABI/runtime

แต่ละ thread จะมี stack ของตัวเองใน chapter concurrency

---

## 5. Lab: Observe Addresses

~~~bash
cd 02-process-context-I
make
./bin/memory-layout
~~~

ผลลัพธ์ address เปลี่ยนได้ระหว่าง run เพราะ ASLR/PIE

ลอง:

~~~bash
./bin/memory-layout
./bin/memory-layout
./bin/memory-layout
~~~

อย่าพยายามจำเลข address

ให้เข้าใจ relationship

ดู symbols/sections เพิ่ม:

~~~bash
readelf -S ./bin/memory-layout | less
nm -n ./bin/memory-layout | less
~~~

---

## 6. PID และ PPID

PID = Process ID  
PPID = Parent Process ID

ลอง:

~~~bash
echo $$
ps -o pid,ppid,stat,comm -p $$
~~~

run example:

~~~bash
./bin/process-info
~~~

โปรแกรมจะรอช่วงหนึ่งเพื่อให้เปิด terminal ใหม่แล้ว inspect PID

### PID ไม่ใช่ identity ถาวร

หลัง process จบ PID สามารถถูก reuse ภายหลังได้

ดังนั้นอย่าคิดว่า PID เป็น globally unique forever

---

## 7. Process Hierarchy

ดู:

~~~bash
pstree -p
~~~

processes มีความสัมพันธ์ parent/child แต่ Linux process relationships จริงมีรายละเอียดมากกว่าต้นไม้สำหรับทุก notion เช่น threads, namespaces และ subreapers

ในระบบ Fedora ปกติ PID 1 มักเป็น systemd

~~~bash
ps -p 1 -o pid,ppid,comm,args
~~~

---

## 8. Textbook Process States

โมเดลพื้นฐาน:

~~~text
             admitted
 New --------------------> Ready
                              |
                              | dispatch
                              v
                           Running
                          /   |    \
                         /    |     \
             wait/event /     |      \ exit
                       v      |       v
                    Waiting   |    Terminated
                       |      |
                       +------+
                       event
~~~

หลายตำราอาจแยก/ตั้งชื่อ state ต่างกัน

จุดประสงค์คือ reasoning ไม่ใช่บอกว่า kernel implementation ต้องมี enum เหมือนรูป

---

## 9. Linux Process State Codes

ps สามารถแสดง code เช่น:

- R = running or runnable
- S = interruptible sleep
- D = uninterruptible sleep โดยทั่วไปเกี่ยวกับ kernel wait state บางประเภท
- T = stopped/traced
- Z = zombie

ดู:

~~~bash
ps -eo pid,ppid,stat,comm | head -30
~~~

STAT อาจมีตัวอักษรเพิ่มเป็น flags

อย่าเทียบ textbook state แบบ one-to-one โดยไม่ดู definition ของ Linux tools/kernel

---

## 10. PCB — Process Control Block

ในวิชา OS, PCB คือ abstraction ของข้อมูลที่ OS ต้องเก็บเพื่อบริหาร process

เช่น:

~~~text
PCB concept
├── process identity
├── process state
├── CPU context
├── scheduling information
├── memory-management information
├── open resources
├── credentials/security
└── signals/accounting/etc.
~~~

### Linux relation

Linux มี kernel data structures หลายตัว โดย task_struct เป็น central task descriptor สำคัญ

แต่อย่าเขียน:

~~~text
PCB = task_struct แบบสมบูรณ์ 1:1
~~~

เพราะ textbook PCB เป็น conceptual model ขณะที่ Linux แยกข้อมูลบางอย่างผ่าน structures/pointers/subsystems อื่น ๆ

---

## 11. CPU State

สมมติ CPU กำลัง execute process A

state ที่สำคัญต่อ execution เช่น:

- instruction pointer / program counter concept
- stack pointer
- general-purpose registers
- flags/status register
- architecture-specific execution state

mental model:

~~~text
Process A
  |
  v
CPU
+------------------+
| RIP / PC concept |
| RSP / SP concept |
| Registers        |
| Flags            |
+------------------+
~~~

เมื่อ execution ของ A หยุดชั่วคราวและ B จะใช้ CPU state ที่จำเป็นของ A ต้องถูกเก็บไว้ในรูปแบบที่ kernel สามารถกลับมารันต่อได้อย่างถูกต้อง

รายละเอียด save/restore จริงขึ้นกับ architecture และ transition path

---

## 12. Why Context Exists

ลองคิดว่า:

~~~text
A: sum = sum + value
~~~

ถ้า CPU หยุด A กลางทางแล้วภายหลังกลับมา แต่ register values หาย โปรแกรมจะทำต่อไม่ได้อย่างถูกต้อง

ดังนั้น OS ต้องรักษา execution context

Chapter 03 จะเรียน context switch เต็มรูป

---

## 13. /proc/PID

ตอน process-info ยังรัน เปิด terminal อีกอัน:

~~~bash
ps -o pid,ppid,stat,comm -p PID
cat /proc/PID/status
cat /proc/PID/stat
cat /proc/PID/maps
ls -l /proc/PID/fd
~~~

แทน PID ด้วยเลขจริง

### status

human-readable process metadata จำนวนหนึ่ง

### stat

compact fields ใช้โดย tools จำนวนมาก ต้องอ่าน proc_pid_stat(5) ก่อน parse จริง

### maps

virtual memory mappings

### fd

symbolic links แสดง file descriptors ที่ process เปิดอยู่ ซึ่ง permission อาจจำกัดตาม security rules

---

## 14. /proc/self

process สามารถอ้างตัวเองด้วย:

~~~bash
ls -l /proc/self
~~~

แต่ระวัง: command เช่น ls เป็น process ตัวเอง ดังนั้น /proc/self ที่มันเปิดคือ PID ของ ls ไม่ใช่ shell เสมอ

สำหรับ shell ใช้:

~~~bash
ls /proc/$$
~~~

---

## 15. Process vs Program Table

| Property | Program | Process |
|---|---|---|
| Nature | passive file/code representation | active execution |
| PID | ไม่มี | มี |
| CPU context | ไม่มี runtime context | มี |
| Address space | ไม่ใช่ live address space | มี live virtual address space |
| Open FDs | ไม่มี per-process set | มี |
| Scheduling | ไม่ถูก schedule | execution entity ถูก scheduler จัดการ |

---

## 16. Common Misconceptions

### "เปิดไฟล์ executable = process"

ผิด การอ่าน file ยังไม่เท่ากับ executing instance

### "1 program มีได้แค่ 1 process"

ผิด

### "PCB คือ struct ชื่อ PCB ใน Linux"

ผิดในเชิง Linux implementation

### "ทุก process state มีเพียง 5 state ตามตำรา"

ผิด รูป 5-state เป็น teaching model

### "process อยู่ใน RAM ทั้งก้อนเสมอ"

ผิด mental model และจะชัดขึ้นใน virtual memory

### "PID ไม่เคยซ้ำ"

ผิด PID ถูก reuse ได้

---

## 17. Debugging Lab

ถ้า process จบก่อน inspect:

~~~bash
./bin/process-info
~~~

โปรแกรมให้เวลาสังเกตประมาณ 30 วินาที

ถ้าต้องการ PID:

~~~bash
pgrep -n process-info
~~~

หรือดู output จากโปรแกรม

---

## 18. Security Notes

ข้อมูลใน /proc บางส่วนถูกจำกัดด้วย:

- process ownership
- ptrace policy
- namespaces
- mount options
- Linux security controls

ดังนั้น "มี path ใน /proc" ไม่ได้แปลว่าทุก user อ่านได้ทั้งหมด

---

## 19. Performance Notes

process มี abstraction มากกว่าชุด instructions

kernel ต้อง track scheduling, memory, resources และ accounting

แต่ห้ามสรุปง่าย ๆ ว่า "process หนักเสมอ thread เบาเสมอ" โดยไม่ระบุ operation และ platform

เราจะวัดจริงในบทต่อ ๆ ไป

---

## 20. Exercises

1. Program คืออะไร
2. Process คืออะไร
3. ทำไม program เดียวมีหลาย PID ได้
4. PID กับ PPID ต่างกันอย่างไร
5. PID reuse หมายถึงอะไร
6. PCB เป็น concept หรือ Linux struct ที่ชื่อ PCB
7. task_struct คืออะไรในระดับสูง
8. อธิบาย Ready vs Running
9. อธิบาย Waiting/Blocked
10. Linux Z หมายถึงอะไร
11. instruction pointer สำคัญต่อการ resume อย่างไร
12. stack pointer สำคัญอย่างไร
13. /proc/PID/maps แสดงอะไร
14. /proc/PID/fd แสดงอะไร
15. ทำไม address ของ stack/heap เปลี่ยนข้าม run ได้
16. malloc จำเป็นต้อง syscall ทุกครั้งหรือไม่
17. อธิบาย process address space diagram และข้อจำกัดของ diagram
18. ใช้ ps หา PPID ของ shell
19. ใช้ pstree อธิบาย parent chain ของ terminal shell
20. ออกแบบ experiment ที่เปิด program เดียว 3 instance และพิสูจน์ว่าเป็น 3 processes

---

## 21. Quiz

1. Program เป็น passive หรือ active entity
2. Process มี PID หรือไม่
3. BSS concept เกี่ยวกับ storage แบบใด
4. heap เท่ากับ mmap syscall 1:1 หรือไม่
5. Linux R หมายถึงเฉพาะ "กำลัง execute บน CPU" หรือรวม runnable ได้
6. PCB เป็น textbook abstraction หรือไม่
7. task_struct เท่ากับ PCB 1:1 หรือไม่
8. context มี register state หรือไม่
9. PID สามารถ reuse หรือไม่
10. /proc/PID/maps ใช้สังเกตอะไร

### Answers

1. passive
2. มี
3. static/global zero/uninitialized storage concept
4. ไม่
5. รวม running/runnable ตาม reporting semantics
6. ใช่
7. ไม่ควรกล่าวแบบ 1:1
8. มี
9. ได้
10. virtual memory mappings

---

## 22. Explain-It-Back

โดยไม่เปิดโน้ต:

- Program vs Process
- Process state
- PID/PPID
- PCB vs Linux task_struct
- CPU state ที่ต้องรักษา
- /proc/PID แต่ละ path ที่ใช้ใน Lab

ถ้าอธิบายไม่ได้ ให้ทำ LAB-02 ซ้ำก่อน Chapter 03
