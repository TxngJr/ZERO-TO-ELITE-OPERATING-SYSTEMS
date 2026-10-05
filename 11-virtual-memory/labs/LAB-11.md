# Lab 11 — Demand Paging, COW, mmap and Protection

## Build

~~~bash
cd 11-virtual-memory
make clean
make
~~~

## A. Memory Baseline

~~~bash
free -h
swapon --show
zramctl
vmstat 1 5
~~~

เขียนสิ่งที่เครื่องจริงรายงาน อย่า assume ว่าต้องมี swap/zram แบบใดแบบหนึ่ง

## B. Demand Paging

~~~bash
./bin/page-fault-demo
~~~

บันทึก:

~~~text
before mmap
after mmap
after touching pages
~~~

ตอบ:

1. mmap อย่างเดียวจำเป็นต้อง populate physical page ทุกหน้าไหม
2. touch แล้ว minor faults เปลี่ยนอย่างไร
3. ทำไม exact count อาจไม่เท่าจำนวน 4 KiB pages

## C. External Fault Counters

~~~bash
/usr/bin/time -v ./bin/page-fault-demo
~~~

ดู Minor/Major Page Faults และ Maximum RSS

## D. Copy-on-Write

~~~bash
./bin/cow-demo
~~~

วาด:

~~~text
before fork
↓
shared COW backing
↓
child write
↓
private copy
↓
parent value vs child value
~~~

## E. File-Backed mmap

~~~bash
./bin/mmap-file
~~~

ตอบ:

- MAP_SHARED หมายถึงอะไร
- msync ทำอะไรใน demo
- pointer write ไปสัมพันธ์ file อย่างไร

## F. Page Protection

~~~bash
./bin/protection-demo
~~~

expected concept:

~~~text
parent remains alive
child attempts forbidden write
kernel delivers SIGSEGV
parent observes signal
~~~

## G. Page Replacement Simulator

~~~bash
python3 page_replacement_sim.py
~~~

ทำ FIFO และ LRU ด้วยมือก่อน

## H. Belady Anomaly

reference:

~~~text
1 2 3 4 1 2 5 1 2 3 4 5
~~~

คำนวณ FIFO 3 frames และ 4 frames

## I. Thrashing — Safe Paper Lab

ไม่สร้าง workload ที่ตั้งใจทำให้ laptop thrash

ให้จำลอง:

~~~text
Process A working set = 100 pages
Process B working set = 100 pages
Available frames = 120
~~~

วิเคราะห์ pressure และแนวคิดลด multiprogramming

## J. Full Memory Flow

วาด:

~~~text
malloc/mmap
↓
virtual mapping
↓
first load/store
↓
TLB/page-table path
↓
possible page fault
↓
kernel resolution
↓
physical backing
↓
subsequent normal accesses
~~~

เพิ่ม branch สำหรับ:

- COW write
- invalid write
- file-backed major fault concept
