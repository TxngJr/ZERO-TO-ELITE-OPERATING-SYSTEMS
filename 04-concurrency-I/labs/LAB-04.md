# Lab 04 — Threads, Interleaving and Lost Update

## Goal

สังเกต threads จริงบน Linux และสร้าง logical race condition โดยไม่พึ่ง C undefined behavior ของ plain data race

## Build

~~~bash
cd 04-concurrency-I
make clean
make
~~~

## A. Thread identity

~~~bash
./bin/thread-basic
~~~

รัน 5 รอบและจด:

- PID
- TIDs
- output order

## B. Observe kernel-visible threads

terminal A:

~~~bash
./bin/thread-observe
~~~

terminal B:

~~~bash
ps -L -p PID -o pid,tid,psr,stat,comm
ls /proc/PID/task
~~~

เลือก TID หนึ่งตัว:

~~~bash
cat /proc/PID/task/TID/status | head -30
~~~

## C. Lost update

~~~bash
for i in {1..10}; do ./bin/lost-update; done
~~~

ตอบ:

1. expected เท่าไร
2. observed เหมือนกันทุกครั้งหรือไม่
3. ทำไม counter เป็น _Atomic แล้วผลยังผิดได้
4. difference ระหว่าง atomic object access กับ atomic compound operation คืออะไร

## D. CPU observation

ขณะ thread-observe รัน:

~~~bash
top -H -p PID
~~~

หรือ:

~~~bash
ps -L -p PID -o pid,tid,psr,stat,comm
~~~

PSR อาจเปลี่ยนได้

## E. Predict Interleavings

ให้ shared logical value เริ่ม 0 และแต่ละ "increment" แยกเป็น load/store

เขียน interleaving อย่างน้อย 3 แบบ:

- final 2
- final 1
- schedule ที่ A ทำเกือบทั้งหมดก่อน B

## Cleanup

โปรแกรม observe จบเองหลังประมาณ 20 วินาที หรือใช้ Ctrl+C

## Explain

เขียนคำอธิบาย 1 หน้า:

"เหตุใด concurrent program ที่รันผ่าน 1,000 ครั้งยังไม่พิสูจน์ว่า thread-safe"
