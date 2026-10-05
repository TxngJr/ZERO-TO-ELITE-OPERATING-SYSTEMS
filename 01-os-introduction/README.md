# Chapter 01 — Course Overview & OS Introduction

## 1. Goals

หลังบทนี้ต้องอธิบายได้ว่า:

- Operating System คืออะไร
- OS ต่างจาก Kernel อย่างไร
- Fedora, Linux kernel และ user-space tools เกี่ยวข้องกันอย่างไร
- User Space และ Kernel Space ต่างกันอย่างไร
- ทำไม application จึงไม่ควรเข้าถึง hardware โดยตรง
- System Call คืออะไร
- library function ไม่จำเป็นต้องเท่ากับ system call
- CPU, RAM, storage และ devices เชื่อมกับ OS อย่างไร
- boot ตั้งแต่ firmware ถึง user application มีภาพรวมอย่างไร
- ใช้ strace และ /proc เพื่อสังเกตระบบจริงได้

---

## 2. Why This Matters

เวลาเราเขียน:

~~~c
printf("Hello\n");
~~~

CPU ไม่ได้เข้าใจภาษา C และ terminal ไม่ได้อ่าน source code โดยตรง

สิ่งที่เกิดขึ้นจริงมีหลายชั้น:

~~~text
C source
  ↓ compiler/linker
Executable
  ↓ loader/kernel
Process
  ↓ library/runtime
System-call boundary
  ↓
Kernel
  ↓
driver / terminal subsystem
  ↓
displayed output
~~~

ถ้าไม่เข้าใจชั้นเหล่านี้ เรื่อง process, thread, scheduling, virtual memory และ synchronization จะกลายเป็นการจำศัพท์

---

## 3. Operating System คืออะไร

Operating System หรือ OS คือ software layer ที่ช่วย:

1. จัดการ resource
2. สร้าง abstraction
3. แยกและป้องกัน programs
4. ประสาน hardware กับ software
5. ให้ interface ที่ application ใช้งานได้

resource ตัวอย่าง:

- CPU time
- memory
- storage
- network
- GPU
- keyboard/mouse
- files
- devices

### 3.1 Resource manager

ถ้ามี 100 processes แต่ CPU มี logical CPUs จำนวนจำกัด OS ต้องตัดสินใจว่าใครได้รันเมื่อใด

### 3.2 Abstraction

แทนที่ทุก program ต้องรู้ sector จริงบน SSD เราใช้ abstraction เช่น:

~~~text
file
directory
file descriptor
process
socket
virtual memory
~~~

### 3.3 Isolation

process A ไม่ควรแก้ memory ของ process B ได้ตามใจ เพราะจะทำให้ระบบทั้งระบบพังง่ายและไม่ปลอดภัย

---

## 4. OS vs Kernel

คำสองคำนี้ไม่ควรใช้แทนกันแบบ 100%

### Kernel

Kernel คือแกน privileged ของ OS ทำงานด้านเช่น:

- scheduling
- memory management
- system-call handling
- device access
- process/thread management
- filesystems
- networking
- security enforcement

### Operating System

OS ในการใช้งานจริงกว้างกว่า kernel และรวม user-space components ด้วย

mental model:

~~~text
Applications
    ↓
Libraries / Runtime
    ↓
System Calls
    ↓
Linux Kernel
    ↓
Drivers
    ↓
Hardware
~~~

Fedora เป็น Linux distribution ซึ่งนำ Linux kernel มารวมกับ user-space packages, package management, configuration, desktop/server components และ integration ต่าง ๆ

---

## 5. Hardware → OS → Application

### CPU

CPU fetch/decode/execute instructions

OS ไม่ได้ "รันแทน CPU" แต่ kernel จัดการว่า execution context ใดได้ใช้ CPU

### RAM

RAM เก็บ instructions และ data ที่กำลังใช้งาน

ต่อไปเราจะเห็นว่า process ไม่ได้คิดเป็น physical RAM address ตรง ๆ แต่ใช้ virtual address

### Storage

SSD/NVMe เก็บ executable, files และ persistent data

### Devices

เช่น:

- keyboard
- mouse
- GPU
- NIC
- audio
- USB controller

