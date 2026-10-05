# Chapter 05 — Concurrency Part II

## 1. Goals

หลังบทนี้ต้องสามารถ:

- อธิบาย multithreading lifecycle ได้ลึกขึ้น
- แยก joinable และ detached thread ได้
- เข้าใจ shared mutable state
- วิเคราะห์ check-then-act, read-modify-write และ ordering races
- แยก Race Condition กับ Data Race
- อธิบาย Thread Safety และ Reentrancy ระดับใช้งานได้
- เข้าใจเหตุผลที่ memory visibility/order ต้องมี synchronization
- เข้าใจ happens-before ระดับ introductory
- ใช้ C atomics เป็นเครื่องมือสร้างตัวอย่างที่ defined behavior
- เตรียมพร้อมเข้าสู่ Mutex, Semaphore และ Atomic RMW ใน Chapter 06

---

## 2. From "Many Threads" to "Correct Many Threads"

การสร้าง threads ได้ไม่เท่ากับเขียน concurrent program ถูกต้อง

ปัญหาหลักคือ:

~~~text
shared mutable state
+
multiple execution flows
+
insufficient synchronization
=
timing-dependent correctness
~~~

ตัวอย่าง shared resource:

- counter
- bank balance
- inventory
- queue
- linked list
- file offset/state
- socket state
- cache
- log buffer
- application configuration

---

## 3. Multithreading Lifecycle

joinable thread:

~~~text
create
  ↓
runs
  ↓
returns / pthread_exit
  ↓
terminated but join state retained
  ↓
pthread_join
  ↓
resources associated with join state released
~~~

detached thread:

~~~text
create
  ↓
detach
  ↓
runs
  ↓
terminates
  ↓
implementation can reclaim join-related resources automatically
~~~

detached thread ไม่สามารถ pthread_join แบบปกติภายหลังได้

---

## 4. Joinable vs Detached

### Joinable เหมาะเมื่อ

- ต้องรอผล
- ต้องรู้ว่า worker จบแล้ว
- ต้อง synchronize phase
- ต้องรับ return pointer/status

### Detached เหมาะเมื่อ

- fire-and-forget work ที่ lifecycle ถูกออกแบบชัดเจน
- ไม่มีใครต้อง join
- resource ownership ถูกจัดการถูกต้อง

detached ไม่ได้แปลว่า thread จะอยู่หลัง process จบ

เมื่อ process จบ threads ทั้ง process ก็สิ้นสุดด้วย

---

## 5. Shared Mutable State

อ่านประโยคนี้ให้แม่น:

~~~text
Sharing is not automatically wrong.
Mutation is not automatically wrong.
Concurrent unsynchronized access with incompatible operations is the danger.
~~~

ถ้าข้อมูล immutable หลัง initialization การอ่านพร้อมกันมัก reasoning ง่ายกว่า

---

## 6. Classic Race Pattern: Read–Modify–Write

logical operation:

~~~text
x = x + 1
~~~

อาจต้องคิดเป็น:

~~~text
read x
compute new value
write x
~~~

หาก operation ทั้งชุดต้อง indivisible แต่ synchronization ครอบไม่ครบ จะเกิด lost update หรือ invariant failure

---

## 7. Classic Race Pattern: Check–Then–Act

ตัวอย่าง inventory:

~~~text
if stock > 0:
    stock = stock - 1
    sell()
~~~

ถ้า 2 threads เห็น stock=1 พร้อมกัน:

~~~text
T1 check -> 1
T2 check -> 1
T1 act
T2 act
~~~

ทั้งคู่คิดว่าซื้อได้

นี่คือ TOCTOU-style logical pattern ในระดับ concept: time of check กับ time of use แยกกัน

---

## 8. Defined Check–Then–Act Demo

examples/check-then-act.c ใช้ atomic integer เพื่อป้องกัน C data race แต่จงใจแยก:

~~~text
atomic load
barrier
atomic decrement
~~~

สอง threads จึงเห็น stock=1 ก่อนทั้งคู่ แล้วต่างคนต่าง decrement

result ที่ตั้งใจ:

~~~text
initial stock=1
two buyers both saw 1
final stock=-1
~~~

นี่แสดงว่า:

~~~text
atomic variables ≠ automatically atomic business transaction
~~~

---

## 9. Data Race vs Race Condition

คำสองคำเกี่ยวกันแต่ไม่เหมือนกัน

### Race Condition

กว้างกว่า: correctness ขึ้นกับ timing/order ของ concurrent events

### Data Race ใน C memory model

โดยย่อ: conflicting accesses ต่อ memory location เดียวกันจากหลาย threads โดยไม่มี ordering/synchronization ที่มาตรฐานกำหนด และอย่างน้อยหนึ่งเป็น write

C data race ทำให้ undefined behavior

ดังนั้น:

~~~text
Data race ⇒ serious race problem
Race condition ⇏ necessarily C data race
~~~

