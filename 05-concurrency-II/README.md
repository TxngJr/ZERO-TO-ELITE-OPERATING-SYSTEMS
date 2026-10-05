# Chapter 05 — Concurrency Part II

## เป้าหมาย

หลังบทนี้ต้องเข้าใจ:

- Multithreading
- Interleaving
- Shared Resources
- Read-Modify-Write
- Check-Then-Act
- Visibility
- Ordering
- Volatile
- Interlocked
- Thread Safety
- Reentrancy
- Foreground / Background Threads

---

## 1. Shared Mutable State

ปัญหา concurrency ส่วนใหญ่เริ่มจาก:

~~~text
multiple execution flows
+
same mutable state
+
insufficient synchronization
~~~

ทางออกไม่ได้มีแค่ lock

ยังมี:

- immutability
- ownership
- thread-local state
- message passing
- atomic operations

---

## 2. Read-Modify-Write

ตัวอย่าง:

~~~csharp
counter++;
~~~

เป็น logical read-modify-write

ถ้าต้องการ atomic increment ใช้:

~~~csharp
Interlocked.Increment(ref counter);
~~~

---

## 3. Check-Then-Act

ผิด:

~~~text
check stock > 0
↓
another thread changes stock
↓
act using stale observation
~~~

แม้ act ใช้ Interlocked แต่ถ้า check กับ act เป็นคนละ atomic event business invariant ยังพังได้

---

## 4. Atomicity vs Visibility vs Ordering

ต้องแยก 3 เรื่อง:

### Atomicity
operation ไม่เห็น state กลางบางแบบ

### Visibility
write จาก thread หนึ่งถูก observe โดยอีก thread ตาม synchronization rules

### Ordering
compiler/JIT/CPU/runtime ต้องเคารพ ordering constraints ที่ synchronization primitive กำหนด

ห้ามลดทุกอย่างเป็นคำว่า “race” อย่างเดียว

---

## 5. Volatile

C# มี:

~~~csharp
Volatile.Read(ref value);
Volatile.Write(ref value, newValue);
~~~

volatile access ใช้กับ visibility/order protocol บางรูปแบบ

แต่ไม่ทำให้ compound operation กลายเป็น atomic

ผิด:

~~~text
volatile int counter;
counter++;
→ atomic
~~~

ไม่จริง

---

## 6. Interlocked

เหมาะกับ atomic state transition ขนาดเล็ก

เช่น:

- Increment
- Decrement
- Exchange
- CompareExchange

แต่ invariant หลาย fields อาจยังต้อง lock/protocol ที่ใหญ่กว่า

---

## 7. Thread Safety

thread-safe หมายถึงใช้งาน concurrent ตาม contract แล้ว state/behavior ยังถูกต้อง

ต้องระบุ contract ด้วย

เช่น:

~~~text
method individually thread-safe
does not automatically mean
two-method transaction is atomic
~~~

---

## 8. Reentrancy

reentrant และ thread-safe ไม่ใช่คำเดียวกัน

reentrancy สนใจ function ถูกเรียกซ้อน/interrupt-like reentry แล้ว state ปลอดภัยหรือไม่

thread safety สนใจ concurrent calls ตาม contract

---

## 9. Background Threads

C#:

~~~csharp
thread.IsBackground = true;
~~~

ถ้า process เหลือแต่ background threads runtime สามารถ terminate process ได้

เพื่อเห็นความต่าง lab ควรเปรียบเทียบ:

~~~text
foreground no Join
background no Join
background with Join
~~~

ไม่ควร demo background แล้ว Join ทันทีเพียงกรณีเดียว

---

## 10. Activity 03 State Predicate

Activity 03 มี:

~~~text
hasValue
exitflag
~~~

นี่คือตัวอย่าง state predicate

ต่อไป Chapter 06–08 จะใช้:

~~~text
while predicate false
→ Wait
→ wake
→ re-check
~~~

---

## แบบฝึกหัด

1. shared mutable state คืออะไร
2. read-modify-write คืออะไร
3. check-then-act bug
4. atomicity vs visibility
5. visibility vs ordering
6. Volatile ใช้แทน lock ได้ทุกกรณีหรือไม่
7. Interlocked.Increment แก้อะไร
8. CAS คืออะไร
9. business transaction หลายขั้นใช้ Interlocked ตัวเดียวพอไหม
10. thread-safe vs reentrant
11. foreground vs background
12. สร้าง background no-Join experiment
13. ใช้ ThreadLocal สร้าง per-thread state
14. หา state predicates ใน Activity 03
15. เตรียม pseudo-code สำหรับ Monitor.Wait

---

## Explain-It-Back

~~~text
shared state
→ operation semantics
→ atomicity / visibility / ordering
→ choose synchronization strategy
~~~
