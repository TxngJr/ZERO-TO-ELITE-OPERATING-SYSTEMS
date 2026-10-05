# Chapter 04 — Concurrency Part I

## 1. Goals

หลังบทนี้ต้องสามารถ:

- แยก Sequential, Concurrent และ Parallel execution ได้
- อธิบาย Process vs Thread ได้
- เข้าใจว่า threads ใน process เดียวกัน share อะไร และมี state ส่วนตัวอะไร
- สร้างและ join POSIX threads ด้วย pthreads
- อ่าน execution interleaving ได้
- อธิบาย Race Condition, Critical Section และ Atomicity
- เข้าใจว่าลำดับ output ของ threads มักไม่ deterministic
- ใช้ ps -L, top -H และ /proc สังเกต threads บน Fedora
- เข้าใจว่าตัวอย่าง "counter++ แบบไม่ล็อก" ในภาษา C อาจกลายเป็น data race/undefined behavior และไม่ควรใช้เป็น mental model ที่แม่นยำโดยไม่อธิบาย

---

## 2. Why Concurrency Exists

เครื่องหนึ่งเครื่องอาจมี:

- browser
- terminal
- editor
- database
- network service
- background services

ทำงานพร้อมกันในช่วงเวลาเดียวกัน

ภายใน application เดียวก็อาจต้อง:

- รับ input
- อ่าน network
- decode data
- render UI
- เขียน disk
- ทำ computation

ถ้าทุกอย่างต้องรอทีละงาน ระบบอาจตอบสนองช้าและใช้ hardware ไม่เต็มประสิทธิภาพ

---

## 3. Sequential vs Concurrent vs Parallel

### Sequential

งานหนึ่งเสร็จก่อนอีกงานเริ่ม:

~~~text
time →
A: [AAAAAA]
B:       [BBBBBB]
~~~

### Concurrent

งานหลายงานมีช่วง lifetime ทับซ้อนกัน และ execution สามารถ interleave:

~~~text
time →
A: [AA]    [AAA]   [A]
B:    [BB]     [BBB]
~~~

แม้มี CPU core เดียวก็เกิด concurrency ได้ผ่าน scheduling/interleaving

### Parallel

สองงาน execute จริงในเวลาเดียวกันบน execution resources คนละชุด เช่นคนละ CPU core:

~~~text
Core 0: [AAAAAA]
Core 1: [BBBBBB]
~~~

ดังนั้น:

~~~text
Concurrency ≠ Parallelism
~~~

Concurrency เป็นเรื่องของการจัดการหลายงานที่ progress ทับซ้อนกัน  
Parallelism เป็นเรื่องของการ execute พร้อมกันจริง

---

## 4. Process vs Thread

Process เป็น resource/container + execution abstraction ขนาดใหญ่กว่า

Thread คือ execution flow ภายใน process

simplified model:

~~~text
Process
├── Virtual address space
│   ├── Code
│   ├── Global/static data
│   └── Heap
│
├── Thread 1
│   ├── Registers
│   ├── Instruction pointer
│   └── Stack
│
└── Thread 2
    ├── Registers
    ├── Instruction pointer
    └── Stack
~~~

### โดยทั่วไป threads ใน process เดียวกัน share

- code mappings
- global/static data
- heap
- open file descriptor table semantics
- process-level resources หลายอย่าง

### แต่แต่ละ thread ต้องมี execution state ของตัวเอง

- register state
- stack
- instruction pointer/program counter concept
- scheduling state บางส่วน
- thread-specific data

รายละเอียด Linux implementation มี sharing rules และ task structures ที่ซับซ้อนกว่ารูปนี้

---

## 5. Linux Thread Mental Model

Linux scheduler จัดการ schedulable tasks

POSIX thread ใน user-space program map ไปยัง kernel scheduling entities ตาม Linux threading implementation

อย่าจำว่า:

~~~text
หนึ่ง process = สิ่งเดียวที่ scheduler เห็น
~~~

เพราะ multithreaded process มีหลาย execution entities ที่ scheduler สามารถ schedule ได้

---

## 6. POSIX Threads

header:

~~~c
#include <pthread.h>
~~~

สร้าง thread:

~~~c
pthread_create(...)
~~~

รอ thread จบ:

~~~c
pthread_join(...)
~~~

compile:

~~~bash
gcc -Wall -Wextra -Wpedantic -std=c17 program.c -o program -pthread
~~~

-pthread สำคัญกว่าแค่การเติม library name เพราะ compiler/linker driver สามารถตั้ง options ที่เกี่ยวข้องกับ threading environment ด้วย

---

## 7. First Thread Program

build:

~~~bash
cd 04-concurrency-I
make
./bin/thread-basic
~~~

รันหลายครั้ง:

~~~bash
for i in {1..5}; do ./bin/thread-basic; done
~~~

ให้สังเกต:

- process PID เดียวกัน
- Linux TID ของ worker ต่างกัน
- output order อาจเปลี่ยน

