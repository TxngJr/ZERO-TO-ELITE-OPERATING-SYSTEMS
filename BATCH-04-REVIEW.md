# Batch 04 Review — Chapters 10–11

## Scope

- Chapter 10 — Address Translation
- Chapter 11 — Virtual Memory
- Final Integration
- Final Review
- Capstone
- Full Coverage Audit

---

## 1. Dependency Chain

~~~text
Process Address Space
↓
Virtual Address
↓
Page / Offset
↓
MMU / TLB
↓
Page Table / PTE
↓
Physical Frame
↓
Demand Paging
↓
Page Fault
↓
Replacement / Swap
↓
mmap / COW / Protection
~~~

---

## 2. Coverage Matrix

| Topic | Theory | Diagram | Code/Sim | Lab | Exercises | Quiz |
|---|---:|---:|---:|---:|---:|---:|
| VA vs PA | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| MMU | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Page / Frame | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Page Table / PTE | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Multi-Level Tables | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| TLB | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| TLB vs Page Fault | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Demand Paging | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Minor/Major Fault | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| FIFO / OPT / LRU / Clock | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Belady | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Locality / Working Set | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Thrashing | ✅ | ✅ | simulation/paper | ✅ | ✅ | ✅ |
| Swap / zram | ✅ | ✅ | observation | ✅ | ✅ | ✅ |
| mmap | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| COW | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Protection / SIGSEGV | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |

---

## 3. Correctness Gate

ต้องแก้ misconception:

- Virtual Memory = Swap ❌
- TLB miss = Page Fault ❌
- malloc = physical RAM immediately ❌
- all page faults = errors ❌
- every page fault = disk I/O ❌
- same VA = same PA across processes ❌
- context switch always flushes entire TLB ❌
- x86-64 always uses exactly 4 paging levels ❌
- more FIFO frames always means fewer faults ❌

---

## 4. Practical Gate

~~~bash
make -C 10-address-translation clean all
make -C 11-virtual-memory clean all

python3 10-address-translation/page_table_sim.py
python3 11-virtual-memory/page_replacement_sim.py

./11-virtual-memory/bin/page-fault-demo
./11-virtual-memory/bin/cow-demo
./11-virtual-memory/bin/mmap-file
./11-virtual-memory/bin/protection-demo
~~~

---

## 5. Explain-It-Back Gate

ตอบโดยไม่เปิดโน้ต:

1. VA→PA flow
2. VPN/offset
3. TLB hit/miss
4. multi-level page tables
5. demand paging
6. minor/major fault
7. COW
8. page replacement
9. thrashing
10. mmap
11. mprotect
12. SIGSEGV

ผ่านแล้วจึงทำ Final Integration และ Capstone
