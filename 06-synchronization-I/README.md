# Chapter 06 — Synchronization Part I

## 1. Goals

หลังบทนี้ต้องสามารถ:

- อธิบายว่าทำไม synchronization จำเป็น
- ใช้ pthread mutex ได้อย่างถูกต้อง
- อธิบาย generic lock concept
- แยก Mutex, Semaphore และ Atomic Operation
- เข้าใจ Binary vs Counting Semaphore
- ใช้ POSIX unnamed semaphore บน Fedora
- ใช้ C11 atomic read-modify-write
- เข้าใจ Compare-And-Swap (CAS) concept
- แยก blocking จาก spinning
- วิเคราะห์ mutual exclusion, safety และ liveness ระดับพื้นฐาน
- เลือก primitive ให้เหมาะกับปัญหา ไม่ใช่เลือกเพราะ "เร็วที่สุด"

---

## 2. From Race to Synchronization

Chapter 04–05 แสดงว่า:

~~~text
shared mutable state
+
concurrent execution
+
incorrect ordering/atomicity
=
broken invariants
~~~

Synchronization คือกลไกที่สร้างข้อจำกัดต่อ:

- ใครเข้าถึง resource ได้
- เข้าเมื่อไร
- operation ใดต้องมองเป็นหน่วยเดียว
- memory effects ต้อง visible/order กันอย่างไร

---

## 3. Correctness Before Primitive

ก่อนเลือก mutex หรือ atomic ให้เขียน **Invariant**

ตัวอย่าง counter:

~~~text
final counter = number of successful increments
~~~

inventory:

~~~text
stock >= 0
and
successful_sales + stock = initial_stock
~~~

bounded resource:

~~~text
active_users <= capacity
~~~

ถ้าไม่รู้ invariant จะรู้ไม่ได้ว่า synchronization ถูกหรือยัง

---

## 4. Mutual Exclusion

Mutual Exclusion หมายถึงใน critical section ที่กำหนด มี execution participant ได้ไม่เกินหนึ่งรายในเวลาเดียวกัน

mental model:

~~~text
Thread A ----              > [ CRITICAL SECTION ] -> shared state
Thread B ----/
        only one enters at a time
~~~

mutex เป็น primitive ที่ออกแบบมาเพื่อ ownership-style mutual exclusion

---

## 5. pthread Mutex

ประกาศ:

~~~c
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;
~~~

หรือ init แบบ runtime:

~~~c
pthread_mutex_init(&mutex, NULL);
~~~

ใช้งาน:

~~~c
pthread_mutex_lock(&mutex);

/* critical section */

pthread_mutex_unlock(&mutex);
~~~

destroy เมื่อ lifecycle จบ:

~~~c
pthread_mutex_destroy(&mutex);
~~~

---

## 6. Mutex Counter

examples/mutex-counter.c ใช้ plain long counter

แต่ access ทั้งหมดที่เปลี่ยน counter อยู่ใต้ mutex:

~~~text
lock
  ↓
counter++
  ↓
unlock
~~~

run:

~~~bash
cd 06-synchronization-I
make
./bin/mutex-counter
~~~

ผลควรเท่ากับ expected ตาม algorithm contract

---

## 7. Lock Scope

critical section ใหญ่เกินไป:

~~~text
correct but may serialize too much
~~~

critical section เล็กเกินไป:

~~~text
may fail to protect invariant
~~~

หลักคือ lock ต้องครอบ **logical operation ที่ต้อง indivisible**

ไม่ใช่ครอบ "แค่บรรทัดที่เขียนตัวแปร"

ตัวอย่าง check-then-act:

~~~text
lock
check
act/update
unlock
~~~

ไม่ใช่:

~~~text
lock
check
unlock

lock
act
unlock
~~~

ถ้า invariant ต้องผูกสองขั้นเข้าด้วยกัน

---

## 8. Ownership

mutex มี ownership semantics:

- thread lock
- thread เดิมควร unlock ตาม API contract
- unlock mutex ที่ไม่ได้ถือเป็น bug/undefined behavior ตามกรณี

