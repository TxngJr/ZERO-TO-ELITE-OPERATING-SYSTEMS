# Zero to Elite Operating Systems

หลักสูตร Operating Systems แบบลงมือทำบน **Fedora Linux / x86-64** สำหรับเส้นทางจาก Absolute Zero ไปสู่ระดับ Systems Engineer

> Target machine: Acer Aspire 7 A715-43G, AMD Ryzen 7 5825U, x86-64, Fedora Linux

## Learning model

ทุกบทใช้วงจร:

```text
Why → Concept → Mental Model → Linux → Code → Run → Observe → Debug → Explain
```

เราไม่ได้เรียนเพื่อจำ definition แต่ต้องพิสูจน์แนวคิดด้วย Linux tools และ C programs จริง

## Core roadmap

1. **Course Overview & OS Introduction**
2. **Process & Context Switch Part I**
3. **Process & Context Switch Part II**
4. Concurrency Part I
5. Concurrency Part II
6. Synchronization Part I
7. Synchronization Part II
8. Synchronization Part III
9. Scheduling
10. Address Translation
11. Virtual Memory

> Midterm scope โดยประมาณ: Chapter 01–05

## Batch 1 — available now

- [Chapter 01 — Course Overview & OS Introduction](./01-os-introduction/README.md)
- [Chapter 02 — Process & Context Switch Part I](./02-process-context-I/README.md)
- [Chapter 03 — Process & Context Switch Part II](./03-process-context-II/README.md)
- [Fedora Setup](./SETUP-FEDORA.md)
- [Batch 1 Review](./BATCH-01-REVIEW.md)

## Study order

```text
SETUP-FEDORA.md
      ↓
Chapter 01
      ↓
Chapter 02
      ↓
Chapter 03
      ↓
BATCH-01-REVIEW.md
```

อย่าเพิ่งข้ามไป concurrency ถ้ายังอธิบายไม่ได้ว่า:

- OS ต่างจาก Kernel อย่างไร
- User Space ต่างจาก Kernel Space อย่างไร
- System Call คืออะไร
- Program ต่างจาก Process อย่างไร
- PID / PPID คืออะไร
- CPU state ที่ต้องรักษามีอะไรบ้าง
- Context Switch คืออะไร
- `fork()`, `exec*()`, `wait*()` ทำหน้าที่ต่างกันอย่างไร
- Zombie process เกิดจากอะไร

## Build philosophy

C examples ใช้ warning flags:

```bash
gcc -Wall -Wextra -Wpedantic -std=c17 file.c -o program
```

ตัวอย่างที่ใช้ POSIX extensions อาจเพิ่ม feature-test macro หรือ compiler option ตามที่ source ระบุ

## Repository status

Batch 1 ครอบคลุม Chapters 01–03 พร้อม theory, diagrams, Fedora labs, C programs, exercises และ review checkpoint

ถัดไป: Chapters 04–06 + Midterm Review 1–5
