# Chapter 07 — Synchronization Part II

## 1. Goals

หลังบทนี้ต้องสามารถ:

- แก้ Producer–Consumer แบบ bounded buffer
- อธิบายบทบาทของ mutex, condition variable และ semaphore ในปัญหาเดียวกัน
- วิเคราะห์ Readers–Writers และ trade-off ด้าน fairness
- ใช้ pthread_rwlock_t ใน Linux/POSIX
- วิเคราะห์ Dining Philosophers
- หา circular-wait pattern
- ออกแบบวิธีป้องกัน deadlock อย่างน้อย 3 แนวคิด
- แยก correctness, throughput และ fairness ออกจากกัน

---

## 2. ทำไมต้องเรียน Classic Synchronization Problems

ปัญหาเหล่านี้ไม่ใช่แค่โจทย์เก่าในตำรา

มันเป็นแบบจำลองของระบบจริง:

| Classic Problem | ระบบจริง |
|---|---|
| Producer–Consumer | queue, logging, network pipeline, job worker |
| Readers–Writers | cache, config, metadata, database-like access |
| Dining Philosophers | หลาย resources ที่ต้องถือพร้อมกัน |

เป้าหมายคือฝึกถามว่า:

1. shared state คืออะไร
2. invariant คืออะไร
3. condition ที่ต้องรอคืออะไร
4. critical section อยู่ตรงไหน
5. deadlock/starvation เกิดได้อย่างไร
6. primitive ใดเหมาะ

---

# Part I — Producer–Consumer

## 3. Problem Model

มี buffer ขนาดจำกัด:

~~~text
Producer(s)
    |
    v
+-------------------+
| bounded buffer    |
| capacity = N      |
+-------------------+
    |
    v
Consumer(s)
~~~

Producer:

- ใส่ item
- ถ้าเต็มต้องรอ

Consumer:

- เอา item
- ถ้าว่างต้องรอ

---

## 4. Shared State

ring buffer แบบพื้นฐาน:

~~~text
buffer[N]
head
tail
count
~~~

invariants:

~~~text
0 <= count <= N
head/tail อยู่ในช่วง 0..N-1
item ไม่ถูก consume ก่อน produce
แต่ละ produced item ถูก consume ตาม design ที่กำหนด
~~~

---

## 5. Broken Design

แบบผิด:

~~~text
if count < N:
    buffer[tail] = item
    tail = ...
    count++
~~~

ถ้ามีหลาย producers:

- check พร้อมกัน
- write slot เดียวกัน
- update count ผิด

consumer ก็มีปัญหาแบบเดียวกัน

---

## 6. Mutex + Condition Variables

pattern ที่ถูก:

~~~text
lock mutex

while buffer full:
    wait(not_full, mutex)

insert item

signal(not_empty)
unlock mutex
~~~

consumer:

~~~text
lock mutex

while buffer empty:
    wait(not_empty, mutex)

remove item

signal(not_full)
unlock mutex
~~~

condition variable ไม่ได้เก็บ "จำนวน events" แบบ semaphore

มันใช้เพื่อรอ predicate บน shared state ภายใต้ mutex

---

## 7. ทำไมต้องใช้ while ไม่ใช่ if

เขียน:

~~~c
while (count == CAPACITY) {
    pthread_cond_wait(&not_full, &mutex);
}
~~~

ไม่ใช่:

~~~c
if (count == CAPACITY) {
    pthread_cond_wait(...);
}
~~~

เพราะ:

- wakeup ไม่ได้แปลว่า predicate ต้อง true
- thread อื่นอาจแย่ง resource ก่อนเรา reacquire mutex
- spurious wakeups เป็นสิ่งที่ API อนุญาต

หลักคือ:

~~~text
Wakeup = "ตรวจ condition อีกครั้ง"
ไม่ใช่ = "condition สำเร็จแน่นอน"
~~~

---

## 8. pthread_cond_wait ทำอะไร

conceptually:

~~~text
caller holds mutex
      |
      | pthread_cond_wait
      v
release mutex + sleep atomically with respect to wait protocol
      |
   wakeup
      |
reacquire mutex
      |
return to caller
~~~

ดังนั้นเมื่อฟังก์ชัน return caller ถือ mutex อีกครั้ง

---

## 9. Producer–Consumer Lab

ไฟล์:

~~~text
examples/producer-consumer.c
~~~