นี่ต่างจาก semaphore ซึ่งเป็น counter/signal primitive และไม่ใช่ ownership lock แบบเดียวกัน

---

## 9. Generic Lock Concept

คำว่า Lock เป็นคำกว้าง

ตัวอย่าง:

- mutex
- spinlock
- read-write lock
- adaptive/internal locks
- file/database locks

อย่าใช้คำว่า lock = mutex เสมอ

ในบทนี้ mutex คือ blocking mutual-exclusion primitive หลักของ user-space pthread programming

---

## 10. Blocking vs Spinning

### Blocking

ถ้า lock unavailable thread สามารถ sleep/block และให้ scheduler รันงานอื่น

เหมาะเมื่อ wait อาจนานพอที่ spinning ไม่คุ้ม

### Spinning

thread loop ตรวจ lock ต่อ:

~~~text
while lock unavailable:
    keep checking
~~~

ข้อดี: ไม่ต้อง sleep/wakeup หาก wait สั้นมาก  
ข้อเสีย: ใช้ CPU ระหว่างรอ

เลือกตาม context

อย่าสร้าง hand-written spinlock ใช้จริงจาก boolean ธรรมดา

---

## 11. Spinlock Preview

C atomic_flag สามารถเป็น building block ของ spinlock:

~~~text
test-and-set
while already set:
    spin
critical section
clear
~~~

แต่ production-quality lock ต้องคำนึงถึง:

- memory ordering
- fairness
- contention
- backoff
- preemption
- architecture

คอร์สนี้จึงไม่เสนอ naive spinlock เป็น replacement ของ pthread mutex

---

## 12. Semaphore

Semaphore เก็บ abstract count

operations มักสอนเป็น:

~~~text
wait / P / down
signal / V / up
~~~

POSIX API:

~~~c
sem_wait()
sem_post()
~~~

### Counting Semaphore

initial count = N หมายถึงอนุญาตได้สูงสุด N permits ตาม design

### Binary Semaphore

count จำกัดในลักษณะ 0/1 ใน usage บางแบบ

แต่ binary semaphore ยังไม่เท่ากับ mutex ทุก semantics เพราะ ownership/usage contract ต่างกัน

---

## 13. Semaphore Resource Limiter

สมมติ service รองรับงานพร้อมกันสูงสุด 2 งาน:

~~~text
Workers ---> [ semaphore count=2 ] ---> scarce resource
~~~

worker:

~~~text
sem_wait
enter
work
leave
sem_post
~~~

run:

~~~bash
./bin/semaphore-limit
~~~

โปรแกรมแสดง active workers และควรไม่เกิน 2 ตาม invariant

---

## 14. sem_wait and EINTR

blocking syscall/library wrapper บางตัวสามารถถูก interrupt

demo จึง retry sem_wait เมื่อ:

~~~c
errno == EINTR
~~~

นี่เป็นตัวอย่างว่าระบบจริงต้อง handle errors ไม่ใช่แค่เขียน happy path

---

## 15. Atomic Operations

C11:

~~~c
#include <stdatomic.h>
~~~

ตัวอย่าง:

~~~c
_Atomic long counter;
atomic_fetch_add(&counter, 1);
~~~

atomic_fetch_add เป็น atomic read-modify-write operation

ต่างจาก Chapter 04:

~~~text
atomic_load
compute
atomic_store
~~~

ซึ่งเป็นหลาย atomic operations แต่ compound action ไม่ atomic

---

## 16. Atomic Counter

examples/atomic-counter.c:

~~~text
N threads
   ↓
atomic_fetch_add
   ↓
one shared atomic counter
~~~

run:

~~~bash
./bin/atomic-counter
~~~

สำหรับ counter ที่ไม่ publish data อื่น เราใช้ memory_order_relaxed เพราะต้องการ atomicity ของ counter แต่ไม่ต้องสร้าง ordering กับ unrelated memory

นี่เป็น design decision ไม่ใช่คำแนะนำว่า relaxed ใช้ได้ทุกที่

---

## 17. Compare-And-Swap

CAS concept:

~~~text
if current == expected:
    current = desired
    success
else:
    expected = current
    fail
~~~

