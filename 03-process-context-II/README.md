# Chapter 03 — Process & Context Switch Part II

## เป้าหมาย

หลังบทนี้ต้องอธิบายได้:

- Context Switch
- Scheduling point
- fork concept
- exec concept
- wait/waitpid concept
- Process Lifecycle
- Zombie
- Exit status
- Copy-on-Write preview
- C# Process.Start / WaitForExit mapping

---

## 1. Context Switch

simplified:

~~~text
Thread A executing
↓
event / scheduling decision
↓
save required A state
↓
choose Thread B
↓
restore B state
↓
Thread B executing
~~~

cost อาจมาจาก:

- scheduler work
- save/restore state
- cache effects
- TLB/address-space effects
- migration between CPUs

---

## 2. Context Switch vs Mode Switch

mode switch:

~~~text
user → kernel → user
~~~

อาจกลับ thread เดิม

context switch:

~~~text
Thread A → Thread B
~~~

สอง concept เกี่ยวกันได้แต่ไม่ใช่คำเดียวกัน

---

## 3. POSIX fork Concept

fork สร้าง child process จาก caller

conceptually:

~~~text
parent
↓ fork
parent + child
~~~

ทั้งสองมี execution flow แยกกัน

แต่ modern implementation ไม่จำเป็นต้อง copy physical RAM ทั้งหมดทันที

Copy-on-Write จะเรียน Chapter 11

---

## 4. exec Concept

exec **ไม่ใช่การสร้าง process ใหม่อีกตัว**

มันเปลี่ยน program image ของ process ที่เรียก

concept:

~~~text
same process identity context
↓ exec
old program image replaced
↓
new program image begins
~~~

รายละเอียด PID/threads/resources ขึ้นกับ POSIX/Linux semantics

---

## 5. wait / waitpid

parent ใช้ wait-family เพื่อ:

- รอ child
- รับ termination status
- reap child

ถ้า child terminate แล้ว status ยังรอ parent collect จะเกิด zombie state

---

## 6. Zombie

Zombie:

~~~text
child execution finished
but
termination record still retained
because parent has not reaped it
~~~

Zombie ไม่ได้ใช้ CPU ทำงานปกติแล้ว

แต่ยังใช้ process-table metadata บางส่วน

---

## 7. Orphan / Reparenting Concept

ถ้า parent terminate ก่อน child:

kernel ต้องจัด parent relationship ใหม่ตาม Linux process-management semantics

ไม่ควรสอนแบบตายตัวว่า “ทุกกรณีถูก process X รับเสมอ” โดยไม่ดู environment เพราะ subreaper/container/session setup มีผลได้

---

## 8. C# Mapping

คอร์สไม่ P/Invoke fork จาก live managed runtime เพื่อเลียนแบบ C แบบตรง ๆ

ใช้:

~~~csharp
Process.Start(...)
Process.WaitForExit()
Process.ExitCode
~~~

เพราะนี่คือ API ที่ C# developer ควรใช้

แต่ทฤษฎี fork/exec/wait ยังคงต้องเรียน

---

## 9. Process.Start ไม่เท่ากับ fork เสมอ

บน Unix .NET runtime เลือก native process-creation implementation ได้

ดังนั้น:

~~~text
Process.Start semantics
≠
hardcoded syscall sequence
~~~

สังเกต:

~~~bash
strace -f -e trace=process   dotnet run --no-build -c Release   --project 03-process-context-II/examples/Chapter03.csproj
~~~

---

## 10. C# Child Lifecycle

~~~csharp
using Process child = Process.Start(info)!;

Console.WriteLine(child.Id);

child.WaitForExit();

Console.WriteLine(child.ExitCode);
~~~

เชื่อม concept:

~~~text
create child
→ child runs
→ child terminates
→ parent observes completion
~~~

---

## 11. Lifecycle Model

~~~text
created
↓
runnable/running
↓
may block/wake repeatedly
↓
exit
↓
termination status
↓
parent/reaper collects status
↓
fully reaped
~~~

---

## แบบฝึกหัด

1. Context switch คืออะไร
2. mode switch ต่างอย่างไร
3. fork สร้างอะไร
4. exec สร้าง PID ใหม่เสมอหรือไม่
5. wait ทำอะไร
6. zombie คืออะไร
7. zombie ยัง execute application code อยู่หรือไม่
8. Process.Start เป็น syscall หรือ API
9. ทำไมไม่ควร P/Invoke fork แบบสุ่มใน multithreaded CLR
10. สร้าง child 3 ตัวด้วย C#
11. ให้แต่ละ child exit code ต่างกัน
12. ใช้ Stopwatch วัด completion order
13. trace process syscalls
14. อธิบาย creation order vs completion order
15. เชื่อม fork กับ COW preview

---

## Explain-It-Back

~~~text
parent
→ process creation abstraction
→ child runnable
→ scheduler
→ child execution
→ exit
→ wait/reap
~~~
