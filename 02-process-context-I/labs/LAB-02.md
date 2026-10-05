# Lab 02 — Inspect a Live Process

## Goal

เชื่อม textbook process model กับข้อมูลที่ Fedora/Linux expose จริง

## Build

~~~bash
cd 02-process-context-I
make clean
make
~~~

## A. Program vs processes

~~~bash
sleep 300 &
sleep 300 &
pgrep -a sleep
~~~

ตอบ: executable เดียวกันหรือไม่ และ PID เหมือนกันหรือไม่

cleanup:

~~~bash
pkill -x sleep
~~~

## B. Memory addresses

~~~bash
./bin/memory-layout
./bin/memory-layout
~~~

เปรียบเทียบ addresses

จากนั้น:

~~~bash
readelf -S ./bin/memory-layout | less
nm -n ./bin/memory-layout | less
~~~

อย่าเหมารวม ELF section layout กับ live process mappings แบบ 1:1

## C. Process metadata

terminal A:

~~~bash
./bin/process-info
~~~

terminal B:

~~~bash
ps -o pid,ppid,state,stat,comm -p PID
cat /proc/PID/status
cat /proc/PID/maps
ls -l /proc/PID/fd
~~~

## D. Shell process

~~~bash
echo $$
ps -o pid,ppid,stat,comm -p $$
cat /proc/$$/status | head -30
~~~

## E. Process tree

~~~bash
pstree -p | less
~~~

หา terminal/shell ของตนเอง

## Questions

1. process-info มี PPID เป็น process ใด
2. memory mappings มี executable, shared libraries, stack หรือไม่
3. fd 0/1/2 ชี้ไปที่อะไรใน terminal session นี้
4. ทำไม output ของ maps ไม่ควรถูกจำเป็นเลขตายตัว
5. textbook PCB ช่วยอธิบายสิ่งที่เห็นจาก /proc ได้อย่างไร
