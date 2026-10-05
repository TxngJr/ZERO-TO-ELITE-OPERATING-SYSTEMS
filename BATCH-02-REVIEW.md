# Batch 02 Review — Chapters 04–06

## Scope

- Chapter 04 — Concurrency Part I
- Chapter 05 — Concurrency Part II
- Midterm Review — Chapters 01–05
- Chapter 06 — Synchronization Part I

---

## 1. Dependency Audit

~~~text
Process/Context
    ↓
Thread
    ↓
Concurrency
    ↓
Interleaving
    ↓
Race Condition
    ↓
Critical Section / Invariant
    ↓
Synchronization
    ↓
Mutex / Semaphore / Atomic / CAS
~~~

ไม่มี primitive ถูกนำมาก่อนปัญหาที่มันแก้

---

## 2. Coverage Matrix

| Topic | Theory | Diagram/Trace | Code | Fedora Lab | Exercises | Quiz |
|---|---:|---:|---:|---:|---:|---:|
| Thread | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Concurrency vs Parallelism | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Interleaving | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Race Condition | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Critical Section | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Data Race | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Shared Resources | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Thread Safety | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Reentrancy | ✅ | ✅ | — | ✅ | ✅ | ✅ |
| Memory Visibility | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Detached Threads | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Mutex | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Semaphore | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Atomic RMW | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Compare-And-Swap | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Spin vs Block | ✅ | ✅ | — | ✅ | ✅ | ✅ |
| Safety/Liveness intro | ✅ | ✅ | — | ✅ | ✅ | ✅ |

---

## 3. Correctness Audit

### C data race

Course ไม่ใช้ plain unsynchronized shared increment เป็น "defined lost-update demo"

Chapter 04 ใช้ _Atomic load/store แยกกันเพื่อให้:

- individual accesses defined
- compound increment intentionally non-atomic
- race condition observable โดยไม่อาศัย undefined behavior

### Check–Then–Act

Chapter 05 ใช้ atomic stock และ barrier เพื่อแสดง logical race ที่ deterministic มากขึ้น

### Visibility

release/acquire example สร้าง ordering ที่ถูกต้องก่อน reader อ่าน non-atomic payload

### Mutex

plain shared counter ถูก access ภายใต้ mutex

### Semaphore

permit count จำกัด active workers; sem_wait handles EINTR

### Atomics

relaxed ordering ถูกใช้เฉพาะตัวอย่างที่ไม่ต้อง publish unrelated shared data

---

## 4. Common Mistake Gate

ต้องแก้คำกล่าวเหล่านี้ได้:

- More threads = faster
- Atomic variable = atomic algorithm
- Volatile = synchronization
- Semaphore(1) = mutex ทุก semantics
- No data race = no race condition
- Race not reproduced = race fixed
- Detached thread survives process
- Source line = atomic operation
- Fair scheduling = guaranteed FIFO
- Spinlock = bool variable loop

---

## 5. Practical Gate

build:

~~~bash
cd 04-concurrency-I && make clean && make
cd ../05-concurrency-II && make clean && make
cd ../06-synchronization-I && make clean && make
~~~

ต้องสามารถอธิบาย output ของ:

~~~text
thread-basic
lost-update
thread-observe
check-then-act
release-acquire
detached-demo
mutex-counter
atomic-counter
semaphore-limit
cas-stock
~~~

โดยแยก:

- guaranteed
- timing-dependent
- invariant
- synchronization guarantee

---

## 6. Explain-It-Back Gate

อธิบาย end-to-end:

~~~text
Multiple threads
      ↓
shared state
      ↓
interleaving
      ↓
race breaks invariant
      ↓
identify critical operation
      ↓
choose primitive
      ↓
establish atomicity/ordering
      ↓
verify invariant
~~~

ถ้าทำได้ พร้อม Chapter 07 classic synchronization problems
