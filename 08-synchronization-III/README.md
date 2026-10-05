# Chapter 08 — Synchronization Part III

## 1. Goals

หลังบทนี้ต้องสามารถ:

- อธิบาย Monitor abstraction
- ใช้ Condition Variable อย่างถูกต้อง
- อธิบาย Mesa-style condition-variable reasoning ใน pthreads
- วิเคราะห์ Deadlock ด้วย Coffman Conditions
- วาด Resource Allocation Graph / Wait-For Graph
- แยก Deadlock, Starvation และ Livelock
- อธิบาย prevention, avoidance, detection และ recovery
- เข้าใจ Priority Inversion และ Priority Inheritance ระดับ concept
- ใช้ lock ordering และ bounded retry เพื่อออกแบบระบบที่ปลอดภัยขึ้น

---

# Part I — Monitor & Condition Variables

## 2. Monitor คืออะไร

Monitor เป็น abstraction ที่รวม:

~~~text
shared state
+
operations over state
+
mutual exclusion
+
condition synchronization
~~~

conceptual interface:

~~~text
Monitor Queue
├── state: buffer, count, head, tail
├── put(item)
├── get()
├── condition not_empty
└── condition not_full
~~~

จุดสำคัญคือ caller ไม่ควรแก้ internal shared state โดยข้าม synchronization policy

---

## 3. Monitor ไม่เท่ากับ pthread API ตัวเดียว

C/POSIX ไม่มี keyword "monitor" แบบบางภาษา

เราสร้าง monitor-like abstraction ได้ด้วย:

- struct ที่เก็บ shared state
- pthread_mutex_t
- pthread_cond_t
- functions ที่ enforce invariants

---

## 4. Condition Variable Pattern

canonical pthread pattern:

~~~c
pthread_mutex_lock(&mutex);

while (!predicate) {
    pthread_cond_wait(&cond, &mutex);
}

/* predicate is true while mutex is held */
do_work();

pthread_mutex_unlock(&mutex);
~~~

producer/consumer ใน Chapter 07 คือ monitor-like design

---

## 5. Signal vs Broadcast

~~~c
pthread_cond_signal(&cond);
~~~

ปลุก waiter อย่างน้อยหนึ่งตัวตาม semantics ที่เกี่ยวข้อง

~~~c
pthread_cond_broadcast(&cond);
~~~

ปลุก waiters ทั้งหมด

แต่ทั้งสองแบบ:

~~~text
wake != predicate guaranteed true
~~~

waiter ต้อง reacquire mutex และตรวจ predicate ใหม่

---

## 6. Mesa-Style Reasoning

ใน pthread-style condition variables ผู้ signal ไม่ได้ "มอบ monitor" ให้ waiter ทันทีแบบ guarantee ว่า predicate ยัง true

ภาพ:

~~~text
T1 changes state
T1 signals
T1 may continue while holding mutex
T1 unlocks

waiter competes to reacquire mutex
waiter rechecks predicate
~~~

นี่คือเหตุผลหลักของ while loop

---

# Part II — Deadlock

## 7. Deadlock Definition

Deadlock คือชุด execution entities ที่ไม่สามารถ progress เพราะแต่ละตัวกำลังรอ resource/event ที่อีกตัวในชุดถือหรือทำให้เกิดได้

ตัวอย่าง 2 locks:

~~~text
Thread A holds L1, waits L2
Thread B holds L2, waits L1
~~~

wait cycle:

~~~text
A → L2 → B → L1 → A
~~~

---

## 8. Four Coffman Conditions

deadlock resource model แบบ classic ต้องมีเงื่อนไขทั้ง 4:

1. **Mutual Exclusion** — resource บางตัวใช้ร่วมพร้อมกันไม่ได้
2. **Hold and Wait** — ถือ resource อยู่และรอ resource เพิ่ม
3. **No Preemption** — resource ไม่ถูกดึงออกไปโดยระบบอย่างอิสระ
4. **Circular Wait** — มีวงจรรอ

ถ้าป้องกันอย่างน้อยหนึ่ง condition ได้ใน model นี้ deadlock แบบนั้นไม่เกิด

---

## 9. Resource Allocation Graph

nodes:

~~~text
P = process/thread
R = resource
~~~

edge concept:

~~~text
P -> R : requesting
R -> P : allocated
~~~

single-instance resource cycle เป็นสัญญาณสำคัญของ deadlock

แต่ใน graphs ที่ resource มีหลาย instances การเห็น cycle อย่างเดียวอาจไม่เพียงพอที่จะสรุป deadlock ทุกกรณี

---

## 10. Wait-For Graph

ถ้าลด resource nodes ออกในบาง model:

~~~text
T1 -> T2
~~~

แปลว่า T1 รอ resource ที่ T2 ถือ

cycle:

