# Midterm Review — Chapters 01–05

## Coverage

Midterm scope:

1. OS Introduction
2. Process & Context Switch I
3. Process & Context Switch II
4. Concurrency I
5. Concurrency II

source alignment:

- Activity 02-1
- Activity 02-2
- Activity 03-1
- Activity 03-2

---

# Section A — Definitions

ต้องอธิบายโดยไม่เปิดโน้ต:

1. Operating System
2. Kernel
3. User Space
4. Kernel Space
5. System Call
6. Process
7. Program
8. PID
9. PPID
10. Thread
11. Context Switch
12. Mode Switch
13. Race Condition
14. Critical Section
15. Shared Mutable State
16. Atomicity
17. Visibility
18. Ordering
19. Volatile
20. Interlocked

---

# Section B — Process Tracing

อธิบาย:

~~~text
C# Program
→ .NET Runtime
→ Linux Process
→ Thread(s)
→ Scheduler
→ CPU
~~~

คำถาม:

1. Process.Start เป็น syscall หรือ API
2. WaitForExit ทำอะไร
3. fork concept คืออะไร
4. exec concept คืออะไร
5. exec สร้าง process ใหม่เสมอหรือไม่
6. zombie คืออะไร
7. /proc/PID/status ใช้ดูอะไร
8. /proc/PID/maps ใช้ดูอะไร
9. /proc/PID/task ใช้ดูอะไร
10. ManagedThreadId vs Linux TID

---

# Section C — Activity 02

## Sequential

ทำนาย final sum ของ:

~~~text
plus 1..1,000,000
minus 0..999,999
~~~

## Threaded

อธิบาย:

~~~text
Thread P
Thread M
Start
Join
System.Threading.Lock
Stopwatch
~~~

คำถาม:

1. ทำไมต้อง Join
2. lock ป้องกันอะไร
3. lock ทุก iteration มี overhead อย่างไร
4. ถ้าเอา lock ออกเกิดอะไร
5. Thread มากขึ้นรับประกันเร็วขึ้นหรือไม่

---

# Section D — Activity 03

state:

~~~text
x
exitflag
hasValue
lockObj
~~~

ต้องอธิบาย:

1. reader predicate
2. writer predicate
3. Wait ปล่อย lock หรือไม่
4. Wait return เมื่อไร
5. PulseAll ทำอะไร
6. ทำไมใช้ while
7. สาม readers แปลว่า broadcast หรือไม่
8. input “exit” ทำให้ threads จบอย่างไร

---

# Section E — Race Tracing

ให้สอง threads ทำ:

~~~text
counter initially 0

A: counter++
B: counter++
~~~

วาด interleaving ที่ final = 1

จากนั้นเสนอ:

- lock
- Interlocked

และอธิบาย trade-off

---

# Section F — Check-Then-Act

~~~text
stock = 1

A reads stock
B reads stock
A decrements
B decrements
~~~

ถาม:

1. invariant คืออะไร
2. ทำไม atomic decrement ตัวเดียวไม่ทำให้ check+act ทั้งชุด atomic
3. วิธีแก้ด้วย lock
4. วิธี redesign ด้วย CAS one-step transition

---

# Section G — Practical Commands

ต้องใช้ได้:

~~~bash
uname -r
ps
ps -L
pstree
strace
cat /proc/PID/status
cat /proc/PID/maps
ls /proc/PID/task
~~~

---

# Section H — Mock Midterm

## Q1

OS กับ Kernel ต่างกันอย่างไร

## Q2

System call กับ context switch ต่างกันอย่างไร

## Q3

Program กับ Process ต่างกันอย่างไร

## Q4

อธิบาย Process State model

## Q5

PCB เป็น C# class หรือไม่

## Q6

fork และ exec ต่างกันอย่างไร

## Q7

Zombie คืออะไร

## Q8

Thread share อะไรกับ Thread อื่นใน process เดียวกัน

## Q9

Thread.Sleep ใช้แทน Join ได้หรือไม่

## Q10

Concurrency กับ Parallelism ต่างกันอย่างไร

## Q11

วาด lost update

## Q12

Volatile แก้ counter++ ให้ atomic หรือไม่

## Q13

Interlocked ใช้ทำอะไร

## Q14

Activity 03 ทำไมต้อง Monitor.Wait

## Q15

ทำไม Wait ต้องอยู่ใน while

---

# Passing Standard

ผ่านเมื่อ:

~~~text
definition >= 80%
+
trace race ได้
+
อธิบาย source activities ได้
+
เขียน Thread Start/Join ได้เอง
+
อธิบาย Wait/Pulse protocol ได้
+
ใช้ Linux observation tools ได้
~~~