C:

~~~c
atomic_compare_exchange_strong(...)
~~~

CAS ช่วย implement state transitions แบบ conditional atomic update

---

## 18. CAS Fix for One-Item Stock

Chapter 05 broken logic:

~~~text
check stock
later decrement
~~~

Chapter 06 CAS:

~~~text
expected = 1

CAS(stock, expected=1, desired=0)

only one thread can win transition 1 -> 0
~~~

run:

~~~bash
./bin/cas-stock
~~~

หนึ่ง buyer success อีกคนควรเห็น sold out/failure ตาม race result แต่ final stock ไม่ติดลบ

---

## 19. Mutex vs Semaphore vs Atomic

| Primitive | Core idea | Ownership | Typical use |
|---|---|---|---|
| Mutex | one owner in critical section | Yes-style ownership | protect complex invariant/shared structure |
| Semaphore | permit counter | not mutex ownership semantics | limit capacity, signal resource availability |
| Atomic RMW | indivisible memory operation | no lock owner | counters, flags, state machine building blocks |

ไม่มี primitive ใด "ดีที่สุดทุกกรณี"

---

## 20. Mutex vs Atomic Counter

counter++ อย่างเดียว:

- atomic fetch_add เหมาะและ concise

complex invariant:

~~~text
if account >= amount:
    account -= amount
    log transaction
    update statistics
~~~

atomic variable ตัวเดียวอาจไม่พอ

mutex สามารถครอบ transaction ที่ประกอบด้วยหลาย objects/steps ได้ง่ายกว่าในการ reasoning

---

## 21. Safety and Liveness

### Safety

"สิ่งที่ไม่ควรเกิด ต้องไม่เกิด"

ตัวอย่าง:

~~~text
critical section มีสอง threads พร้อมกันไม่ได้
stock ห้ามติดลบ
~~~

### Liveness

"สิ่งที่ควร progress ในที่สุด ต้องมีโอกาส progress"

ตัวอย่าง:

~~~text
thread ที่รอ resource ไม่ควรถูก block ตลอดไปโดย design ที่ผิด
~~~

บท 08 จะลง:

- deadlock
- starvation
- livelock

---

## 22. Fairness

mutex/semaphore ไม่ควรถูก assume ว่าเข้าคิว FIFO เสมอ เว้นแต่ API/implementation contract ระบุ

อย่าเขียน algorithm ที่ correctness พึ่ง "thread ที่รอนานที่สุดจะได้ก่อนแน่"

fairness และ correctness เป็นคนละ property

---

## 23. Memory Synchronization

mutex ไม่ได้มีประโยชน์แค่ "กันสองคนเข้า"

locking/unlocking ตาม synchronization model ยังสร้าง ordering/visibility guarantees ที่จำเป็นต่อ shared data

นี่คือเหตุผลที่:

~~~c
volatile int locked;
~~~

ไม่สามารถแทน pthread mutex ได้

---

## 24. Common Mutex Bugs

- ลืม unlock
- return ออกจาก critical section ก่อน unlock
- unlock ผิด mutex
- double unlock
- lock order ขัดกัน
- critical section ยาวเกิน
- access shared state บางจุดไม่ใช้ mutex เดียวกัน
- free object ขณะที่อีก thread ยังใช้

บางข้อจะกลายเป็น Deadlock/Lifetime chapters ภายหลัง

---

## 25. Common Semaphore Bugs

- sem_post มากกว่าที่ design อนุญาต
- ลืม sem_post
- ใช้ count ผิด initial value
- คิดว่า semaphore มี mutex ownership semantics
- ใช้ semaphore แก้ invariant ซับซ้อนโดย reasoning ไม่ครบ

---

## 26. Common Atomic Bugs

- operation แต่ละตัว atomic แต่ transaction ไม่ atomic
- memory order อ่อนเกินจน visibility ผิด
- ABA problem ใน advanced lock-free algorithms
- lifetime/reclamation ไม่ปลอดภัย
- false sharing/contention ทำ performance แย่

atomic != easy lock-free correctness

---

## 27. Linux/Fedora Observation

ขณะ semaphore-limit ทำงาน:

