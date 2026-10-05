# Course Guide — Zero to Elite Operating Systems

## เป้าหมายของ Core Course

Core Course นี้ครอบคลุม scope ที่กำหนดไว้ 11 Chapters:

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

คำว่า "ครบ" ใน repository นี้หมายถึง **ครบตาม Core Scope 1–11 ที่กำหนด** ไม่ได้หมายความว่าเนื้อหา Operating Systems ทั้งโลกสิ้นสุดที่ 11 บท

หัวข้อขั้นสูง เช่น VFS, filesystems, signals, IPC, namespaces, cgroups, device drivers, RCU, futex internals, NUMA, I/O schedulers, kernel modules และ virtualization เป็น Advanced Track ถัดไป

---

## Learning Contract

ทุก chapter ต้องตอบได้:

- What?
- Why?
- How?
- What can go wrong?
- How does Linux expose it?
- How can we observe it?
- How can we reproduce it?
- How can we debug it?

วงจรการเรียน:

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

## Recommended Prerequisites

ก่อนเริ่มควรรู้พื้นฐาน:

- C syntax เบื้องต้น
- pointer เบื้องต้น
- terminal commands
- binary/hexadecimal เบื้องต้น

แต่ chapter ต่าง ๆ จะทบทวน concept ที่จำเป็นเมื่อใช้งาน

---

## Primary Platform

~~~text
Fedora Linux
x86-64
GCC
POSIX threads
Linux /proc
strace
gdb
perf
Python 3
~~~

เครื่องตัวอย่างหลัก:

~~~text
Acer Aspire 7 A715-43G
AMD Ryzen 7 5825U
~~~

ให้ยึด output จาก Linux บนเครื่องจริงเป็น source of truth สำหรับ core/thread/page-size/runtime behavior

---

## How to Study Each Chapter

### 1. อ่าน Goals

ต้องรู้ว่าเมื่อจบบทจะตอบอะไรได้

### 2. อ่าน Mental Model

อย่าข้าม diagram

### 3. Predict Lab

เขียนคำทำนายก่อนรัน

### 4. Build

~~~bash
make
~~~

### 5. Observe

ใช้ tools ที่บทกำหนด

### 6. Modify

เปลี่ยน constants/ordering/workload แล้วทำนายใหม่

### 7. Exercises

ทำโดยไม่เปิด Answers

### 8. Explain-It-Back

ถ้าอธิบายไม่ได้ แสดงว่ายังไม่ master concept

---

## Exam Boundary

Midterm โดยประมาณ:

~~~text
Chapter 01–05
~~~

ดู:

- MIDTERM-REVIEW-01-05.md

หลัง Chapter 11:

- FINAL-REVIEW.md
- FINAL-INTEGRATION.md
- capstone/README.md

---

## Build Whole Course

จาก root:

~~~bash
make core-build
make python-check
make smoke
make clean
~~~

---

## Mastery Standard

ไม่ถือว่า master เพียงเพราะ:

- อ่านจบ
- code compile
- quiz ถูก

ต้องทำได้ทั้ง:

~~~text
Explain
Predict
Trace
Code
Observe
Debug
Calculate
Design
~~~

---

## What Comes Next

หลัง Core 1–11 สามารถต่อ Advanced Track:

~~~text
Signals
IPC
Pipes
Shared Memory
Sockets
Filesystems / VFS
I/O
Block Layer
Device Drivers
Kernel Modules
Boot
systemd internals
Containers
Namespaces
cgroups
NUMA
Huge Pages
SLUB
Page Cache
Zero-Copy
io_uring
Real-Time Scheduling
CPU Affinity
Memory Ordering
Futex
Lock-Free
RCU
Security
SELinux
Capabilities
Virtualization
Kernel Compilation
Kernel Debugging
OS Development
~~~
