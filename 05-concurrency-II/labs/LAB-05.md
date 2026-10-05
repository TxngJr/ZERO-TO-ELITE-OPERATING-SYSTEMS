# Lab 05 — Logical Races, Visibility and Thread Lifecycle

## Goal

แยก race condition ออกจาก C data race และฝึก reasoning เรื่อง thread lifecycle/visibility

## Build

~~~bash
cd 05-concurrency-II
make clean
make
~~~

## A. Check–Then–Act

~~~bash
./bin/check-then-act
~~~

ควรเห็น buyers ทั้งสอง check stock=1 ก่อน act

ตอบ:

1. stock เป็น atomic หรือไม่
2. individual decrement atomic หรือไม่
3. ทำไม final stock ยังติดลบ
4. invariant ที่จริงควรเป็นอะไร
5. synchronization ต้องครอบส่วนไหน

## B. Release / Acquire

~~~bash
./bin/release-acquire
~~~

วาด:

~~~text
writer payload=42
      ↓
store-release ready=1
      ↓ synchronizes-with
load-acquire sees 1
      ↓
reader payload
~~~

อย่าแก้ demo เป็น plain int ready แล้วสรุปจาก "เครื่องฉันยังได้ 42"

## C. Detached Thread

~~~bash
./bin/detached-demo
~~~

ตอบ:

- ทำไม main ไม่ pthread_join
- completion ถูกสื่อสารอย่างไร
- ถ้า main return ทันทีหลัง detach จะเกิดอะไรกับ worker

## D. ThreadSanitizer Discussion

ลองได้ถ้า Fedora/GCC environment รองรับ:

~~~bash
gcc -fsanitize=thread -g -O1 examples/check-then-act.c -o /tmp/check-tsan -pthread
/tmp/check-tsan
~~~

อาจไม่ report เพราะ accesses ใช้ atomics; นั่นไม่แปลว่า business invariant ถูก

จุดประสงค์คือเรียนว่า detector หา "data race class" ไม่ได้พิสูจน์ logical correctness ทั้งหมด

## E. Design Exercise

ออกแบบ pseudo-code ขาย ticket หนึ่งใบให้ผู้ใช้สองคนพร้อมกัน

เขียน:

1. broken check-then-act
2. critical section ที่ต้อง protect
3. invariant
4. failure case

ยังไม่ต้องเขียน mutex implementation; Chapter 06 จะทำจริง