ตัวอย่าง check-then-act.c ใช้ atomic accesses จึงสามารถมี logical race condition โดยไม่ต้องมี data race

---

## 10. Thread Safety

component/function เป็น thread-safe เมื่อสามารถถูกใช้ตาม documented contract จากหลาย threads พร้อมกันโดยไม่ทำให้ correctness พัง

thread safety อาจเกิดจาก:

- immutable state
- internal locks
- atomics
- thread-local state
- ownership transfer
- message passing
- API contract ที่จำกัด concurrent use

อย่าดูเพียงว่า "ไม่มี global variable"

---

## 11. Reentrancy

Reentrant function โดยแนวคิดสามารถถูก interrupt/called again ก่อน invocation เดิมเสร็จโดยไม่พัง state

thread-safe และ reentrant มีส่วนทับซ้อน แต่ไม่เท่ากัน

ตัวอย่าง concept:

- function ที่ใช้ shared mutable static buffer อาจไม่ reentrant
- function ที่ lock global mutex อาจ thread-safe แต่ถ้าเรียกซ้อนใน context ที่ lock เดิมแบบ non-recursive อาจมีปัญหา

รายละเอียด exact classification ต้องอิง API contract

---

## 12. Shared Library/API State

ฟังก์ชัน library บางตัวคืน pointer ไป internal/static storage หรือมี thread-local variants

หลักปฏิบัติ:

1. อ่าน man page
2. ดู thread-safety attributes เมื่อ documentation มี
3. อย่าเดาจากชื่อ function

บน Linux man pages บางส่วนมี ATTRIBUTES section เช่น MT-Safe

---

## 13. Ordering Problem

บางครั้งปัญหาไม่ใช่ "สองคนเขียนตัวเดียวกัน" แต่คือ order

สมมติ:

~~~text
Thread A:
data = 42
ready = true

Thread B:
while (!ready) {}
print(data)
~~~

ถ้าใช้ plain C variables โดยไม่มี synchronization นี่ไม่ใช่ concurrent program ที่ถูกต้องตาม C memory model

compiler/CPU ไม่ได้มี obligation ให้ model นี้ทำงานตาม intuition

---

## 14. Memory Visibility

CPU cores มี caches, store buffers และ hardware memory-ordering rules

compiler ก็ทำ optimization/reordering ได้ภายใน language rules

ดังนั้น mental model:

~~~text
"Thread A เขียนบรรทัดก่อน Thread B ต้องเห็นทันที"
~~~

ไม่ใช่ synchronization rule

เราต้องใช้ primitives ที่สร้าง ordering/visibility guarantees

---

## 15. Happens-Before — Intro

happens-before เป็น relation ที่ช่วยตอบว่า access หนึ่งถูก ordered ก่อนอีก access ตาม language synchronization model หรือไม่

ตัวอย่าง release/acquire:

~~~text
Writer:
write ordinary data
atomic store-release ready=true

Reader:
atomic load-acquire ready
then read ordinary data
~~~

เมื่อ reader acquire อ่านค่าจาก matching release chain ที่เหมาะสม writes ก่อน release สามารถถูก ordered/visible ตาม C memory model

examples/release-acquire.c สาธิต pattern นี้

---

## 16. Memory Order Preview

C atomics มี memory orders เช่น:

- memory_order_relaxed
- memory_order_acquire
- memory_order_release
- memory_order_acq_rel
- memory_order_seq_cst

Chapter 06 จะใช้ relaxed สำหรับ counter ที่ไม่ต้อง publish data และอธิบาย atomic RMW

อย่าเลือก relaxed เพราะ "เร็วกว่า" โดยไม่พิสูจน์ correctness

---

## 17. Detached Thread Demo

build:

~~~bash
cd 05-concurrency-II
make
./bin/detached-demo
~~~

โปรแกรมจะ detach worker แล้ว main รอด้วย condition ที่เป็น atomic completion flag เพื่อไม่ให้ process exit ก่อนสังเกต worker

นี่เป็น demo เพื่อการเรียน ไม่ใช่ pattern ที่ดีที่สุดสำหรับทุก application

---

## 18. Ownership

concurrent design ที่ดีควรถามว่า:

~~~text
ใครเป็นเจ้าของ object นี้?
ใครแก้ไขได้?
ใครต้อง free?
lifetime สิ้นสุดเมื่อไร?
มีใครถือ pointer หลัง free หรือไม่?
~~~

หลาย concurrency bugs เป็นทั้ง synchronization + lifetime bugs เช่น use-after-free

---

## 19. Shared Resources Beyond Memory

race เกิดกับ resource อื่นได้:

### File

หลาย threads เขียน/seek shared file state

### Queue

producer/consumer access head/tail พร้อมกัน

### Socket

หลาย execution flows จัด message framing ไม่ตรงกัน

### Database

check-then-update โดยไม่มี transaction isolation

หลัก reasoning เดียวกัน:

~~~text
What invariant must remain true?
Which operations must appear indivisible?
Who owns synchronization?
~~~