ตัวอย่างใช้ Linux gettid ผ่าน SYS_gettid เพื่อสังเกต kernel thread IDs โดยตรง

---

## 8. Interleaving

สมมติ thread สองตัว:

~~~text
Thread A               Thread B
--------               --------
A1
                       B1
A2
                       B2
A3
~~~

scheduler ไม่จำเป็นต้อง execute function ทั้งก้อนของ A ก่อน B

สิ่งที่ programmer เห็นจึงอาจเป็นหลาย valid schedules

### Important

interleaving ไม่ได้แปลว่า instruction ทุกตัวสลับ A-B-A-B อย่างสวยงาม

scheduler, compiler, CPU, blocking, core count และ runtime state ล้วนมีผลต่อ observation

---

## 9. Shared State

ถ้า threads share memory:

~~~text
Thread A ----+
             +---- shared variable
Thread B ----+
~~~

ความสะดวกนี้ทำให้ communication เร็ว แต่สร้าง correctness problems ได้

---

## 10. Race Condition

Race condition คือความถูกต้องของผลลัพธ์ขึ้นกับ relative timing/order ของ concurrent operations

ตัวอย่างเชิงตรรกะ:

~~~text
shared counter = 0

Thread A: read counter -> 0
Thread B: read counter -> 0
Thread A: compute 1
Thread B: compute 1
Thread A: store 1
Thread B: store 1

final counter = 1
~~~

ทั้งที่เราคาดว่าควรเป็น 2

นี่เรียก lost update

---

## 11. ทำไมไม่ใช้ plain shared counter++ เป็น demo หลัก

ในภาษา C ถ้า threads หลายตัว access object เดียวกันพร้อมกัน โดยมี write และไม่มี synchronization ที่ถูกต้อง อาจเป็น **data race**

ตาม C memory model, data race ทำให้ behavior เป็น undefined

ดังนั้นการบอกว่า:

~~~c
counter++;
~~~

"จะกลายเป็น load/add/store เสมอ แล้วแค่ได้ค่าผิดนิดหน่อย"

เป็นคำอธิบายที่ไม่แม่นพอสำหรับ C

ในคอร์สนี้ demo lost-update ใช้ C atomic object เพื่อให้ individual load/store defined แต่จงใจแยก read-modify-write ออกเป็นหลาย operations จึงยังเกิด logical race condition ได้โดยไม่สร้าง C data race

---

## 12. Safe-to-Observe Lost Update Demo

ไฟล์ examples/lost-update.c ใช้:

~~~c
_Atomic long counter;
~~~

แต่จงใจทำ:

~~~text
atomic load
yield opportunity
atomic store
~~~

แทน atomic read-modify-write

ดังนั้น:

- load/store แต่ละครั้งเป็น atomic ตาม C model
- แต่ operation "increment" ทั้งชุดไม่ atomic
- threads จึง overwrite กันได้

build/run:

~~~bash
./bin/lost-update
./bin/lost-update
./bin/lost-update
~~~

เทียบ:

~~~text
Expected = THREADS × ITERATIONS
Observed = ...
~~~

ค่าจริงขึ้นกับ schedule และเครื่อง

---

## 13. Critical Section

Critical section คือส่วนของ code ที่ access shared state/resource และต้องมี correctness rule ป้องกัน concurrent conflict

ตัวอย่าง:

~~~text
read balance
check condition
update balance
~~~

ถ้าต้องมองเป็น operation เดียว การปล่อยให้ threads แทรกระหว่างขั้นอาจทำให้ invariant พัง

---

## 14. Atomicity

Atomic operation ใน mental model คือ operation ที่ observer ที่เกี่ยวข้องไม่เห็น "ครึ่งหนึ่ง" ของ operation ตาม guarantees ของ primitive/memory model นั้น

อย่าใช้คำว่า atomic = "เร็วมาก"

และอย่าใช้ว่า atomic = "แก้ concurrency ทุกแบบ"

ตัวอย่าง:

- atomic increment อาจแก้ counter update
- แต่ invariant ที่เกี่ยวหลาย variables อาจต้อง lock หรือ design อื่น

Chapter 06 จะเรียน atomics ลึกขึ้น

---

## 15. Thread Lifecycle

simplified:

~~~text
pthread_create
     ↓
runnable thread
     ↓
running / waiting / runnable ...
     ↓
thread function returns
     ↓
terminated
     ↓
pthread_join collects completion
~~~

joinable thread ที่ terminate แล้วมี resources บางอย่างต้องถูก join เพื่อ release ตาม API contract

---

## 16. Observe Threads on Fedora

รัน demo ที่อยู่ค้างชั่วคราว:

~~~bash
./bin/thread-observe
~~~

เปิดอีก terminal:

~~~bash
ps -L -p PID -o pid,tid,psr,stat,comm
~~~

หรือ:

~~~bash
top -H -p PID
~~~

fields:

- PID = process ID
- TID/LWP = thread/task ID ตาม tool terminology
- PSR = logical CPU ล่าสุด/ปัจจุบันที่รายงาน

