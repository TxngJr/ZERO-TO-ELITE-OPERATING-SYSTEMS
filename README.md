# Zero to Elite Operating Systems — C# / Fedora Edition

หลักสูตร Operating Systems ที่ใช้ C# เป็นภาษาปฏิบัติหลัก และใช้ Fedora/Linux tools เพื่อสังเกต OS จริง

## Core Scope

1. ✅ OS Introduction
2. ✅ Process & Context Switch I
3. ✅ Process & Context Switch II
4. ✅ Concurrency I
5. ✅ Concurrency II
6. ✅ Synchronization I
7. ✅ Synchronization II
8. ✅ Synchronization III
9. ✅ Scheduling
10. ✅ Address Translation
11. ✅ Virtual Memory

---

# Start Here

1. COURSE-GUIDE.md
2. ASSIGNMENT-MAPPING.md
3. SETUP-FEDORA.md
4. STUDY-ORDER.md
5. CSHARP-OS-MAPPING.md
6. CORRECTNESS-GATES.md

---

# ตรงกับไฟล์ที่เรียนจริง

คอร์ส map เนื้อหากับ:

- Activity 02 sequential
- Activity 02 threaded + System.Threading.Lock
- Activity 03 one-reader handoff
- Activity 03 three-reader competition
- unsafe ring buffer
- thread-safe ring buffer
- Case Study local reduction

อ่าน:

- ASSIGNMENT-MAPPING.md
- SOURCE-ALIGNED-EXERCISES.md
- SOURCE-ALIGNED-ANSWERS.md

---

# Language Policy

programming examples ใช้ C#

Linux shell commands ใช้เพื่อ observation:

~~~bash
ps
strace
pmap
vmstat
chrt
taskset
~~~

บาง memory/system labs ใช้ C# P/Invoke เพื่อเรียก Linux primitives โดยตรงขึ้น เช่น:

- write
- gettid
- mmap
- munmap
- mprotect
- getrusage

---

# C# Synchronization Rule

แยกสองแบบ:

~~~text
System.Threading.Lock
→ modern mutual exclusion
→ ตรงกับ Activity 02

object + lock + Monitor
→ condition synchronization
→ ตรงกับ Activity 03 / Thread-Safe Buffer
~~~

ห้ามเหมารวมสอง mechanism นี้เป็น implementation เดียวกัน

---

# Build and Verify

~~~bash
./build.sh
./verify.sh
~~~

verify.sh ตรวจมากกว่า compile:

- expected outputs
- invariants
- scheduler golden tests
- address translation tests
- bounded concurrency termination
- page replacement results
- file COW
- mmap first-touch path
- mprotect protection child

---

# CI Environment

repository CI ใช้:

~~~text
Fedora 45
.NET 10 SDK
global.json SDK feature baseline
~~~

---

# Reviews

- MIDTERM-REVIEW-01-05.md
- BATCH-01-REVIEW.md
- BATCH-02-REVIEW.md
- BATCH-03-REVIEW.md
- BATCH-04-REVIEW.md
- FINAL-INTEGRATION.md
- FINAL-REVIEW.md
- FULL-COVERAGE-AUDIT.md

---

# End-to-End Model

~~~text
C# code
↓
.NET runtime
↓
Linux process / threads
↓
scheduler
↓
CPU
↓
synchronization
↓
virtual address
↓
TLB / page table / MMU
↓
physical memory
↓
possible page fault
↓
kernel resolution / signal
~~~

---

# Meaning of Complete

Core Chapters 01–11 และ supplied coursework alignment เป็น target scope ของ repository นี้

advanced OS topics เช่น filesystems, IPC, drivers, containers, NUMA, RCU, io_uring, virtualization และ kernel development อยู่ beyond-core track