kernel ใช้ drivers และ subsystem ต่าง ๆ เพื่อควบคุม/สื่อสารกับอุปกรณ์

---

## 6. User Space vs Kernel Space

### User Space

พื้นที่ execution ปกติของ applications เช่น:

- shell
- browser
- editor
- game
- gcc
- ps

process ปกติถูกจำกัดสิทธิ์ ไม่สามารถทำ privileged operations ได้ตามใจ

### Kernel Space

kernel ทำงานใน privileged mode และเข้าถึง resource ที่ user-space code ถูกห้าม

mental model:

~~~text
+----------------------------------+
| User Space                       |
| bash  gcc  browser  your program |
+---------------+------------------+
                |
                | system-call boundary
                v
+----------------------------------+
| Kernel Space                     |
| scheduler memory VFS network ... |
+---------------+------------------+
                |
                v
+----------------------------------+
| Hardware                         |
+----------------------------------+
~~~

คำว่า "space" ตรงนี้ไม่ใช่เพียง folder หรือพื้นที่บน disk แต่เป็นทั้ง execution/protection boundary และส่วนของ virtual address design

---

## 7. CPU Privilege

x86 มี privilege levels ที่มักเรียก rings

ใน Linux user programs โดยทั่วไปทำงานที่ privilege ต่ำกว่า kernel

แบบจำลองเพื่อเรียน:

~~~text
User mode
   |
   | controlled transition
   v
Kernel mode
~~~

อย่าจำว่า user mode = "ไม่มีสิทธิ์อะไรเลย"  
จริง ๆ process ยังอ่าน/เขียน memory ที่ OS อนุญาต และใช้ CPU instructions ที่ไม่ privileged ได้

สิ่งสำคัญคือ privileged operations ต้องผ่านกลไกที่ kernel ควบคุม

---

## 8. System Call

System call คือ interface ที่ user-space program ใช้ขอบริการจาก kernel

ตัวอย่าง:

- read
- write
- openat
- close
- fork
- clone
- execve
- wait4/waitid
- mmap

simplified flow:

~~~text
Application
   ↓
C library / runtime
   ↓
system-call interface
   ↓
kernel handler
   ↓
kernel subsystem
~~~

### 8.1 printf ไม่เท่ากับ syscall โดยตรง

printf เป็น C library function

มันอาจ:

- format data ใน user space
- buffer output
- สุดท้ายใช้ write-like operation เพื่อส่ง bytes ออก

ดังนั้น:

~~~text
library call ≠ necessarily one system call
~~~

และ system call หนึ่งครั้งก็อาจถูก wrapper ด้วย library API

---

## 9. Experiment: Hello → write

ไฟล์ examples/hello.c ใช้ printf

build:

~~~bash
cd 01-os-introduction
make
./bin/hello
~~~

ดู system calls:

~~~bash
strace ./bin/hello
~~~

output จะมี system calls หลายรายการ เพราะ process ต้อง startup, loader ต้องเตรียม libraries และ runtime ก่อน main

ให้หา write:

~~~bash
strace -e trace=write ./bin/hello
~~~

คำถาม:

1. เห็น write กี่ครั้ง
2. file descriptor ที่ใช้คือเลขอะไร
3. string ที่เขียนตรงกับ output หรือไม่

---

## 10. Direct syscall experiment

ไฟล์ examples/syscall-write.c ใช้ syscall wrapper พร้อม SYS_write เพื่อทำให้ boundary ชัดขึ้น

~~~bash
./bin/syscall-write
strace -e trace=write ./bin/syscall-write
~~~

หมายเหตุสำคัญ: ฟังก์ชัน syscall() ที่เรียกใน C ยังเป็น user-space library wrapper ที่จัดรูปแบบ argument และทำ instruction transition ที่เหมาะสมให้เรา เราไม่ได้เขียน assembly syscall instruction เองในบทนี้

---

## 11. File Descriptor Preview

ใน strace อาจเห็น:

~~~text
write(1, "Hello...", ...)
~~~

เลข 1 คือ stdout ตาม convention ของ process

โดยทั่วไป:

~~~text
0 = stdin
1 = stdout
2 = stderr
~~~

