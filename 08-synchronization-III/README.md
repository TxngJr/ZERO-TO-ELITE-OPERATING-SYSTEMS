# Chapter 08 — Synchronization Part III

## เป้าหมาย

หลังบทนี้ต้องเข้าใจ:

- Monitor abstraction
- waiting queue / ready queue
- Deadlock
- Coffman Conditions
- Resource Allocation / Wait-For Graph
- Prevention / Avoidance / Detection / Recovery
- Starvation
- Livelock
- Priority Inversion
- bounded deterministic concurrency demos

---

## 1. Monitor

monitor-style abstraction รวม:

~~~text
shared state
+
mutual exclusion
+
operations
+
condition waiting
~~~

Activity 03 และ Thread-Safe Buffer เป็นตัวอย่างโดยตรง

---

## 2. Wait Queue vs Ready Queue

เมื่อ Thread ถือ monitor แล้วเรียก Wait:

~~~text
owner
↓
Monitor.Wait
↓
release monitor
↓
waiting queue
↓ Pulse/PulseAll
ready queue
↓ compete
reacquire monitor
↓
Wait returns
~~~

นี่คือเหตุผลว่าทำไม PulseAll ไม่ทำให้ทุก thread execute critical section พร้อมกัน

---

## 3. Deadlock

deadlock แบบ classic:

~~~text
T1 holds A, waits B
T2 holds B, waits A
~~~

ไม่มี participant ใด progress ได้

---

## 4. Coffman Conditions

เงื่อนไข classic 4 ข้อ:

1. Mutual Exclusion
2. Hold and Wait
3. No Preemption
4. Circular Wait

ถ้าทำลายอย่างน้อยหนึ่ง condition ใน model นั้น deadlock แบบดังกล่าวไม่เกิด

---

## 5. Wait-For Graph

edge:

~~~text
T1 → T2
~~~

หมายถึง T1 รอ resource/event ที่ T2 ต้องปล่อยหรือทำให้เกิด

single-instance wait-for model:

~~~text
cycle
→ deadlock indicator
~~~

Lab ใช้ DFS cycle detection เพื่อเรียน dependency โดยไม่ตั้งใจทำ process ค้าง

---

## 6. Prevention

ตัวอย่าง:

### Global lock ordering

ทำลาย circular wait

### Acquire-all policy

ลด hold-and-wait แต่ลด concurrency และเพิ่มข้อจำกัด

### Rollback / retry

ใช้ได้เฉพาะ resource/state ที่ย้อนกลับได้

---

## 7. Avoidance

Banker's Algorithm เป็น teaching model สำหรับ safe state

safe state:

~~~text
มี completion sequence ที่ทำให้ participants จบได้
~~~

ไม่ได้แปลเพียงว่า “ตอนนี้ยังไม่ deadlock”

OS ทั่วไปไม่ได้ใช้ Banker's Algorithm กับ C# lock ทุกตัว

---

## 8. Detection / Recovery

บางระบบยอมให้ dependency เกิด แล้วตรวจ:

~~~text
build dependency graph
→ detect cycle/problem
→ choose recovery
~~~

recovery อาจเป็น:

- abort transaction
- restart participant
- rollback
- release reclaimable resource

---

## 9. Starvation

ระบบส่วนอื่น progress แต่ participant หนึ่งถูกเลื่อนออกไปนานมาก

ไม่จำเป็นต้องมี cycle

ตัวอย่าง:

- unfair scheduling
- priority policy
- repeated lock competition

---

## 10. Livelock

participants active และตอบสนองต่อกัน แต่ไม่มี useful progress

~~~text
both detect conflict
→ both back off
→ both retry together
→ repeat
~~~

ต่างจาก deadlock:

~~~text
deadlock = blocked dependency
livelock = active retry without useful progress
~~~

---

## 11. ทำไม Lab Livelock ต้อง deterministic

demo เดิมที่ใช้ Barrier ไม่ครบ phase สามารถทำให้ thread หนึ่ง return ขณะที่อีก thread รอ Barrier และกลายเป็น accidental deadlock

เวอร์ชันแก้ใช้ 3 phases ต่อ round:

~~~text
announce intent
Barrier
snapshot conflict
Barrier
back off
Barrier
~~~

ทุก thread ผ่าน phase count เท่ากัน จึงไม่ทิ้ง partner ค้าง

และจำกัด rounds เพื่อให้ test จบแน่นอน

---

## 12. Priority Inversion

scenario:

~~~text
Low holds lock
High needs lock → blocks
Medium runs CPU work
Low delayed
therefore High indirectly delayed
~~~

priority inheritance concept:

temporarily boost lock holder เพื่อให้ปล่อย resource เร็วขึ้น

คอร์สพื้นฐานไม่เปลี่ยน real-time scheduling priority ของเครื่องหลัก

---

## Lab

~~~bash
dotnet run --project 08-synchronization-III/examples/Chapter08.csproj -- monitor
dotnet run --project 08-synchronization-III/examples/Chapter08.csproj -- graph
dotnet run --project 08-synchronization-III/examples/Chapter08.csproj -- livelock
dotnet run --project 08-synchronization-III/examples/Chapter08.csproj -- self-test
~~~

---

## แบบฝึกหัด

1. Monitor abstraction คืออะไร
2. waiting queue vs ready queue
3. Coffman 4 conditions
4. lock ordering ทำลาย condition ใด
5. วาด wait-for graph 3 threads
6. หา cycle
7. safe state คืออะไร
8. prevention vs avoidance
9. detection vs prevention
10. starvation vs deadlock
11. livelock vs deadlock
12. อธิบาย 3-phase livelock demo
13. ทำไม Barrier participant count ต้องตรง
14. สร้าง acyclic graph test
15. สร้าง cyclic graph test
16. อธิบาย priority inversion
17. priority inheritance
18. deadlock-free = fair หรือไม่
19. try-lock retry ทำไมอาจ livelock
20. เสนอ asymmetric tie-breaker

---

## Explain-It-Back

~~~text
multiple resources / conditions
→ dependency
→ deadlock or starvation/livelock risk
→ graph/policy analysis
→ prevention/detection/recovery
~~~