ใช้:

- pthread_mutex_t
- pthread_cond_t not_empty
- pthread_cond_t not_full
- ring buffer

build:

~~~bash
cd 07-synchronization-II
make
./bin/producer-consumer
~~~

สังเกต:

- count ไม่ต่ำกว่า 0
- count ไม่เกิน capacity
- producers/consumers interleave ได้

---

## 10. Semaphore Alternative

bounded buffer สามารถ model ด้วย:

~~~text
empty_slots = N
filled_slots = 0
mutex = protect buffer metadata
~~~

producer:

~~~text
wait(empty_slots)
lock(mutex)
insert
unlock(mutex)
post(filled_slots)
~~~

consumer:

~~~text
wait(filled_slots)
lock(mutex)
remove
unlock(mutex)
post(empty_slots)
~~~

นี่แสดงว่า semaphore เหมาะกับ resource counts

---

# Part II — Readers–Writers

## 11. Problem Model

resource หนึ่ง:

- readers หลายคนอ่านพร้อมกันได้
- writer ต้อง exclusive

~~~text
Reader A ----Reader B -----+--> shared object
Reader C ----/
Writer ------- exclusive
~~~

invariant:

~~~text
ถ้ามี writer active:
    readers active = 0
    writers active = 1

ถ้ามี readers active:
    writer active = 0
~~~

---

## 12. Why Mutex Alone Is Correct but May Serialize Too Much

ถ้าใช้ mutex ธรรมดา:

~~~text
reader 1 lock
reader 2 wait
reader 3 wait
~~~

แม้ทุก reader เพียงอ่าน

correct ได้ แต่เสีย concurrency

read-write lock อนุญาตหลาย readers พร้อมกัน

---

## 13. POSIX Read–Write Lock

API:

~~~c
pthread_rwlock_rdlock(...)
pthread_rwlock_wrlock(...)
pthread_rwlock_unlock(...)
~~~

read lock:

- compatible กับ read locks อื่นตาม semantics

write lock:

- exclusive

run:

~~~bash
./bin/readers-writers
~~~

---

## 14. Fairness Problem

Readers–Writers มีหลาย policy:

### Reader preference

reader ใหม่เข้าได้ง่าย  
ข้อเสีย: writer อาจ starvation

### Writer preference

เมื่อ writer รอ อาจกัน readers ใหม่  
ข้อเสีย: reader latency เพิ่ม

### Fair / phase-based approaches

พยายาม balance ordering

POSIX rwlock implementation/fairness details ไม่ควรถูก assume เป็น FIFO เว้นแต่ documentation ระบุ

---

## 15. Thread Safety Is Not Fairness

ระบบอาจ:

- ไม่มี data race
- invariant ถูก
- แต่ thread บางตัวรอนานมาก

ดังนั้น correctness มีหลายมิติ:

~~~text
Safety
Liveness
Fairness
Performance
~~~

---

# Part III — Dining Philosophers

## 16. Problem Model

philosophers นั่งเป็นวง

แต่ละคนต้องถือ fork ซ้ายและขวาจึงจะกินได้

~~~text
P0 -- F0 -- P1 -- F1 -- P2 -- ... -- P0
~~~

simplified mapping:

- philosopher = thread
- fork = mutex/resource
- eating = critical work needing 2 resources

---

## 17. Naive Algorithm

ทุกคน:

~~~text
lock left
lock right
eat
unlock right
unlock left
~~~

ถ้าทุกคน lock left พร้อมกัน:

~~~text
P0 holds F0, waits F1
P1 holds F1, waits F2
...
Pn holds Fn, waits F0
~~~

เกิด circular wait

---

## 18. Deadlock Prevention by Resource Ordering

กำหนด global order ให้ resources:

~~~text
always lock lower-numbered fork first
then higher-numbered fork
~~~

ถ้าทุก thread ทำตาม order เดียวกัน circular wait ถูกทำลาย

ไฟล์:

~~~text
examples/dining-philosophers.c
~~~

ใช้ resource ordering เพื่อหลีกเลี่ยง deadlock

---

## 19. Other Solutions

### Arbitrator / waiter

ต้องขอ permission ก่อนหยิบ forks

### Limit concurrency

อนุญาต philosophers เข้าพื้นที่พยายามกินไม่เกิน N-1

### Asymmetric acquisition

บางคนหยิบซ้ายก่อน บางคนขวาก่อน

