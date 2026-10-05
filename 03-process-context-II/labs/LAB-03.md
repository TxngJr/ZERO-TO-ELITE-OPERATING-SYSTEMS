# Lab 03 — fork, exec, wait, Zombie and Reparenting

## Goal

สังเกต process lifecycle จริงและแยก API semantics ออกจาก scheduling behavior

## Build

~~~bash
cd 03-process-context-II
make clean
make
~~~

## A. fork

~~~bash
./bin/fork-demo
./bin/fork-demo
./bin/fork-demo
~~~

จดลำดับ parent/child output

ตอบ:

- อะไร guaranteed
- อะไรขึ้นกับ scheduling
- parent ได้ return value อะไร
- child ได้ return value อะไร

## B. exec

~~~bash
./bin/exec-demo
~~~

trace:

~~~bash
strace -f -e trace=process ./bin/exec-demo
~~~

หา execution creation, execve-like call, waiting และ exit

ชื่อ syscall ที่เห็นจริงอาจไม่ตรงกับชื่อ high-level C API 1:1

## C. wait status

~~~bash
./bin/wait-demo
~~~

พิสูจน์ว่า parent ได้ exit status 42 ผ่าน wait status macros

## D. Zombie

~~~bash
./bin/zombie-demo
~~~

ภายใน 20 วินาที:

~~~bash
ps -o pid,ppid,stat,comm -p PARENT_PID,CHILD_PID
~~~

หลังโปรแกรม parent wait แล้ว รัน ps ซ้ำ

เขียนความแตกต่าง

## E. Reparenting

~~~bash
./bin/orphan-demo
~~~

เปรียบเทียบ PPID ของ child ก่อนและหลัง parent exit

อย่าคาดเดาว่าต้องกลายเป็น 1 ในทุก environment

## F. Background job

~~~bash
sleep 20 &
echo $!
jobs
ps -o pid,ppid,stat,comm -p $!
~~~

สังเกต shell คืน prompt ทันที

## G. Shell external command vs builtin

~~~bash
type ls
type cd
~~~

อธิบายว่าทำไม cd ต้องเปลี่ยน state ของ shell เอง

## Debugging

### exec failed

ตรวจ:

~~~bash
echo $PATH
command -v ls
~~~

### zombie ไม่ทันเห็น

รันใหม่; demo ตั้งใจให้ 20 วินาที

### orphan output กลับ prompt ปะปน

เป็นไปได้เพราะ parent จบแต่ child ยังเขียน terminal เดิม ให้ดู PID/PPID ไม่ใช่สวยงามของ prompt

## Final explanation

วาด diagram ของ:

~~~text
shell → child creation → exec → program → exit → wait/reap
~~~

แล้ววงจุดที่:

- PID ถูกสร้าง
- image ถูก replace
- child termination เกิด
- zombie window สามารถเกิด
- parent reaps
