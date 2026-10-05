# Course Guide — Zero to Elite Operating Systems with C#

## Scope

หลักสูตรนี้ตั้งใจให้ครบตาม Core Scope 01–11:

1. OS Introduction
2. Process & Context Switch I
3. Process & Context Switch II
4. Concurrency I
5. Concurrency II
6. Synchronization I
7. Synchronization II
8. Synchronization III
9. Scheduling
10. Address Translation
11. Virtual Memory

คำว่า “ครบ” ใน repo นี้หมายถึงครบตาม core scope นี้และตรงกับไฟล์ C# ที่เรียนที่ผู้ใช้ส่งมา

ไม่ได้หมายความว่า field Operating Systems ทั้งหมดมีเพียง 11 บท

---

# Source Alignment

เริ่มจาก:

- ASSIGNMENT-MAPPING.md
- SOURCE-ALIGNED-EXERCISES.md
- SOURCE-ALIGNED-ANSWERS.md

ไฟล์เหล่านี้ map:

~~~text
Activity 02-1
Activity 02-2
Activity 03-1
Activity 03-2
Unsafe Buffer
Thread-Safe Buffer
Case Study 02
~~~

เข้ากับ chapters ที่เกี่ยวข้อง

หลักสำคัญ:

ถ้า source ที่เรียนไม่ได้บอกบางอย่าง เช่น implementation ภายใน external CalculatingFunctions คอร์สจะระบุว่าไม่ทราบจาก source แทนการเดา

---

# Teaching Model

ทุกบทใช้:

~~~text
Why
→ Concept
→ OS Mental Model
→ C#/.NET Mapping
→ Broken Example
→ Correct Example
→ Fedora Observation
→ Exercise
→ Self-Test / Invariant
→ Explain-It-Back
~~~

---

# C# Rule

source-code programming labs ใช้ C#

Linux commands ยังคงใช้สำหรับ OS observation

ตัวอย่าง:

~~~bash
ps
strace
pmap
vmstat
chrt
taskset
~~~

P/Invoke ยังถือว่า source หลักเป็น C# และใช้เฉพาะจุดที่ต้องเห็น Linux primitive ตรงขึ้น เช่น:

- gettid
- write
- mmap
- munmap
- mprotect
- getrusage

---

# Critical C# Synchronization Rule

คอร์สแยก:

~~~text
System.Threading.Lock
→ modern mutual exclusion

object + lock + Monitor
→ monitor condition synchronization
~~~

Activity 02 ใช้ System.Threading.Lock

Activity 03 และ Thread-Safe Buffer ใช้ object + Monitor

ห้ามสอนว่าเป็น implementation เดียวกันทุกประการ

---

# Study Loop

ทุก lab:

~~~text
Predict
→ Run
→ Observe
→ Compare
→ Explain
→ Modify
→ Re-run
~~~

---

# Mastery Gate

ไม่ถือว่าผ่านบทเพราะ:

- อ่านจบ
- compile ผ่าน
- copy code ได้

ต้องทำได้:

~~~text
Explain
Trace
Predict
Code
Find Bug
Fix Bug
Observe Linux
Calculate
Design
~~~

---

# Correctness

อ่าน:

- CORRECTNESS-GATES.md

CI ต้องผ่าน:

~~~text
compile
+
timeout/termination
+
invariant assertions
+
golden algorithm tests
~~~

ไม่ใช้เพียง exit code 0

---

# Midterm

ประมาณ Chapters 01–05

อ่าน:

- MIDTERM-REVIEW-01-05.md
- SOURCE-ALIGNED-EXERCISES.md Parts A–C

---

# Final

อ่าน:

- FINAL-INTEGRATION.md
- FINAL-REVIEW.md
- BATCH-04-REVIEW.md
- capstone/README.md

---

# Build

~~~bash
./build.sh
./verify.sh
~~~

ทั้งสองต้องผ่านก่อนถือว่า repository state พร้อมเรียน

---

# Beyond Core

หัวข้อถัดไป:

- Signals
- Pipes
- IPC
- Shared Memory
- Sockets
- Filesystems / VFS
- Page Cache
- Block I/O
- Device Drivers
- Kernel Modules
- Namespaces / cgroups
- Containers
- NUMA
- Futex internals
- RCU
- Lock-Free programming
- io_uring
- Real-Time Linux
- Security / SELinux / Capabilities
- Virtualization
- Kernel debugging
- Small OS development