อย่าคาดว่า thread จะอยู่ core เดิมตลอดเวลา ถ้าไม่ได้ pin affinity

---

## 17. /proc and Threads

สำหรับ Linux:

~~~bash
ls /proc/PID/task
~~~

จะเห็น directories ตาม task/thread IDs

ลอง:

~~~bash
ls /proc/PID/task/TID
cat /proc/PID/task/TID/status | head
~~~

นี่ช่วยเชื่อม POSIX thread กับ Linux kernel-visible task state

---

## 18. Scheduling and Nondeterminism

ถ้า output สลับลำดับ ไม่ได้แปลว่า OS "สุ่ม"

scheduler ตัดสินใจตาม state/policy/timing/events แต่จากมุม programmer อาจ predict exact ordering ไม่ได้

ดังนั้น concurrent correctness ต้องไม่พึ่ง "ปกติ thread A น่าจะรันก่อน"

---

## 19. Common Misconceptions

### "Concurrency = หลาย core"

ผิด

core เดียวก็ interleave หลาย tasks ได้

### "Parallelism = concurrency เสมอ"

parallel execution เป็นรูปหนึ่งของ overlapping work แต่สองคำเน้นคนละมิติ

### "Threads ไม่ต้องมี stack แยก"

ผิด แต่ละ thread ต้องมี execution stack ของตัวเอง

### "Shared memory หมายถึงทุกอย่างของ thread ถูก share"

ผิด registers/stack/execution state มีส่วนตัว

### "counter++ เป็น atomic เพราะเขียน C แค่บรรทัดเดียว"

ผิด source-level statement ไม่ได้แปลว่า atomic operation

### "ถ้ารัน 100 ครั้งแล้วยังไม่เจอบัค แปลว่า thread-safe"

ผิด concurrency bug อาจ timing-sensitive

---

## 20. Performance Perspective

threads สามารถช่วย throughput/latency เมื่อ:

- มี parallelizable CPU work
- overlap I/O waits
- แยก independent work

แต่เพิ่ม overhead:

- creation/teardown
- scheduling
- context switching
- synchronization
- cache contention
- false sharing (preview)

more threads ≠ always faster

---

## 21. Exercises

### Recall

1. Concurrency คืออะไร
2. Parallelism คืออะไร
3. Thread คืออะไร
4. pthread_create ทำอะไร
5. pthread_join ทำอะไร

### Understanding

6. ทำไม single-core CPU ยังมี concurrency ได้
7. threads share heap อย่างไร
8. ทำไม stack ต้องแยกต่อ thread
9. race condition คืออะไร
10. critical section คืออะไร
11. atomicity คืออะไร

### Trace

12. trace A1,B1,A2,B2 แล้วบอกว่าเป็น concurrent schedule หรือไม่
13. shared x=0; A load, B load, A store 1, B store 1 — final x เท่าไร
14. ทำไม output thread-basic เปลี่ยน order ได้
15. process PID กับ Linux TID ต่างกันอย่างไรใน lab

### Analyze

16. ทำไม plain unsynchronized counter++ ใน C เป็น demo ที่ต้องระวัง
17. lost-update.c ไม่มี C data race บน counter แต่ยังมี race condition ได้อย่างไร
18. ทำไม exact core assignment เปลี่ยนได้
19. more threads อาจช้าลงได้อย่างไร
20. ออกแบบ critical section สำหรับ shared queue แบบ concept

---

## 22. Quiz

1. Concurrent ต้อง execute พร้อมกันจริงหรือไม่
2. Parallel ต้องมี simultaneous execution หรือไม่
3. threads ใน process เดียวกัน share virtual address space โดยทั่วไปหรือไม่
4. แต่ละ thread มี stack ของตัวเองหรือไม่
5. source statement หนึ่งบรรทัดรับประกัน atomicity หรือไม่
6. data race ใน C เป็น defined lost-update behavior เสมอหรือไม่
7. pthread_join ใช้รอ joinable thread หรือไม่
8. /proc/PID/task ใช้สังเกต Linux tasks/threads ได้หรือไม่
9. scheduling order ควรถูก hardcode ใน correctness assumption หรือไม่
10. race condition ต้องเกี่ยวกับ shared timing/order dependency หรือไม่

### Answers

1. ไม่
2. ใช่ในความหมายหลัก
3. ใช่
4. ใช่
5. ไม่
6. ไม่; data race นำไป undefined behavior
7. ใช่
8. ใช่
9. ไม่
10. ใช่

---

## 23. Explain-It-Back

อธิบายให้เพื่อนฟัง:

1. Process vs Thread
2. Concurrency vs Parallelism
3. Interleaving คืออะไร
4. Race Condition เกิดอย่างไร
5. Data Race กับ Race Condition ไม่ใช่คำเดียวกันอย่างไรในระดับ introductory
6. ทำไม Chapter 06 ต้องมี synchronization