file descriptor คือ integer handle ที่ process ใช้อ้างถึง open file-like resource

เราจะเรียนเรื่องนี้ลึกขึ้นใน advanced track

---

## 12. Interrupt, Exception, Trap — Preview

อย่าเหมารวมทั้งหมดว่าเป็นสิ่งเดียวกัน

ระดับ introductory:

- interrupt: มักเป็น event ภายนอก CPU เช่น device/timer
- exception: event ที่เกิดสัมพันธ์กับ instruction ที่ CPU กำลังทำ
- system call: controlled request จาก user program เข้า kernel ผ่าน architecture-defined mechanism

คำศัพท์เชิง architecture มีรายละเอียดมากกว่านี้ จึงให้จำ mental model ไม่ใช่จำว่า "ทุกอย่างคือ interrupt"

---

## 13. Boot Overview

simplified Linux boot:

~~~text
Power On
   ↓
UEFI/Firmware
   ↓
Bootloader
   ↓
Linux Kernel
   ↓
initramfs / early userspace
   ↓
root filesystem
   ↓
systemd as system manager / PID 1
   ↓
services / login / graphical session
   ↓
applications
~~~

นี่เป็น simplified model

เครื่องจริงมีรายละเอียด เช่น firmware initialization, Secure Boot path, kernel command line, initramfs contents และ service dependencies

ลอง:

~~~bash
cat /proc/cmdline
systemctl --version
ps -p 1 -o pid,comm,args
~~~

---

## 14. Fedora System Exploration Lab

รันทีละคำสั่งและเขียนสิ่งที่ค้นพบ

### OS / Kernel

~~~bash
cat /etc/os-release
uname -a
uname -r
uname -m
~~~

### CPU

~~~bash
lscpu
nproc
~~~

หาคำตอบ:

- architecture
- model name
- CPU(s)
- core(s) per socket
- thread(s) per core

### Memory

~~~bash
free -h
head -40 /proc/meminfo
~~~

### Storage

~~~bash
lsblk
~~~

### PCI / USB

~~~bash
lspci
lsusb
~~~

### Processes

~~~bash
ps
ps -ef | head
top
~~~

กด q เพื่อออกจาก top

---

## 15. /proc คืออะไร

/proc เป็น pseudo-filesystem ที่ kernel expose information/control interfaces ผ่าน file-like paths

สำคัญ:

- หลายไฟล์ใน /proc ไม่ใช่ regular files ที่เก็บถาวรบน SSD
- content ถูก kernel สร้างให้เมื่ออ่าน
- /proc/PID เกี่ยวกับ process นั้น

ลอง:

~~~bash
cat /proc/version
cat /proc/uptime
cat /proc/cpuinfo | head -30
cat /proc/meminfo | head -30
ls /proc/$$
~~~

$$ ใน shell คือ PID ของ shell ปัจจุบัน

---

## 16. End-to-End Mental Model: printf("Hello")

simplified:

~~~text
source.c
  ↓ gcc
ELF executable
  ↓ shell requests execution
kernel creates/loads process context
  ↓
CPU executes user-space instructions
  ↓
printf formats/buffers text
  ↓
write-like system call
  ↓
controlled transition to kernel
  ↓
kernel handles fd 1 / terminal path
  ↓
terminal emulator / display stack eventually shows text
~~~

จุดสำคัญ: อย่าพูดว่า printf "ส่งให้ GPU โดยตรง"

มีหลาย layers ระหว่างกัน

---

## 17. Common Misconceptions

### "Linux = Fedora"

ผิด

Linux เป็น kernel; Fedora เป็น distribution ที่ใช้ Linux kernel และ ecosystem อื่น ๆ

### "Kernel คือ OS ทั้งหมดเสมอ"

ไม่แม่นสำหรับการพูดถึงระบบใช้งานจริง

### "System call = function ทุกตัวใน C"

ผิด

strlen สามารถทำงานทั้งหมดใน user space ได้

### "ทุก system call ทำให้ process ถูก context switch"

ผิด

การเปลี่ยน user mode → kernel mode ไม่จำเป็นต้องสลับไป process อื่น

