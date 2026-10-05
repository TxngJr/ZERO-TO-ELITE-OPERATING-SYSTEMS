# Fedora Setup — Zero to Elite OS

## เป้าหมาย

เตรียม Fedora ให้พร้อมสำหรับทุก Lab ใน Batch 1 โดยยังไม่ปรับ kernel หรือทำสิ่งที่เสี่ยงกับระบบหลัก

## 1. ตรวจสอบระบบ

~~~bash
cat /etc/os-release
uname -a
uname -r
uname -m
lscpu
free -h
lsblk
~~~

สิ่งที่ควรสังเกต:

- uname -m ควรเป็น x86_64 บน Acer A715-43G
- lscpu แสดง CPU, จำนวน core, logical CPU และ cache
- free -h แสดงหน่วยความจำที่ kernel มองเห็น
- lsblk แสดง block devices ไม่ใช่เพียง "ฮาร์ดดิสก์"

ค่าที่ Linux รายงานจากเครื่องจริงให้ถือเป็น source of truth ของ Lab มากกว่าสเปกที่จำจากหน้าเว็บ

## 2. ติดตั้งเครื่องมือ

~~~bash
sudo dnf install gcc make gdb strace ltrace htop perf procps-ng psmisc
~~~

เครื่องมือหลัก:

| Tool | ใช้ทำอะไร |
|---|---|
| gcc | compile C |
| make | build automation |
| gdb | debugger |
| strace | ดู system calls |
| ltrace | ดู library calls ในกรณีที่รองรับ |
| ps/top | process observation |
| htop | interactive process viewer |
| pstree | process hierarchy |
| perf | performance counters/profiling |

หาก package ใดติดตั้งอยู่แล้ว dnf จะไม่ติดตั้งซ้ำ

## 3. ตรวจ compiler

~~~bash
gcc --version
make --version
gdb --version
strace --version
~~~

## 4. สร้าง workspace

หลัง clone repository:

~~~bash
git clone https://github.com/TxngJr/ZERO-TO-ELITE-OPERATING-SYSTEMS.git
cd ZERO-TO-ELITE-OPERATING-SYSTEMS
~~~

## 5. Build rule ของคอร์ส

โดยทั่วไป:

~~~bash
gcc -Wall -Wextra -Wpedantic -std=c17 source.c -o program
~~~

flags:

- Wall: เปิด warnings กลุ่มหลัก
- Wextra: warnings เพิ่มเติม
- Wpedantic: เตือนส่วนที่ออกนอกมาตรฐาน ISO C
- std=c17: ใช้มาตรฐาน C17

POSIX API บางตัวต้องใช้ feature-test macro ซึ่ง source ในคอร์สจะกำหนดให้

## 6. man pages

ฝึกใช้:

~~~bash
man 2 write
man 2 fork
man 2 execve
man 2 waitpid
man 5 proc
man 7 signal
~~~

เลข section สำคัญ:

- 1 = user commands
- 2 = system calls
- 3 = library functions
- 5 = file formats
- 7 = overview/conventions

ตัวอย่าง fork(2) หมายถึงเอกสาร fork ใน section 2

## 7. กฎความปลอดภัยของ Lab

Batch 1 ไม่ต้อง:

- ปิด SELinux
- ปิด firewall
- run code เป็น root
- แก้ kernel parameters
- เขียน kernel module

ถ้าตัวอย่างธรรมดาต้อง sudo เพื่อรัน ให้หยุดและหาสาเหตุก่อน

## 8. Predict → Run → Observe → Explain

ทุก Lab ให้จด 4 อย่าง:

1. Predict — คิดว่าจะเกิดอะไร
2. Run — รันจริง
3. Observe — เก็บ output
4. Explain — อธิบายด้วย OS concept

การจำ output ไม่ใช่เป้าหมาย เพราะ PID, address, timing และ scheduling เปลี่ยนได้ทุกครั้ง