### trylock + backoff

ปล่อย resource ถ้าหาอีกอันไม่ได้

แต่ถ้าทุกคนทำ lockstep อาจเกิด livelock ได้

---

## 20. Deadlock-Free Does Not Automatically Mean Starvation-Free

resource ordering ช่วยตัด circular wait

แต่ scheduler/fairness/lock acquisition อาจยังทำให้ philosopher บางคนรอนาน

จึงต้องแยก:

~~~text
No deadlock
≠
Guaranteed fairness
~~~

---

## 21. Common Misconceptions

### "Condition variable คือ event counter"

ไม่ใช่

condition variable ใช้คู่ predicate + mutex

### "signal หมายถึง condition ต้อง true เมื่อ waiter รัน"

ไม่รับประกัน ต้อง recheck predicate

### "rwlock เร็วกว่า mutex เสมอ"

ไม่จริง ขึ้นกับ workload/contention/implementation

### "Dining Philosophers มีไว้จำชื่อ"

ไม่ใช่ มันฝึก multi-resource acquisition/deadlock reasoning

### "deadlock-free = starvation-free"

ไม่จริง

---

## 22. Performance Notes

Producer–Consumer:

- buffer ใหญ่ขึ้นอาจลด blocking แต่เพิ่ม latency/memory
- lock contention ขึ้นกับ producer/consumer counts
- condition variables ช่วยหลีกเลี่ยง busy-wait

Readers–Writers:

- rwlock มี overhead
- ถ้า writes บ่อย mutex อาจง่ายและเร็วกว่า
- read-heavy workload จึงมีโอกาสได้ประโยชน์

Dining Philosophers:

- coarse global lock แก้ deadlockง่ายแต่ลด parallelism
- ordered fine-grained locks รักษา concurrency มากกว่า

---

## 23. Exercises

1. bounded buffer invariant คืออะไร
2. ทำไม producer ต้องรอเมื่อ full
3. ทำไม consumer ต้องรอเมื่อ empty
4. condition variable ต่างจาก semaphore อย่างไร
5. ทำไม pthread_cond_wait ต้องใช้ mutex
6. ทำไมต้อง while รอบ wait
7. readers หลายคนอ่านพร้อมกันได้เพราะอะไร
8. writer ทำไมต้อง exclusive
9. reader preference ทำให้ใคร starvation ได้
10. writer preference มี trade-off อะไร
11. Dining Philosophers naive algorithm deadlock อย่างไร
12. Coffman condition ใดถูกทำลายด้วย resource ordering
13. จำกัด philosophers เป็น N-1 ช่วยอย่างไร
14. deadlock-free ยัง starvation ได้อย่างไร
15. coarse lock vs fine-grained locks trade-off อย่างไร
16. ออกแบบ bounded queue capacity 8
17. ออกแบบ read-mostly cache
18. ออกแบบ acquisition order สำหรับ resources A,B,C
19. วาด wait-for graph ของ dining deadlock
20. เปรียบเทียบ semaphore bounded-buffer กับ condvar bounded-buffer

---

## 24. Quiz

1. pthread_cond_wait return แล้ว mutex ถูก reacquire หรือไม่
2. condition wait ควรอยู่ใน while หรือไม่
3. rwlock read mode อนุญาตหลาย readers ได้หรือไม่
4. writer lock exclusive หรือไม่
5. fairness รับประกันจาก thread safety หรือไม่
6. Dining Philosophers ต้องถือ 2 resources หรือไม่
7. global lock ordering ช่วยทำลาย circular wait หรือไม่
8. semaphore สามารถ model empty/full slots ได้หรือไม่
9. signal = predicate guaranteed true หรือไม่
10. deadlock-free = starvation-free หรือไม่

### Answers

1. ใช่
2. ใช่
3. ใช่
4. ใช่
5. ไม่
6. ใช่ใน model นี้
7. ใช่
8. ใช่
9. ไม่
10. ไม่

---

## 25. Explain-It-Back

อธิบาย 3 problem โดยใช้ template เดียวกัน:

~~~text
Shared state:
Invariant:
Wait condition:
Critical section:
Primitive:
Failure if incorrect:
Fairness concern:
~~~

ถ้าทำครบทั้ง Producer–Consumer, Readers–Writers และ Dining Philosophers ได้ แสดงว่าพร้อม Chapter 08
