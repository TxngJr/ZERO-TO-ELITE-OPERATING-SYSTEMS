# Lab 08 — Monitor, Deadlock Reasoning and Livelock

## Build

~~~bash
cd 08-synchronization-III
make clean
make
~~~

## A. Monitor-Like Queue

~~~bash
./bin/monitor-queue
~~~

หา:

- state ที่ private โดย convention
- mutex
- predicates
- signal points
- final count

จากนั้นตอบ:

ทำไม caller ที่แก้ queue.count เองโดยไม่ผ่าน queue_put/get จะทำลาย abstraction

## B. Wait-For Cycle

~~~bash
./bin/waitfor-cycle
~~~

วาด graph ตาม output

หา cycle:

~~~text
T0 -> T1 -> T2 -> T0
~~~

ทดลองแก้ source ให้ edge T2->T0 เป็น false แล้ว build ใหม่

ผล cycle detection ควรเปลี่ยน

## C. Livelock

~~~bash
./bin/livelock-demo
~~~

รันหลายครั้ง:

~~~bash
for i in {1..5}; do ./bin/livelock-demo; done
~~~

ตอบ:

- threads active หรือ blocked
- useful progress เกิดเมื่อไร
- bounded demo ต่างจาก true infinite livelock อย่างไร
- randomized/asymmetric retry ช่วยอะไร

## D. Deadlock Design Audit

สำหรับ pseudo-code:

~~~text
T1: lock A -> lock B
T2: lock B -> lock A
~~~

เขียน:

1. Coffman conditions
2. wait-for cycle
3. global order solution

## E. Priority Inversion Paper Exercise

วาด timeline:

~~~text
L locks M
H wakes and blocks on M
M wakes and uses CPU
L delayed
H indirectly delayed
~~~

จากนั้นเพิ่ม priority inheritance แล้วอธิบาย timeline ใหม่

ไม่ต้องเปลี่ยน real-time scheduling policy ของ Fedora เพื่อทำ Lab นี้
