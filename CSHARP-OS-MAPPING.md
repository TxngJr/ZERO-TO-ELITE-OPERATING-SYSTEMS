# C# / .NET ↔ Operating Systems Mapping

เอกสารนี้ป้องกันความผิดพลาดจากการเอา high-level API ไปเท่ากับ kernel primitive แบบ 1:1

| C# / .NET | Concept ที่ใช้เรียน | หมายเหตุ |
|---|---|---|
| Process | OS process wrapper | ไม่ใช่ PCB |
| Process.Start | process creation abstraction | native implementation เปลี่ยนได้ |
| Process.WaitForExit | wait for process completion | ไม่ควรสรุปว่าเป็น syscall ชื่อ wait ตรง ๆ |
| Thread | managed thread abstraction | บน Linux เชื่อมกับ OS thread/task |
| Thread.Start | thread lifecycle start | scheduler เป็นผู้ตัดสิน execution timing |
| Thread.Join | completion wait | ไม่ได้รวม result ให้เอง |
| Thread.Sleep | timed blocking | ไม่ใช่ correctness synchronization |
| object + lock | object-monitor mutual exclusion | ใช้ร่วมกับ Monitor.Wait/Pulse ได้ |
| System.Threading.Lock + lock | dedicated modern mutual exclusion | compiler ใช้ Lock EnterScope semantics |
| Monitor.Wait | condition wait | ต้องถือ monitor object |
| Monitor.Pulse/PulseAll | signal monitor waiters | ไม่เก็บ token แบบ semaphore |
| Volatile | visibility/order primitive | ไม่ทำ compound operation atomic |
| Interlocked | atomic RMW | เหมาะ state transition ขนาดเล็ก |
| SemaphoreSlim | permit counting | ใช้ try/finally รอบ Release pattern |
| ReaderWriterLockSlim | readers/writer synchronization | ไม่ assume strict FIFO fairness |
| Barrier | phase synchronization | participant count/protocol ต้องตรง |
| MemoryMappedFile | file-backed mapping abstraction | ใช้สอน shared/private mapping |
| DllImport | native interop | ใช้เมื่อคอร์สต้องเห็น Linux primitive ตรงขึ้น |
| mmap via P/Invoke | VM mapping | Chapter 11 Linux x86-64 lab |
| mprotect via P/Invoke | page protection | child process ใช้ทดสอบ forbidden write |

---

# lock Statement มีสองกรณีที่ต้องแยก

## A. System.Threading.Lock

~~~csharp
static readonly Lock Gate = new();

lock (Gate)
{
    // mutual exclusion
}
~~~

C# รุ่นใหม่มี special handling สำหรับ type นี้

ใช้เป็น dedicated mutual-exclusion primitive

## B. object Monitor

~~~csharp
static readonly object Gate = new();

lock (Gate)
{
    while (!condition)
        Monitor.Wait(Gate);

    Monitor.PulseAll(Gate);
}
~~~

นี่คือ monitor condition-synchronization pattern ที่ไฟล์ Activity 03 และ Thread-Safe Buffer ใช้

## กฎ

~~~text
System.Threading.Lock
ไม่ควรถูกอธิบายว่าเท่ากับ object Monitor ทุกประการ

Monitor.Wait/Pulse
ต้องสัมพันธ์กับ monitor ownership ของ object ที่ใช้
~~~

---

# API ≠ Syscall Rule

ตัวอย่าง:

~~~text
Console.WriteLine
≠ one guaranteed write syscall

Process.Start
≠ one fixed native syscall sequence

Thread
≠ CPU core

WorkingSet64
≠ whole virtual address space
~~~

ใช้ Linux observation tools เพื่อพิสูจน์ runtime behavior:

~~~bash
strace
ps
ps -L
/proc/PID/status
/proc/PID/maps
/proc/PID/task
pmap
vmstat
~~~
