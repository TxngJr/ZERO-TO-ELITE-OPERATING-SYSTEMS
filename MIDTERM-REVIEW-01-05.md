# Midterm Review — Chapters 01–05

> Scope: OS Introduction → Process/Context Switch → fork/exec/wait → Threads → Concurrency → Race Conditions → Shared State

Chapter 06 เป็นบทหลังขอบเขต midterm ตาม course plan แต่ใช้เป็นคำตอบของปัญหาที่ Chapters 4–5 สร้างขึ้น

---

# 1. Knowledge Map

~~~text
Hardware
   ↓
Operating System / Kernel
   ↓ system calls
User-space process
   ↓
Process state + CPU context
   ↓
fork / exec / wait
   ↓
Multiple execution flows
   ↓
Threads
   ↓
Interleaving
   ↓
Shared mutable state
   ↓
Race conditions
   ↓
Need synchronization
~~~

---

# 2. Must-Know Definitions

ต้องอธิบายด้วยคำตัวเอง ไม่ใช่ท่องหนึ่งบรรทัด:

- Operating System
- Kernel
- User Space
- Kernel Space
- System Call
- Program
- Process
- PID / PPID
- Process State
- PCB abstraction
- CPU Context
- Context Switch
- fork
- exec
- wait/waitpid
- Zombie
- Thread
- Concurrency
- Parallelism
- Interleaving
- Race Condition
- Critical Section
- Atomicity
- Shared Mutable State
- Data Race
- Thread Safety
- Reentrancy
- Memory Visibility
- Happens-Before intro

---

# 3. High-Risk Misconceptions

แก้ประโยคเหล่านี้:

1. Linux = Fedora
2. Kernel = ทุกอย่างใน OS
3. printf เป็น system call
4. system call = context switch ไป process ใหม่
5. program = process
6. PID ไม่เคยถูก reuse
7. Linux มี PCB struct ชื่อ PCB ที่ตรงกับ textbook 1:1
8. fork เริ่ม child จาก main ใหม่
9. exec สร้าง process ใหม่
10. zombie ยังรัน CPU อยู่
11. concurrency = parallelism
12. thread ทุกตัวมี address space แยกแบบ process
13. counter++ เป็น atomic เพราะเป็น C statement เดียว
14. atomic variable ทำให้ algorithm ทั้งหมด atomic
15. volatile แก้ thread synchronization
16. test ผ่าน 10,000 รอบแปลว่าไม่มี race

---

# 4. Part A — Multiple Choice

## Q1

ข้อใดอธิบาย kernel ดีที่สุด?

A. Desktop UI  
B. Privileged core ที่จัดการ resource/subsystems สำคัญ  
C. Package manager  
D. C compiler

## Q2

library function ใดไม่จำเป็นต้อง system call ทุกครั้ง?

A. strlen  
B. execve  
C. fork semantics  
D. opening a new filesystem file

## Q3

ข้อใดถูก?

A. mode switch ต้องเป็น process switch  
B. process switch ไม่ต้องรักษา CPU state  
C. system call สามารถกลับเข้า process เดิมโดยไม่ switch ไป process อื่น  
D. PID unique forever

## Q4

fork return ใน child คือ:

A. -1  
B. 0  
C. parent PID  
D. child PID

## Q5

successful exec:

A. returns 0  
B. creates another child automatically  
C. replaces current process image  
D. always changes PID

## Q6

Zombie คือ:

A. child ที่ terminate แล้วแต่ยังไม่ถูก reap  
B. blocked I/O thread  
C. deadlocked process  
D. orphan ที่รัน CPU 100%

## Q7

Concurrency:

A. ต้องมี 2 cores  
B. หมายถึง overlapping progress/lifetimes และ interleaving ได้  
C. เหมือน parallelism ทุกกรณี  
D. ไม่มี scheduling

## Q8

threads ใน process เดียวกันโดยทั่วไป:

A. มี stack เดียวกัน  
B. มี register set เดียวกันพร้อมกัน  
C. share process address space แต่มี execution state/stack ของตน  
D. มี PID namespace แยกเสมอ

## Q9

ใน C data race:

A. รับประกันแค่ lost update  
B. undefined behavior  
C. compiler ต้อง serialize  
D. volatile แก้ได้

