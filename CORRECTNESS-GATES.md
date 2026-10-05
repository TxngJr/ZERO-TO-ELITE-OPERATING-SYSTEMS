# Correctness Gates

คอร์สนี้ไม่ถือว่า “ผ่าน” เพียงเพราะ dotnet build สำเร็จ

ต้องผ่าน 4 ชั้น:

~~~text
Compile
→ Terminate
→ Invariant
→ Expected algorithm semantics
~~~

---

## Chapter 03

ตรวจ:

~~~text
child created
child exits
parent WaitForExit completes
ExitCode = expected
~~~

---

## Chapter 05

visibility demo:

~~~text
writer publishes payload
Volatile.Write publishes readiness
reader uses Volatile.Read
reader prints payload=42
~~~

check-then-act demo ถูกตั้งใจให้แสดง logical race

ห้ามเขียน test ว่า final stock ต้องถูกเสมอ เพราะ demo มีไว้ให้เห็นความผิด

---

## Chapter 06

CAS:

~~~text
initial stock = 1
two buyers
exactly one transition 1 → 0 succeeds
final stock = 0
~~~

SemaphoreSlim:

~~~text
maximum concurrent workers <= capacity
Release always in finally
~~~

---

## Chapter 07

Producer–Consumer:

~~~text
0 <= count <= capacity at all times
no dequeue from logical empty buffer
all produced items consumed
final count = 0
all threads terminate
~~~

Dining Philosophers:

~~~text
global resource ordering
→ no circular wait
→ all philosophers finish bounded rounds
~~~

---

## Chapter 08

Wait-for graph:

- cyclic graph must report cycle
- acyclic graph must not report cycle

Livelock demo:

- all participants execute same Barrier phases
- bounded round count
- process must terminate within timeout
- demo shows repeated backoff, not accidental Barrier deadlock

---

## Chapter 09

Golden scheduling tests:

FCFS default workload:

~~~text
P1 CT=8
P2 CT=12
P3 CT=14
P4 CT=15
~~~

RR quantum 2:

~~~text
P2 first response begins at t=2
~~~

MLFQ:

~~~text
Q0 quantum=2
P1 owns CPU t=0..2 unless higher-priority queue exists
same-level arrival does not force 1-tick requeue
~~~

metrics:

~~~text
TAT >= 0
WT >= 0
RT >= 0
~~~

---

## Chapter 10

Address split:

~~~text
VA 0x12345
page size 4096
VPN 0x12
offset 0x345
~~~

simulator page size is explicit and independent from host Environment.SystemPageSize

protection translation must reject write to read-only PTE

TLB self-test must produce correct lookup after insert

---

## Chapter 11

Page replacement golden values:

reference:

~~~text
7 0 1 2 0 3 0 4 2 3 0 3 2
frames=3
~~~

expected:

~~~text
FIFO 10
LRU 9
OPT 7
CLOCK 9
~~~

Belady:

~~~text
1 2 3 4 1 2 5 1 2 3 4 5

FIFO 3 frames = 9
FIFO 4 frames = 10
~~~

file COW:

~~~text
mapping before = 10
private mapping write = 99
backing file remains = 10
~~~

page protection:

~~~text
child maps RW
mprotect → R
write attempt cannot complete normally
parent survives and observes abnormal child completion
~~~

---

# CI Rule

verify.sh ใช้ timeout สำหรับ concurrent demos

เหตุผล:

~~~text
a concurrency program that hangs
must fail CI
not hang CI forever
~~~
