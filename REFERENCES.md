# Official References

ใช้เอกสาร official/upstream เป็นหลักสำหรับ behavior ที่เปลี่ยนตาม .NET/kernel version

## .NET / C#

- C# lock statement:
  https://learn.microsoft.com/dotnet/csharp/language-reference/statements/lock

- System.Threading.Lock:
  https://learn.microsoft.com/dotnet/api/system.threading.lock

- Monitor:
  https://learn.microsoft.com/dotnet/api/system.threading.monitor

- Monitor.Wait:
  https://learn.microsoft.com/dotnet/api/system.threading.monitor.wait

- Monitor.PulseAll:
  https://learn.microsoft.com/dotnet/api/system.threading.monitor.pulseall

- Threading objects and features:
  https://learn.microsoft.com/dotnet/standard/threading/threading-objects-and-features

- ReaderWriterLockSlim:
  https://learn.microsoft.com/dotnet/api/system.threading.readerwriterlockslim

- Environment.SystemPageSize:
  https://learn.microsoft.com/dotnet/api/system.environment.systempagesize

- Memory-Mapped Files:
  https://learn.microsoft.com/dotnet/standard/io/memory-mapped-files

## Linux Scheduling

- EEVDF:
  https://docs.kernel.org/scheduler/sched-eevdf.html

- CFS historical design:
  https://docs.kernel.org/scheduler/sched-design-CFS.html

- Scheduler domains:
  https://docs.kernel.org/scheduler/sched-domains.html

- Deadline scheduling:
  https://docs.kernel.org/scheduler/sched-deadline.html

## Linux Memory

- Memory management:
  https://docs.kernel.org/mm/index.html

- Page tables:
  https://docs.kernel.org/mm/page_tables.html

- x86-64 5-level paging:
  https://docs.kernel.org/arch/x86/x86_64/5level-paging.html

- pagemap:
  https://docs.kernel.org/admin-guide/mm/pagemap.html

- Transparent Huge Pages:
  https://docs.kernel.org/admin-guide/mm/transhuge.html

- HugeTLB:
  https://docs.kernel.org/admin-guide/mm/hugetlbpage.html

- zram:
  https://docs.kernel.org/admin-guide/blockdev/zram.html

## Linux APIs

- mmap:
  https://man7.org/linux/man-pages/man2/mmap.2.html

- mprotect:
  https://man7.org/linux/man-pages/man2/mprotect.2.html

- getrusage:
  https://man7.org/linux/man-pages/man2/getrusage.2.html

## Rule

ก่อนสรุป runtime/kernel behavior ที่เป็น implementation detail:

~~~bash
dotnet --info
uname -r
~~~

แล้วเทียบกับ documentation ของ version/generation ที่ใช้งาน