---

## 20. Interleaving Explosion

มี 2 threads แต่ละ thread มี 3 interesting operations จำนวน possible interleavings เพิ่มเร็วมาก

จึงไม่สามารถ test ทุก schedule ด้วยการ "รันหลายรอบ" อย่างเดียว

เราต้องใช้:

- synchronization reasoning
- invariants
- race detection tools
- stress testing
- structured design

---

## 21. Tool Preview: ThreadSanitizer

GCC สามารถรองรับ ThreadSanitizer ในบาง environment:

~~~bash
gcc -fsanitize=thread -g -O1 program.c -o program -pthread
~~~

ใช้ตรวจ data races บางประเภท

แต่:

- availability/support ขึ้นกับ toolchain/architecture
- sanitizer ไม่พิสูจน์ว่าไม่มี race ทุกชนิด
- logical race บน atomic operations เช่น check-then-act อาจไม่ถูก report เป็น data race

จึงต้อง reasoning เองด้วย

---

## 22. Common Misconceptions

### "ใช้ atomic variable แล้ว algorithm thread-safe"

ผิด ถ้า transaction/invariant ครอบหลาย operations

### "ไม่มี data race = ไม่มี race condition"

ผิด

### "volatile แก้ threads"

ผิดใน C; volatile ไม่ใช่ synchronization primitive สำหรับ inter-thread ordering

### "ถ้า x86 ดูเหมือนทำงาน ก็ portable C ถูกต้อง"

ผิด language memory model กับ hardware observation เป็นคนละ layer

### "detached thread อยู่ต่อหลัง process exit"

ผิด

### "test ผ่าน 10,000 ครั้ง = proof"

ผิด

---

## 23. Performance Notes

contention ทำให้:

- cores แย่ง shared cache lines
- locks serialize execution
- atomic RMW มี cache-coherence cost
- scheduler อาจ block/wake threads

แต่การเอา lock ออกเพื่อ "เร็วขึ้น" โดย correctness พังคือ optimization ที่ใช้ไม่ได้

ลำดับที่ถูก:

~~~text
Correctness
→ Measure
→ Find bottleneck
→ Optimize
→ Re-verify correctness
~~~

---

## 24. Exercises

### Concepts

1. joinable vs detached ต่างกันอย่างไร
2. shared mutable state คืออะไร
3. read-modify-write race คืออะไร
4. check-then-act race คืออะไร
5. data race ต่างจาก race condition อย่างไร
6. thread safety คืออะไร
7. reentrancy คืออะไร
8. happens-before มีประโยชน์อะไร
9. volatile ใช้แทน mutex/atomic ได้หรือไม่
10. ownership เกี่ยวกับ concurrency อย่างไร

### Trace

11. stock=1, T1 check, T2 check, T1 decrement, T2 decrement — ผลอะไร
12. เขียน schedule ที่ final counter ถูกและผิด
13. detached thread ยังไม่จบแต่ main return จะเกิดอะไรระดับ process
14. release/acquire demo publish data อย่างไร
15. ถ้า reader อ่าน ready ก่อน writer publish ต้องทำอะไรตาม algorithm

### Analyze

16. atomic_load + atomic_store แยกกันทำไมไม่เท่ากับ atomic increment
17. atomic stock ยัง oversell ได้อย่างไร
18. sanitizer ไม่เจอ race พิสูจน์ correctness ได้หรือไม่
19. database stock decrement คล้าย in-memory check-then-act อย่างไร
20. ออกแบบ ownership rule สำหรับ work queue

---

## 25. Quiz

1. Detached thread join ได้ตามปกติหรือไม่
2. Data race ใน C มี undefined behavior หรือไม่
3. Race condition สามารถเกิดโดยไม่มี data race หรือไม่
4. atomic variable รับประกัน transaction หลายขั้น atomic หรือไม่
5. volatile เป็น inter-thread synchronization หรือไม่
6. release/acquire ใช้สร้าง ordering relation ได้หรือไม่
7. immutable shared data ช่วยลด synchronization burden หรือไม่
8. thread-safe เท่ากับ reentrant ทุกกรณีหรือไม่
9. process exit แล้ว detached thread อยู่ต่อหรือไม่
10. test จำนวนมากพิสูจน์ absence of concurrency bug หรือไม่

### Answers

1. ไม่
2. ใช่
3. ได้
4. ไม่
5. ไม่
6. ใช่
7. ใช่
8. ไม่
9. ไม่
10. ไม่

---

## 26. Explain-It-Back

อธิบายโดยไม่เปิดโน้ต:

- ทำไม atomics ยังมี race condition ได้
- ทำไม volatile ไม่ใช่คำตอบ
- check-then-act ต่างจาก atomic transaction อย่างไร
- happens-before ช่วยให้เราคิดเรื่อง visibility อย่างไร
- ทำไม synchronization primitives ใน Chapter 06 จำเป็น
