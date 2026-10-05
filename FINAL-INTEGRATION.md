# Final Integration — C# → .NET → Linux → Hardware

~~~text
C# source
↓
.NET compilation/runtime
↓
Linux process
↓
managed threads
↓
scheduler
↓
CPU
↓
shared memory / synchronization
↓
virtual address
↓
TLB / page table / MMU
↓
physical memory
↓
possible page fault
↓
kernel resolution
~~~

## End-to-End Scenario

1. Main เริ่มใน C# process
2. Process.Start สร้าง child process ผ่าน runtime/platform layer
3. Thread.Start สร้าง concurrent execution
4. shared static fields ต้อง synchronize
5. lock/Monitor/Interlocked สร้าง correctness/order
6. blocked thread ทำให้ scheduler เลือกงานอื่น
7. memory access ใช้ virtual address
8. MMU/TLB/page table translate
9. page fault อาจเข้า kernel
10. child process exit
11. WaitForExit สังเกต completion

## Final Explain-It-Back

ห้ามตอบว่า ".NET ทำให้เอง" โดยไม่อธิบายว่า:

- runtime abstraction คืออะไร
- OS responsibility คืออะไร
- hardware responsibility คืออะไร
