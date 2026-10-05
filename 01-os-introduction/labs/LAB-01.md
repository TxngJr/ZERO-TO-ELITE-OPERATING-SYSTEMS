# Lab 01 — Crossing the User/Kernel Boundary

## Goal

พิสูจน์ว่า program ธรรมดาใช้ kernel services ผ่าน system calls และฝึกแยก library-level thinking ออกจาก kernel-level thinking

## Build

~~~bash
cd 01-os-introduction
make clean
make
~~~

## Experiment A — prediction

ก่อนรัน ให้ตอบ:

1. ./bin/hello จะพิมพ์อะไร
2. strace จะเห็นเพียง write หรือ system calls อื่นด้วย
3. ทำไม program ที่มี main สั้นมากจึงอาจมี system calls จำนวนมาก

## Experiment B — trace write only

~~~bash
strace -e trace=write ./bin/hello
~~~

จด:

- fd
- byte count
- return value

## Experiment C — trace direct SYS_write example

~~~bash
strace -e trace=write ./bin/syscall-write
~~~

เปรียบเทียบกับ hello

## Experiment D — complete trace

~~~bash
strace -o trace.txt ./bin/hello
less trace.txt
~~~

หา:

- execve
- mmap
- write
- exit_group

อย่าจำว่าทุกเครื่องต้องมี sequence เหมือนกัน 100% เพราะ loader/library/runtime environment อาจต่างกัน

## Experiment E — process information

~~~bash
echo $$
cat /proc/$$/status | head -30
ls -l /proc/$$/fd
~~~

## Debug challenge

ถ้า strace: command not found:

~~~bash
sudo dnf install strace
~~~

ถ้า binary ไม่มี:

~~~bash
make
ls -l bin
~~~

## Explain

เขียน 1 paragraph อธิบายว่า:

"เหตุใด Hello World จึงเป็นตัวอย่างของ hardware–OS–application interaction"
