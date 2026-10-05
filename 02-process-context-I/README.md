# Chapter 02 — Process & Context Switch Part I

## เป้าหมาย

หลังบทนี้ต้องเข้าใจ:

- Program vs Process
- PID / PPID / TID
- Process State
- PCB concept
- CPU State
- Address Space
- File descriptors/resources
- managed vs unmanaged memory
- /proc/PID
- process vs thread boundary

---

## 1. Program vs Process

Program คือ code/data representation ที่ยังไม่ execute

Process คือ instance ที่ OS กำลังจัดการ

process มีอย่างน้อย:

- identity
- execution state
- virtual address space
- open resources
- threads
- scheduling metadata
- credentials/security context

---

## 2. PID และ PPID

PID = Process ID

PPID = Parent Process ID

C#:

~~~csharp
Environment.ProcessId
~~~

Linux:

~~~bash
ps -o pid,ppid,stat,comm -p PID
~~~

---

## 3. Thread ID

หนึ่ง process มีหลาย threads ได้

Linux มองแต่ละ thread เป็น task ที่ scheduler จัดการ

C# มี ManagedThreadId แต่ห้าม assume ว่าเลขนี้คือ Linux TID

ใน lab เราใช้ gettid ผ่าน P/Invoke เพื่อเปรียบเทียบ

---

## 4. Process State

textbook model:

~~~text
New
Ready
Running
Waiting/Blocked
Terminated
~~~

Linux ps ใช้ state codes เช่น:

~~~text
R runnable/running
S interruptible sleep
D uninterruptible sleep
T stopped/traced
Z zombie
~~~

textbook model กับ Linux implementation ไม่ map 1:1 ทุกกรณี

---

## 5. PCB คือ abstraction

PCB = Process Control Block เป็นชื่อเชิงตำราสำหรับข้อมูลที่ OS ต้องเก็บเกี่ยวกับ process

ตัวอย่าง:

- PID
- state
- registers/context
- scheduling info
- memory-management info
- resource info

Linux ไม่ได้มี C# class ชื่อ PCB

Linux kernel ใช้ structures หลายส่วน โดย task_struct เป็น structure สำคัญตัวหนึ่ง

---

## 6. CPU State

เวลาหยุด thread แล้วกลับมารัน ต้องรักษา state ที่จำเป็น เช่น:

- instruction pointer
- stack pointer
- general registers
- flags
- architecture-specific state

นี่คือพื้นฐานของ context switch ใน Chapter 03

---

## 7. Process Address Space

process ใช้ virtual address space

conceptual view:

~~~text
code/runtime mappings
read-only mappings
managed heap
native allocations
shared libraries
thread stacks
memory-mapped regions
~~~

อย่า assume ว่า layout ตายตัว

ASLR และ runtime allocation ทำให้ addresses เปลี่ยนได้

---

## 8. Managed vs Unmanaged Memory

managed object:

- อยู่ภายใต้ GC
- runtime จัด lifecycle
- object อาจถูกย้ายได้ในบางสถานการณ์

unmanaged allocation:

~~~csharp
Marshal.AllocHGlobal(...)
~~~

เหมาะกับ lab ที่ต้องถือ pointer-like address โดยตรง

แต่ Chapter 11 จะใช้ mmap ผ่าน P/Invoke เพื่อควบคุม VM mapping ชัดกว่า

---

## 9. Process.GetCurrentProcess

~~~csharp
using Process p = Process.GetCurrentProcess();

Console.WriteLine(p.Id);
Console.WriteLine(p.Threads.Count);
Console.WriteLine(p.WorkingSet64);
~~~

ค่าต่าง ๆ เป็น observation ผ่าน .NET abstraction

อย่าเหมารวม:

~~~text
WorkingSet64 = virtual address space ทั้งหมด
~~~

---

## 10. /proc/PID

สำคัญ:

~~~bash
cat /proc/PID/status
cat /proc/PID/maps
ls -l /proc/PID/fd
ls /proc/PID/task
pmap -x PID
~~~

status:
- process state
- memory summary
- thread count

maps:
- virtual mappings
- permissions
- backing/pathname

fd:
- open file descriptors

task:
- Linux task/thread IDs

---

## 11. Lab

~~~bash
dotnet run --project 02-process-context-I/examples/Chapter02.csproj
~~~

ระหว่าง sleep เปิด terminal ใหม่ตรวจ:

~~~bash
ps -o pid,ppid,stat,comm -p PID
cat /proc/PID/status
cat /proc/PID/maps
ls /proc/PID/task
pmap -x PID
~~~

---

## แบบฝึกหัด

1. Program vs Process
2. PID vs PPID
3. ManagedThreadId vs Linux TID
4. PCB เป็น implementation name จริงทุก OS หรือไม่
5. CPU context มีอะไรบ้าง
6. ทำไม process ต้องมี virtual address space
7. WorkingSet64 คือ virtual memory ทั้งหมดหรือไม่
8. อ่าน VmRSS จาก /proc/self/status
9. อ่าน Threads จาก /proc/self/status
10. สร้าง unmanaged allocation 1 MiB
11. inspect mapping ที่เกี่ยวข้อง
12. สร้าง 4 threads แล้วดู /proc/PID/task
13. อธิบาย R/S/D/T/Z
14. วาด process resources กับ thread-private execution state
15. อธิบาย ASLR ว่าทำไม address run ใหม่อาจเปลี่ยน

---

## Explain-It-Back

~~~text
program
→ process
→ PID
→ threads
→ CPU state
→ virtual address space
→ kernel-maintained process metadata
~~~