~~~text
T1 -> T2 -> T3 -> T1
~~~

เป็น deadlock indicator ใน model ที่เหมาะสม

examples/waitfor-cycle.c สาธิต cycle detection โดย **ไม่สร้าง threads ที่ค้างจริง**

---

## 11. Deadlock Prevention

เปลี่ยน design เพื่อทำลาย Coffman condition

### Global Lock Ordering

~~~text
L1 < L2 < L3

ทุก thread acquire จากน้อย → มาก
~~~

ทำลาย circular wait

### Acquire All at Once

ลด hold-and-wait แต่ practical constraints อาจสูง

### Preemption / rollback

บาง resource/transaction สามารถ rollback/retry ได้

### Avoid mutual exclusion

เฉพาะ resource ที่สามารถ redesign เป็น shareable/immutable ได้

---

## 12. Deadlock Avoidance

Avoidance ตัดสินใจว่าจะ grant request หรือไม่โดยดู state ว่ายัง safe หรือไม่

ตัวอย่าง classic:

- Banker's Algorithm

คำว่า **safe state** ไม่ได้แปลว่า "ตอนนี้ไม่มี deadlock เท่านั้น"

หมายถึงยังมีลำดับ completion ที่ทำให้ทุก participant สำเร็จได้ภายใต้ model

Banker's Algorithm เป็น teaching model สำคัญ แต่ OS ทั่วไปไม่ได้ใช้มันกับ mutex ทุกตัวใน application

---

## 13. Deadlock Detection

ระบบบางแบบยอมให้ deadlock มีโอกาสเกิด แล้ว:

1. สร้าง dependency graph
2. ตรวจ cycle/deadlock condition
3. เลือก recovery action

เหมาะกับบาง database/distributed/resource-manager designs

---

## 14. Recovery

ตัวเลือก concept:

- terminate participant
- rollback transaction
- preempt/reclaim resource ที่ preempt ได้
- restart component
- operator intervention

แต่ recovery มี cost และ consistency concerns

---

# Part III — Starvation

## 15. Starvation Definition

Starvation คือ participant พร้อมจะ progress แต่ถูกเลื่อนออกไปเป็นเวลานาน/ไม่จำกัดเพราะ policy หรือ contention

ตัวอย่าง:

~~~text
writer waits
readers ใหม่เข้ามาตลอด
writer never gets exclusive access
~~~

ไม่มี circular wait แบบ deadlock ก็ starvation ได้

---

## 16. Causes of Starvation

- unfair locks
- strict priority scheduling
- reader-preference algorithm
- resource hogging
- retry policy ที่เสียเปรียบ participant เดิม

solutions:

- fairer queueing
- aging
- bounded waiting
- writer-aware policy
- reservation/turnstile design

---

# Part IV — Livelock

## 17. Livelock Definition

Livelock คือ participants ยัง active และเปลี่ยน state อยู่ แต่ไม่มี useful progress

analogy:

~~~text
คนสองคนหลบกัน
ซ้ายพร้อมกัน
ขวาพร้อมกัน
ซ้ายพร้อมกัน...
~~~

ต่างจาก deadlock:

~~~text
Deadlock: หยุดรอ
Livelock: ขยับแต่ไม่ไปไหน
~~~

---

## 18. Livelock Demo

examples/livelock-demo.c ใช้ bounded rounds

สอง workers เห็น conflict แล้วพร้อมใจกัน back off หลายรอบ

demo จำกัดรอบเพื่อ:

- เห็น behavior
- ไม่สร้าง infinite loop
- จบได้เสมอ

solution concept:

- randomized/exponential backoff
- asymmetry
- priority/tie breaker
- centralized arbitration

---

# Part V — Deadlock vs Starvation vs Livelock

## 19. Comparison

| Property | Deadlock | Starvation | Livelock |
|---|---|---|---|
| Participants active? | มัก blocked/waiting | system อื่น progress ได้ | active/retrying |
| Useful progress ของ victim? | ไม่มี | ไม่มี/น้อยมาก | ไม่มี |
| Circular wait required? | classic deadlock model ใช่ | ไม่ | ไม่ |
| Fairness related? | อาจ | มาก | อาจ |
| Fix examples | ordering | aging/fairness | backoff/asymmetry |

---

# Part VI — Priority Inversion

## 20. Scenario

สาม tasks:

~~~text
Low priority L holds mutex M
High priority H needs M -> blocks
Medium priority M does CPU work
~~~

ถ้า scheduler ให้ M รันเหนือ L:

~~~text
H waits for L
L cannot run enough to release lock
M keeps running
~~~

effective result:

~~~text
high-priority task indirectly delayed by medium-priority task
~~~

นี่คือ priority inversion

---

## 21. Priority Inheritance

concept:

เมื่อ high-priority waiter ถูก block โดย low-priority lock holder:

~~~text
temporarily boost lock holder priority
↓
holder runs
↓
releases lock
↓
priority restored
~~~

POSIX mutex protocols และ real-time systems มีรายละเอียดเฉพาะ

อย่าทดลองเปลี่ยน real-time scheduling priority เป็น root บนเครื่องหลักโดยไม่จำเป็น

---

## 22. Priority Ceiling Preview

resource ถูกกำหนด ceiling priority ตาม participants ที่อาจใช้

protocol ช่วยควบคุม inversion/deadlock properties ใน real-time designs

ระดับนี้ให้เข้าใจ concept ก่อน ไม่ต้อง configure production RT scheduler

---

## 23. Common Misconceptions

### "Deadlock = program ช้า"

ผิด deadlock เป็น progress failure จาก dependency cycle/resource conditions

### "ไม่มี deadlock แปลว่า concurrency ถูกหมด"

ผิด ยังมี starvation, livelock, races

### "trylock แก้ deadlock เสมอ"

ผิด ถ้าทุก thread retry แบบ symmetric อาจ livelock

### "condition variable signal เก็บ token"

ไม่ใช่ semaphore

### "priority inversion คือ scheduler bug เสมอ"

ไม่ใช่ เป็น interaction ระหว่าง scheduling priority กับ resource dependency

---

## 24. Debugging Approach

เมื่อสงสัย deadlock:

1. หยุดเดา
2. หา threads/tasks ที่ block
3. หา locks/resources ที่รอ
4. หา owner
5. สร้าง wait-for relation
6. หา cycle
7. ตรวจ global lock order

tools ที่มีประโยชน์ตาม environment:

~~~bash
gdb
ps -L
/proc/PID/task
strace -f
perf
~~~

production debugging อาจใช้ tracing/eBPF/perf lock tools ตาม availability

---

## 25. Labs

### Monitor Queue

~~~bash
./bin/monitor-queue
~~~

ตรวจ:

- producer/consumer synchronization
- while predicate
- encapsulated state

### Wait-For Cycle Detector

~~~bash
./bin/waitfor-cycle
~~~

โปรแกรม model graph:

~~~text
T0 -> T1
T1 -> T2
T2 -> T0
~~~

และรายงาน cycle

### Livelock

~~~bash
./bin/livelock-demo
~~~

ดู retries ที่ active แต่ยังไม่มี progress ชั่วคราว

---

## 26. Exercises

1. monitor abstraction มีอะไรบ้าง
2. condvar signal ต่างจาก semaphore post อย่างไร
3. ทำไม waiter ต้องถือ mutex ก่อน wait
4. Coffman conditions 4 ข้อ
5. lock ordering ทำลาย condition ไหน
6. safe state ใน Banker's Algorithm คืออะไร
7. cycle ใน resource graph แปลว่า deadlock เสมอหรือไม่เมื่อหลาย instances
8. starvation ต่างจาก deadlock อย่างไร
9. livelock ต่างจาก deadlock อย่างไร
10. retry เหมือนกันทุก thread ทำไม livelock ได้
11. priority inversion scenario
12. priority inheritance ช่วยอย่างไร
13. bounded waiting คืออะไร
14. writer starvation เกิดได้อย่างไร
15. ออกแบบ lock order สำหรับ 5 resources
16. วาด wait-for graph 3 threads
17. เสนอ recovery จาก database deadlock
18. อธิบาย trade-off prevention vs detection
19. monitor ช่วย encapsulation อย่างไร
20. วิเคราะห์ system ที่ไม่มี deadlock แต่มี starvation

---

## 27. Quiz

1. pthread_cond_wait ควร recheck predicate หรือไม่
2. deadlock classic model ต้องมี 4 Coffman conditions หรือไม่
3. ordering locks ช่วย circular wait หรือไม่
4. starvation ต้องมี cycle หรือไม่
5. livelock participants active ได้หรือไม่
6. trylock รับประกันไม่มี livelock หรือไม่
7. priority inheritance boost lock holder ชั่วคราวหรือไม่
8. condition variable เป็น event counter หรือไม่
9. safe state = no current deadlock อย่างเดียวหรือไม่
10. deadlock-free = fair หรือไม่

### Answers

1. ใช่
2. ใช่
3. ใช่
4. ไม่
5. ได้
6. ไม่
7. ใช่
8. ไม่
9. ไม่
10. ไม่

---

## 28. Explain-It-Back

อธิบาย chain:

~~~text
Locking
→ multiple resources
→ hold-and-wait
→ circular dependency
→ deadlock

Avoid cycle
→ but fairness can still fail
→ starvation

Retry instead of block
→ but symmetric retries
→ livelock
~~~

ถ้าอธิบายได้พร้อมวาด graph ให้เข้าสู่ Scheduling
