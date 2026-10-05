# Batch 01 Review — Chapters 01–03

## เป้าหมาย

ตรวจว่าผู้เรียนเข้าใจเส้นทาง:

~~~text
Hardware
  ↓
Kernel / User boundary
  ↓
System Call
  ↓
Program → Process
  ↓
PID / Process State / CPU Context
  ↓
fork
  ↓
exec
  ↓
wait / exit / reap
~~~

---

## Part A — Explain Without Notes

ตอบด้วยคำตนเอง:

1. OS คืออะไร
2. Kernel คืออะไร
3. User Space คืออะไร
4. Kernel Space คืออะไร
5. System Call มีไว้ทำไม
6. Program vs Process
7. PID vs PPID
8. PCB คือ abstraction อะไร
9. task_struct เกี่ยวข้องอย่างไร
10. CPU execution context มีอะไรบ้าง
11. Context Switch คืออะไร
12. Mode Switch ต่างจาก Context Switch อย่างไร
13. fork ทำอะไร
14. exec ทำอะไร
15. wait ทำอะไร
16. Zombie คืออะไร
17. Reparenting คืออะไร
18. Copy-on-Write เกี่ยวกับ fork อย่างไรระดับ introductory

---

## Part B — Predict

ก่อนรัน source examples ให้ทำนาย:

- PID relationships
- output order ที่ guaranteed
- output order ที่ไม่ guaranteed
- exit status
- /proc entries ระหว่าง process มีชีวิต
- zombie state ก่อน/หลัง wait

---

## Part C — Observe

ต้องทำได้โดยไม่เปิดคำตอบ:

~~~bash
uname -r
lscpu
echo $$
ps -o pid,ppid,stat,comm -p $$
pstree -p
cat /proc/$$/status
cat /proc/$$/maps
ls -l /proc/$$/fd
strace -e trace=write 01-os-introduction/bin/hello
~~~

และ:

~~~bash
strace -f -e trace=process 03-process-context-II/bin/exec-demo
~~~

---

## Part D — Debug

### Scenario 1

student บอก:

"printf เป็น system call"

แก้คำอธิบายให้ถูก

### Scenario 2

student บอก:

"ทุกครั้งที่เข้า kernel คือ context switch ไป process อื่น"

แก้ให้ถูก

### Scenario 3

student บอก:

"exec สร้าง child"

แก้ให้ถูก

### Scenario 4

student บอก:

"zombie ยัง execute อยู่แต่โดนค้าง"

แก้ให้ถูก

### Scenario 5

student บอก:

"Linux PCB คือ struct PCB"

แก้ให้ถูก

---

## Part E — Mini Midterm Practice

### Q1

อธิบาย Hardware → Application stack และตำแหน่ง system-call boundary

### Q2

มี executable /usr/bin/sleep และมี sleep 100 สาม process

กี่ program file และกี่ process instances ใน scenario นี้

### Q3

parent PID 500 เรียก fork สำเร็จและได้ child PID 501

fork return value ใน parent และ child คืออะไร

### Q4

ทำไม successful exec ไม่ควรกลับมาบรรทัดถัดไป

### Q5

process child exit แล้ว parent ไม่ wait ทันที สถานะใดสามารถสังเกตได้

### Q6

อธิบายว่าทำไม user→kernel→same process ไม่จำเป็นต้องเป็น process context switch

### Q7

ให้วาด:

~~~text
Ready → Running → Waiting → Ready
~~~

พร้อม event ที่ทำให้เปลี่ยนแต่ละขั้น

### Q8

PCB abstraction ต้องเก็บ CPU context เพราะอะไร

### Q9

ทำไม /proc/PID/maps ไม่ควรจำ address เป็นค่าตายตัว

### Q10

ทำไม fork บน Linux ไม่จำเป็นต้อง copy physical RAM ทั้ง process ทันที

---

## Answer Guide

### A1

OS จัด resource, abstraction, isolation และ interfaces; kernel เป็น privileged core ส่วนหนึ่งของระบบ

### A2

หนึ่ง executable program file สามารถมีสาม process instances

### A3

parent ได้ 501; child ได้ 0

### A4

exec replace current process image; ถ้าสำเร็จ old instruction stream ไม่ดำเนินต่อ

### A5

zombie/waitable terminated child state จนถูก reap

### A6

privilege transition สามารถเกิดและ return เข้า A เดิมโดย scheduler ไม่ต้องเลือก B

### A7

ตัวอย่าง: dispatch, blocking event/I/O wait, event completion

### A8

เพื่อ resume execution ที่ตำแหน่งและ machine state ที่ถูกต้อง

### A9

ASLR/PIE/mappings/runtime แตกต่างข้าม run

### A10

Copy-on-Write และ shared backing ชั่วคราวลด immediate copying

---

## Coverage Matrix

| Topic | Theory | Diagram | Code | Lab | Exercise | Quiz |
|---|---:|---:|---:|---:|---:|---:|
| OS/Kernel/User Space | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| System Calls | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Hardware–OS–Application | ✅ | ✅ | — | ✅ | ✅ | ✅ |
| Program vs Process | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| PID/PPID | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Process States | ✅ | ✅ | — | ✅ | ✅ | ✅ |
| PCB/task_struct relation | ✅ | ✅ | — | ✅ | ✅ | ✅ |
| CPU State | ✅ | ✅ | — | ✅ | ✅ | ✅ |
| Context Switch | ✅ | ✅ | — | ✅ | ✅ | ✅ |
| fork | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| exec | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| wait/waitpid | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Zombie | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Reparenting | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Copy-on-Write intro | ✅ | ✅ | — | ✅ | ✅ | ✅ |

Hardware–OS conceptsที่ไม่มี code โดยตรงถูกพิสูจน์ผ่าน Linux observation แทนที่จะสร้าง code ปลอมเพื่อให้มี checkbox

---

## Completion Gate

พร้อม Batch 2 เมื่อสามารถ:

1. อธิบายทุกข้อ Part A โดยไม่ท่อง definition
2. build examples ทั้ง 3 chapters โดยไม่มี compiler error
3. ใช้ strace และ /proc ได้
4. trace fork/exec/wait ได้
5. แยก mode switch กับ process context switch ได้
6. อธิบาย zombie โดยไม่ใช้คำว่า "process ยังรันค้าง"
7. วาด end-to-end process lifecycle ได้

Batch ถัดไป:

- Chapter 04 — Concurrency Part I
- Chapter 05 — Concurrency Part II
- Midterm Review 1–5
- Chapter 06 — Synchronization Part I