~~~bash
top -H
ps -eLf | head
~~~

สำหรับ performance study ภายหลัง:

~~~bash
perf stat ./bin/mutex-counter
perf stat ./bin/atomic-counter
~~~

อย่าสรุปจาก run เดียว เพราะ scheduling/frequency/background load มี noise

---

## 28. man Pages

ฝึก:

~~~bash
man 3 pthread_mutex_lock
man 3 pthread_mutex_unlock
man 3 sem_init
man 3 sem_wait
man 3 sem_post
~~~

C atomics ให้ดู compiler/C standard library documentation เพิ่มเติม เพราะ man coverage อาจต่างตาม distribution

---

## 29. Exercises

### Recall

1. Mutual exclusion คืออะไร
2. Mutex คืออะไร
3. Semaphore คืออะไร
4. Atomic RMW คืออะไร
5. CAS คืออะไร
6. blocking ต่างจาก spinning อย่างไร

### Compare

7. Mutex vs binary semaphore
8. Semaphore vs atomic counter
9. atomic_load+store vs fetch_add
10. mutex vs spinlock

### Trace

11. mutex counter มี 4 threads; ใครเข้าคริติคัลพร้อมกันได้กี่ thread
12. semaphore initial 2 อนุญาต active ได้สูงสุดเท่าไร
13. CAS stock=1 มี buyers 2 คน ใครชนะได้กี่คน
14. atomic_fetch_add 100 ครั้งจาก 4 threads ควร final เท่าไรถ้าทุก call สำเร็จ

### Analyze

15. ทำไม lock check และ update แยกกันอาจยังผิด
16. ทำไม semaphore ไม่ใช่ mutex แม้ initial=1
17. ทำไม relaxed atomic counter ถูกใน demo แต่ไม่ใช่ default answer ทุก algorithm
18. ทำไม fairness ไม่ควรถูก assume
19. ทำไม hand-written boolean spinlock ผิด
20. ออกแบบ invariant สำหรับ connection pool capacity=10

### Design

21. เลือก primitive สำหรับ global statistics counter
22. เลือก primitive สำหรับ linked-list mutation หลายขั้น
23. เลือก primitive สำหรับอนุญาต concurrent downloads 3 งาน
24. เลือก primitive สำหรับ transition state AVAILABLE -> SOLD
25. อธิบาย trade-off correctness/performance ของแต่ละตัว

---

## 30. Quiz

1. Mutex มี ownership semantics หรือไม่
2. Semaphore เป็น permit counter หรือไม่
3. Binary semaphore เหมือน mutex ทุกอย่างหรือไม่
4. atomic_fetch_add เป็น RMW เดียวหรือไม่
5. atomic_load แล้ว atomic_store แปลว่า compound increment atomic หรือไม่
6. CAS update แบบ conditional ได้หรือไม่
7. volatile เป็น lock หรือไม่
8. spinning ใช้ CPU ระหว่างรอหรือไม่
9. blocking อาจให้ scheduler รัน thread อื่นหรือไม่
10. relaxed ordering ใช้ได้โดยไม่ต้องพิสูจน์ algorithm หรือไม่
11. mutex ช่วย memory ordering/visibility หรือไม่
12. critical section ควรเล็กที่สุดโดยไม่ทำ invariant แตกหรือไม่

### Answers

1. มี
2. ใช่
3. ไม่
4. ใช่
5. ไม่
6. ได้
7. ไม่
8. ใช่
9. ใช่
10. ไม่
11. ใช่
12. ใช่

---

## 31. Explain-It-Back

อธิบาย:

- Race Condition → Critical Section → Mutex
- capacity problem → Counting Semaphore
- simple counter → Atomic RMW
- conditional state transition → CAS

แล้วตอบคำถามสำคัญ:

~~~text
Primitive ที่เลือก protect invariant อะไร?
ทำไม scope นี้จึงพอดี?
ถ้าเอา synchronization ออก failure schedule คืออะไร?
~~~

นี่คือระดับ reasoning ที่ต้องได้ก่อนเข้าสู่ classic synchronization problems ใน Chapter 07