## Q10

atomic_load + atomic_store แยกกัน:

A. ทำให้ compound read-modify-write atomic เสมอ  
B. แต่ละ access atomic ได้ แต่ transaction หลายขั้นยัง race ได้  
C. เท่ากับ mutex  
D. ห้ามใช้ threads

---

# 5. Part B — Short Answer

1. OS ทำ abstraction และ resource management อย่างไร
2. User Space vs Kernel Space ต่างกันอย่างไร
3. System Call boundary มีประโยชน์ด้าน protection อย่างไร
4. Program vs Process
5. PID vs PPID
6. PCB เป็น abstraction เพื่ออะไร
7. instruction pointer และ stack pointer สำคัญต่อ context switch อย่างไร
8. process state Ready vs Running vs Waiting
9. mode transition vs process context switch
10. fork vs exec vs wait
11. ทำไม Copy-on-Write เหมาะกับ fork
12. zombie ทำไมต้องมีข้อมูลเหลือให้ parent
13. thread ต่างจาก process อย่างไร
14. concurrency vs parallelism
15. race condition vs data race
16. critical section คืออะไร
17. thread-safe vs reentrant
18. memory visibility ทำไมต้องมี synchronization
19. ทำไม volatile ไม่ใช่ thread synchronization
20. ทำไม run ผ่านหลายรอบไม่ใช่ proof

---

# 6. Part C — Trace Process

พิจารณา pseudo-C:

~~~c
printf("start\n");
pid_t p = fork();

if (p == 0) {
    execlp("echo", "echo", "child", NULL);
    perror("exec");
    _exit(127);
}

waitpid(p, NULL, 0);
printf("parent done\n");
~~~

ตอบ:

1. "start" เกิดก่อน fork หรือไม่
2. child branch รู้ได้อย่างไรว่าเป็น child
3. ถ้า exec สำเร็จ perror ทำงานหรือไม่
4. "child" กับ "parent done" มี ordering relation แบบใดจาก wait
5. PID ของ child ก่อน/หลัง successful exec โดย mental model หลักเปลี่ยนหรือไม่
6. zombie window สามารถเกิดตรงไหนถ้า parent ยังไม่ wait

---

# 7. Part D — Trace Interleaving

shared logical value x=0

~~~text
Thread A: load x
Thread B: load x
Thread A: store loaded+1
Thread B: store loaded+1
~~~

1. final x เป็นอะไร
2. expected sequential result คืออะไร
3. lost update เกิดตรงไหน
4. ถ้า x เป็น C plain int และ threads access unsynchronized จะมีปัญหา language-level อะไร
5. ถ้า x เป็น atomic แต่ใช้ load/store แยกกัน race condition ยังเป็นไปได้หรือไม่

---

# 8. Part E — Check–Then–Act

initial stock = 1

~~~text
T1 check stock
T2 check stock
T1 buy
T2 buy
~~~

ตอบ:

1. invariant ที่ควรเป็นคืออะไร
2. final stock อาจผิดอย่างไร
3. atomic individual accesses เพียงอย่างเดียวพอหรือไม่
4. logical operation ที่ต้อง synchronize คือส่วนใด

---

# 9. Part F — Linux Commands

อธิบายคำสั่งและสิ่งที่สังเกต:

~~~bash
uname -r
lscpu
echo $$
ps -o pid,ppid,stat,comm -p $$
pstree -p
cat /proc/$$/status
cat /proc/$$/maps
ls -l /proc/$$/fd
strace -e trace=write ./program
strace -f -e trace=process ./program
ps -L -p PID -o pid,tid,psr,stat,comm
ls /proc/PID/task
~~~

ต้องรู้ว่า output แบบ PID/address/core assignment เปลี่ยนได้

---

# 10. Part G — Code Reading

## Q1

~~~c
pid_t p = fork();

if (p == 0) {
    puts("A");
} else if (p > 0) {
    puts("B");
}
~~~

อะไร guaranteed และอะไรไม่ guaranteed เกี่ยวกับ A/B ordering

## Q2

~~~c
_Atomic int x = 0;

int old = atomic_load(&x);
atomic_store(&x, old + 1);
~~~

ทำไม x เป็น atomic ยังไม่ได้ทำให้ increment ทั้งชุด atomic

