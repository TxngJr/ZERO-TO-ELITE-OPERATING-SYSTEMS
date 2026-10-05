# C# ↔ OS Mapping

| C#/.NET | OS Concept |
|---|---|
| Thread | execution thread |
| Thread.Start | make thread runnable |
| Thread.Join | wait for completion |
| Thread.Sleep | timed blocking |
| lock | Monitor.Enter/Exit-based mutual exclusion |
| Monitor.Wait | release monitor + block + reacquire |
| Monitor.PulseAll | wake monitor waiters |
| Interlocked | atomic read-modify-write |
| Volatile | visibility/order primitive for specific patterns |
| SemaphoreSlim | permit counting in managed code |
| Mutex | OS-capable mutex abstraction |
| ReaderWriterLockSlim | readers/writer synchronization |
| Process | operating-system process wrapper |
| Process.Start | start child process through runtime/platform implementation |
| Process.WaitForExit | wait for child completion |
| MemoryMappedFile | file/shared memory mapping abstraction |
| Marshal.AllocHGlobal | unmanaged allocation |
| DllImport | call native/Linux API when necessary |

## Critical Principle

C#/.NET abstraction และ Linux primitive ไม่จำเป็นต้อง map 1:1

เราใช้ C# API ร่วมกับ Linux observation tools เพื่อเข้าใจของจริง
