# Chapter 01 — Course Overview & OS Introduction

## เป้าหมาย

หลังบทนี้ต้องอธิบายได้ว่า:

- Operating System คืออะไร
- Kernel คืออะไร
- User Space และ Kernel Space ต่างกันอย่างไร
- Application, Runtime, OS และ Hardware เชื่อมกันอย่างไร
- System Call คืออะไร
- Library/API call ต่างจาก system call อย่างไร
- Mode switch ต่างจาก process context switch อย่างไร
- C#/.NET อยู่ตรงไหนในระบบ
- ใช้ Fedora tools สังเกต process และ syscall อย่างไร

---

## 1. Operating System คืออะไร

Operating System เป็นระบบซอฟต์แวร์ที่จัดการ resource และสร้าง abstraction ให้ application

หน้าที่หลัก:

- CPU management
- process/thread management
- memory management
- file/storage management
- device I/O
- protection/isolation
- networking
- system-call interface

mental model:

~~~text
Application
↓
Language Runtime / Libraries
↓
Operating System
↓
Hardware
~~~

---

## 2. Kernel คืออะไร

Kernel คือ privileged core ของ OS

Linux kernel ทำงานเช่น:

- scheduling
- virtual memory
- filesystems
- drivers
- networking
- process/thread primitives
- protection

Fedora ไม่เท่ากับ kernel

~~~text
Fedora distribution
├── Linux kernel
├── system libraries
├── systemd
├── package manager
├── user tools
└── desktop/software
~~~

---

## 3. User Space vs Kernel Space

C# program ปกติรันใน user space

kernel รันด้วย privilege สูงกว่า

เหตุผลที่แยก:

- application crash ไม่ควรทำลาย kernel โดยตรง
- process หนึ่งไม่ควรอ่าน/เขียน memory process อื่นตามใจ
- hardware access ต้องถูกควบคุม
- resource ownership ต้องมี policy

---

## 4. C#/.NET Layer

คอร์สนี้ใช้ C# แต่ต้องไม่ลืมว่า C# ไม่ใช่ kernel language ในการทดลองของเรา

~~~text
C# source
↓
compiler
↓
IL / assembly
↓
CoreCLR / .NET runtime
↓
BCL / native runtime
↓
Linux syscall interface
↓
kernel
↓
hardware
~~~

ดังนั้น API เช่น:

~~~csharp
Console.WriteLine("Hello");
~~~

ไม่ใช่ Linux syscall ชื่อ Console.WriteLine

---

## 5. System Call

System call คือ controlled entry จาก user space ไปใช้บริการ kernel

ตัวอย่าง Linux concepts:

- read
- write
- openat
- mmap
- clone
- execve
- wait4
- futex

แต่ high-level C# API อาจเรียกหลาย native function/syscall และ implementation เปลี่ยนตาม runtime version

กฎของคอร์ส:

~~~text
C# API semantics
≠
ต้อง map 1:1 ไป syscall ชื่อเดียว
~~~

---

## 6. Mode Switch vs Context Switch

### Mode switch

CPU เปลี่ยน privilege context เช่น user → kernel เพื่อจัดการ syscall/exception

ไม่จำเป็นต้องเปลี่ยน process

### Context switch

CPU execution เปลี่ยนจาก task/thread หนึ่งไปอีก task/thread

ต้องมีการ save/restore state ที่จำเป็น

ดังนั้น:

~~~text
system call
does not automatically mean
switch to another process
~~~

---

## 7. Interrupt / Exception / System Call

ระดับ introductory:

~~~text
interrupt
→ event จาก hardware/asynchronous source

exception
→ CPU-detected event ระหว่าง instruction execution

system call
→ software-requested controlled kernel service
~~~

รายละเอียด architecture-specific จะเรียนเพิ่มใน advanced track

---

## 8. Observe Fedora

~~~bash
uname -r
cat /etc/os-release
lscpu
free -h
lsblk
ps -ef
cat /proc/self/status
~~~

ดู syscall:

~~~bash
dotnet build -c Release 01-os-introduction/examples/Chapter01.csproj

strace -f   -e trace=write,writev   dotnet run --no-build -c Release   --project 01-os-introduction/examples/Chapter01.csproj
~~~

---

## 9. P/Invoke Demo

คอร์สมี raw-write mode เพื่อให้เห็น native boundary:

~~~csharp
[DllImport("libc")]
static extern nint write(...);
~~~

นี่ **ยังเป็น C# source**

แต่ใช้ interop เพื่อเรียก native API ที่ใกล้ Linux syscall layer มากขึ้น

---

## 10. Common Misconceptions

ผิด:

~~~text
OS = kernel อย่างเดียว
C# API = syscall
system call = context switch
Thread = CPU core
/proc = directory files ปกติบน disk ทั้งหมด
~~~

ถูก:

~~~text
OS กว้างกว่า kernel
runtime ซ่อน native implementation หลายชั้น
syscall อาจ return กลับ task เดิม
Thread เป็น schedulable execution abstraction
/proc เป็น kernel-provided pseudo filesystem
~~~

---

## แบบฝึกหัด

1. อธิบาย OS และ kernel ต่างกันอย่างไร
2. Fedora กับ Linux kernel ต่างกันอย่างไร
3. ทำไม application ไม่ควรรัน kernel privilege
4. C# program อยู่ user space หรือ kernel space
5. System call แก้ปัญหาอะไร
6. Console.WriteLine เป็น syscall โดยตรงหรือไม่
7. mode switch vs context switch
8. interrupt vs exception
9. ใช้ strace ดู write/writev
10. ใช้ /proc/self/status แล้วหา Threads
11. เพิ่ม mode info แสดง ProcessId, ProcessorCount, SystemPageSize
12. เขียน C# อ่าน /proc/self/status
13. ทำนายว่า Console.WriteLine 3 ครั้งต้องเกิด write syscall 3 ครั้งหรือไม่
14. อธิบายเหตุผลที่ runtime implementation ไม่ควรถูก hardcode ในทฤษฎี
15. วาด C# → .NET → kernel → hardware

---

## Explain-It-Back

อธิบาย chain:

~~~text
C# code
→ runtime/library
→ system-call boundary
→ kernel
→ hardware
→ return to user space
~~~