### "/proc คือ folder ข้อมูลธรรมดาบน SSD"

ผิด

เป็น kernel-backed pseudo-filesystem

---

## 18. Performance Notes

system-call transition มี overhead มากกว่า ordinary function call เพราะต้องข้าม privilege boundary และให้ kernel ทำงาน

แต่ห้ามสรุปว่า "system calls ช้ามากเสมอ" โดยไม่วัด

หลัก systems engineering คือ measure

ต่อไปสามารถใช้ perf/strace time summaries เพื่อสังเกตได้

---

## 19. Security Notes

privilege separation ป้องกัน application จากการ:

- เขียน kernel memory ตามใจ
- ควบคุม device โดยไม่ผ่าน policy
- อ่าน memory ของ process อื่นแบบไม่มีสิทธิ์

Linux ยังมี layers เพิ่ม เช่น permissions, capabilities, SELinux, namespaces และ seccomp ซึ่งจะเรียนใน advanced track

---

## 20. Exercises

### Level 1 — Recall

1. OS มีหน้าที่หลักอะไร 3 อย่าง
2. Kernel คืออะไร
3. User Space คืออะไร
4. System Call คืออะไร
5. /proc เป็น regular disk filesystem หรือไม่

### Level 2 — Understanding

6. ทำไม browser ไม่ควรเขียน NVMe controller registers โดยตรง
7. อธิบาย OS vs Kernel ด้วยคำของตนเอง
8. ทำไม printf ไม่เท่ากับ write syscall
9. ทำไม privilege separation ช่วยเรื่อง reliability
10. Fedora เกี่ยวข้องกับ Linux อย่างไร

### Level 3 — Observe

11. หา kernel version จากเครื่อง
12. หา logical CPU count
13. หา PID ของ shell
14. ใช้ strace หา write ของ hello
15. ดู PID 1 และบอกชื่อ process

### Level 4 — Analyze

16. ถ้า program เรียก strlen("abc") จำเป็นต้องเข้า kernel หรือไม่ เพราะอะไร
17. ถ้า program เปิด file ต้องเกี่ยวข้องกับ kernel เพราะอะไร
18. อธิบาย flow จาก source code ถึง text บน terminal แบบอย่างน้อย 6 ขั้น

### Level 5 — Design

19. ออกแบบ experiment เพื่อพิสูจน์ว่า printf อาจ buffer output
20. ออกแบบ checklist เพื่อแยกข้อมูลที่มาจาก user space กับ kernel

---

## 21. Quiz

1. Kernel ทำงาน privileged หรือ unprivileged โดยทั่วไป
2. Fedora คือ kernel หรือ distribution
3. printf เป็น syscall โดยตรงหรือไม่
4. write(2) เลข 2 หมายถึงอะไร
5. mode switch เท่ากับ process context switch เสมอหรือไม่
6. /proc/PID/status ใช้ดูอะไร
7. PID 1 บน Fedora ปกติสัมพันธ์กับอะไร
8. CPU เป็นคน execute instruction หรือ OS
9. OS ทำ abstraction เพื่ออะไร
10. file descriptor 1 ตาม convention คืออะไร

### Answers

1. privileged
2. distribution
3. ไม่
4. man-page section 2 / system call
5. ไม่
6. process status/metadata ที่ kernel expose
7. systemd/system manager ในระบบปกติ
8. CPU
9. ซ่อน/จัดรูป resource ที่ซับซ้อนให้ใช้ผ่าน interface ที่ควบคุมได้
10. stdout

---

## 22. Explain-It-Back Checkpoint

อธิบายโดยไม่เปิดโน้ต:

1. ถ้ากดรันโปรแกรมหนึ่งตัว OS เข้ามาเกี่ยวตอนไหน
2. user mode กับ kernel mode ต่างกันอย่างไร
3. system call แก้ปัญหาอะไร
4. ทำไม /proc จึงมีประโยชน์ต่อการเรียน OS
5. printf("Hello") เดินทางผ่าน layer ใดบ้าง

ถ้ายังตอบข้อใดแบบเชื่อมเหตุผลไม่ได้ ให้กลับไปทำ Lab ก่อนขึ้น Chapter 02