## Q3

~~~c
int ready = 0;
int data = 0;

/* writer */
data = 42;
ready = 1;

/* reader */
while (!ready) {}
printf("%d", data);
~~~

ถ้ารันหลาย threads โดยไม่มี synchronization ปัญหาคืออะไรใน C

## Q4

~~~c
pthread_create(...);
pthread_detach(thread);
return 0;
~~~

detached worker รับประกันว่าจะทำงานจนเสร็จหลัง main return หรือไม่

---

# 11. Part H — Explain the Output

ก่อนรันแต่ละโปรแกรมจาก repo ให้ prediction:

~~~bash
04-concurrency-I/bin/thread-basic
04-concurrency-I/bin/lost-update
05-concurrency-II/bin/check-then-act
05-concurrency-II/bin/release-acquire
05-concurrency-II/bin/detached-demo
~~~

เขียน:

- deterministic properties
- nondeterministic properties
- invariant
- scheduling-sensitive output

---

# 12. Practice Midterm — 30 Points

## Section 1 — Concepts (10 points)

ตอบสั้น 1 คะแนน/ข้อ:

1. OS vs Kernel
2. syscall
3. program vs process
4. PCB
5. context switch
6. fork
7. exec
8. thread
9. race condition
10. critical section

## Section 2 — Analysis (10 points)

### 11–12

อธิบายว่าทำไม user→kernel→same process ไม่ใช่ process switch เสมอ

### 13–14

วาด fork parent/child return values

### 15–16

อธิบาย zombie lifecycle

### 17–18

trace lost update ของ two-thread counter

### 19–20

อธิบาย race condition vs data race

## Section 3 — Linux/Code (10 points)

### 21–22

ใช้ /proc paths ใดดู:

- memory maps
- file descriptors

### 23–24

strace -f มีประโยชน์อะไรกับ fork/exec lab

### 25–26

ps -L ช่วยดูอะไร

### 27–28

ทำไม address จาก /proc/PID/maps ไม่ควรถูกจำ

### 29–30

ทำไม volatile ไม่ใช่ synchronization primitive

---

# 13. Answer Key

## Multiple Choice

1. B
2. A
3. C
4. B
5. C
6. A
7. B
8. C
9. B
10. B

## Process Trace

1. ใช่ ตาม program order ก่อน fork call
2. fork return 0 ใน child
3. ไม่; successful exec ไม่ return
4. wait ทำให้ parent done เกิดหลัง child termination ที่ wait รอ
5. ไม่เปลี่ยนจาก exec เพียงอย่างเดียวใน mental model นี้
6. หลัง child terminate และก่อน parent reap

## Interleaving

1. 1
2. 2
3. ทั้งสองโหลด 0 ก่อน แล้ว store 1 ทับกัน
4. data race/undefined behavior
5. ได้ เพราะ compound action ไม่ atomic

## Check–Then–Act

1. stock >= 0 และขายไม่เกิน inventory
2. oversell/negative stock
3. ไม่พอ
4. check + state transition/update ต้องถูกมองเป็น transaction/critical operation

## Code Reading

Q1: ทั้ง A/B เกิดหนึ่งครั้งถ้า fork สำเร็จ แต่ relative order ไม่ guaranteed  
Q2: เป็นสอง atomic operations แยกกัน  
Q3: unsynchronized conflicting accesses/data race และ visibility/order ไม่มี guarantee ตามที่โค้ดหวัง  
Q4: ไม่; process exit สิ้นสุด threads ทั้งหมด

---

# 14. Midterm Mastery Gate

ก่อนถือว่าพร้อมสอบ ต้องทำได้:

- วาด hardware → kernel → user-space stack
- trace system-call boundary
- trace process lifecycle
- trace fork/exec/wait
- แยก process switch จาก privilege transition
- อ่าน PID/PPID และ /proc
- แยก process vs thread
- สร้าง interleaving ด้วยมือ
- หา invariant ที่ race ทำให้พัง
- แยก race condition/data race
- อธิบายว่าทำไม synchronization จำเป็น

ถ้ายังอ่อน Chapters 4–5 ให้กลับไป trace interleavings ก่อนท่องชื่อ Mutex/Semaphore
